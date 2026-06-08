using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Minio;
using Scalar.AspNetCore;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;
using Shiron.ResonanceSystem.Services;
using Shiron.ResonanceSystem.API.Configuration;
using Shiron.ResonanceSystem.API.Endpoints;
using Shiron.ResonanceSystem.API.Seeders;
using Tesseract;

Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
builder.Services.AddSingleton<ObjectPool<TesseractEngine>>(sp => {
    var provider = sp.GetRequiredService<ObjectPoolProvider>();
    var policy = new TesseractEnginePolicy("eng");
    return provider.Create(policy);
});
builder.Services.AddSingleton<IOCRService, OCRService>();

builder.Services.AddOpenApi();
builder.Services.AddIdentity<User, IdentityRole<Guid>>(c => {
    c.Password.RequireDigit = false;
    c.Password.RequiredLength = 4;
    c.Password.RequireNonAlphanumeric = false;
    c.Password.RequireUppercase = false;
    c.Password.RequireLowercase = false;

    c.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<ResSystemDbContext>().AddDefaultTokenProviders();

builder.Services.AddDbContext<ResSystemDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetSection("RESONANCE_SYSTEM_ConnectionStrings")["Default"]
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

builder.Services.AddAuthorization(options => {
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

builder.Services.Configure<StorageOptions>(options => {
    options.Endpoint = builder.Configuration["RESONANCE_SYSTEM_MINIO_ENDPOINT"] ?? "localhost:9000";
    options.AccessKey = builder.Configuration["RESONANCE_SYSTEM_MINIO_ACCESS_KEY"] ?? "minioadmin";
    options.SecretKey = builder.Configuration["RESONANCE_SYSTEM_MINIO_SECRET_KEY"] ?? "minioadmin";
    options.UseSsl = bool.TryParse(builder.Configuration["RESONANCE_SYSTEM_MINIO_USE_SSL"], out var ssl) && ssl;
    options.BucketAssets = builder.Configuration["RESONANCE_SYSTEM_MINIO_BUCKET_IMAGES"] ?? "resonance-system-assets";
    options.BucketUserData = builder.Configuration["RESONANCE_SYSTEM_BUCKET_USER_DATA"] ?? "resonance-system-user-data";
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

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<ResSystemDbContext>();
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
using (var scope = app.Services.CreateScope()) {
    scope.ServiceProvider.SeedCharacters();
}

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "Resonance System API";
        options.Theme = ScalarTheme.Purple;
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "OK" }));
app.MapIdentityEndpoints();

var api = app.MapGroup("/api");
api.MapContentEndpoints();
api.MapInventoryEndpoints();
api.MapScanEndpoints();

app.Run();
