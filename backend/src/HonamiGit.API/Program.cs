using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Scalar.AspNetCore;
using Shiron.HonamiGit.API.Configuration;
using Shiron.HonamiGit.API.Endpoints;
using Shiron.HonamiGit.API.Services;
using Shiron.HonamiGit.DB;
using Shiron.HonamiGit.DB.Schema;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();
builder.Services.AddIdentity<User, IdentityRole<Guid>>(c => {
    c.Password.RequireDigit = false;
    c.Password.RequiredLength = 4;
    c.Password.RequireNonAlphanumeric = false;
    c.Password.RequireUppercase = false;
    c.Password.RequireLowercase = false;

    c.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<HonamiGitDb>().AddDefaultTokenProviders();

builder.Services.AddDbContext<HonamiGitDb>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetSection("HONAMI_GIT_ConnectionStrings")["Default"]
        ?? throw new InvalidOperationException("Database connection string not configured")
    ));

builder.Services.ConfigureApplicationCookie(c => {
    c.Events.OnRedirectToLogin = context => {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
    c.Events.OnRedirectToAccessDenied = context => {
        context.Response.StatusCode = StatusCodes.Status403Forbidden;
        return Task.CompletedTask;
    };
});
builder.Services.AddAuthentication()
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationOptions.SchemeName, null);
builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<StorageOptions>(options => {
    options.Endpoint = builder.Configuration["HONAMI_GIT_MINIO_ENDPOINT"] ?? throw new InvalidOperationException("Minio endpoint not configured");
    options.AccessKey = builder.Configuration["HONAMI_GIT_MINIO_ACCESS_KEY"] ?? throw new InvalidOperationException("Minio access key not configured");
    options.SecretKey = builder.Configuration["HONAMI_GIT_MINIO_SECRET_KEY"] ?? throw new InvalidOperationException("Minio secret key not configured");
    options.UseSsl = bool.TryParse(builder.Configuration["HONAMI_GIT_MINIO_USE_SSL"], out var ssl) && ssl;
});

builder.Services.AddSingleton<IMinioClient>(sp => {
    var opts = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
    return new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .WithSSL(opts.UseSsl)
        .Build();
});
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<HonamiGitDb>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var config = services.GetRequiredService<IConfiguration>();

        var adminEmail = config["ADMIN_EMAIL"] ?? "admin@shiron.io";
        var adminPassword = config["ADMIN_PASSWORD"] ?? "admin";
        var adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole)) {
            await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
        }

        var existingAdmin = await userManager.FindByNameAsync("admin") ?? await userManager.FindByEmailAsync(adminEmail);
        if (existingAdmin == null) {
            var admin = new User {
                DisplayName = "Admin",
                Email = adminEmail,
                UserName = "admin",
                EmailConfirmed = true
            };

            var res = await userManager.CreateAsync(admin, adminPassword);
            if (res.Succeeded) {
                await userManager.AddToRoleAsync(admin, adminRole);
            } else {
                var errors = string.Join(", ", res.Errors.Select(e => e.Description));
                throw new Exception($"Failed to create admin user: {errors}");
            }
        }
    } catch (Exception e) {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(e, "A fatal error occurred applying DB migrations or seeding the admin user.");
        throw;
    }
}

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "HonamiGit API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "OK" }));
app.MapIdentityEndpoints();
app.MapKeyEndpoints();
app.MapApiKeyEndpoints();

app.Run();
