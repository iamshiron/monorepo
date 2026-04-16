using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Minio;
using Scalar.AspNetCore;
using Shiron.HonamiGit.API.Configuration;
using Shiron.HonamiSystem.Server.Endpoints;
using Shiron.TheArchive.API.Services;
using Shiron.TheArchive.DB;
using Shiron.HonamiSystem.Schema;

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
}).AddEntityFrameworkStores<ArchiveDbContext>().AddDefaultTokenProviders();

builder.Services.AddDbContext<ArchiveDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetSection("ARCHIVE_ConnectionStrings")["Default"]
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
    options.Endpoint = builder.Configuration["ARCHIVE_MINIO_ENDPOINT"] ?? "localhost:9000";
    options.AccessKey = builder.Configuration["ARCHIVE_MINIO_ACCESS_KEY"] ?? "minioadmin";
    options.SecretKey = builder.Configuration["ARCHIVE_MINIO_SECRET_KEY"] ?? "minioadmin";
    options.UseSsl = bool.TryParse(builder.Configuration["ARCHIVE_MINIO_USE_SSL"], out var ssl) && ssl;
    options.BucketImages = builder.Configuration["ARCHIVE_MINIO_BUCKET_IMAGES"] ?? "archive-images";
});

builder.Services.AddSingleton<IMinioClient>(sp => {
    var opts = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
    return new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .WithSSL(opts.UseSsl)
        .Build();
});
builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddScoped<IApiKeyService, ApiKeyService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<ArchiveDbContext>();
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
        options.Title = "Archive API";
        options.Theme = ScalarTheme.Purple;
    });
}
app.UseHttpsRedirection();
app.UseMiddleware<ApiKeyMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "OK" }));

app.MapIdentityEndpoints();

var api = app.MapGroup("/api");
api.MapBrandEndpoints();
api.MapModelEndpoints();
api.MapCarEndpoints();
api.MapImageEndpoints();
api.MapStatisticsEndpoints();
api.MapApiKeyEndpoints();

app.Run();
