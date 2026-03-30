using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shiron.TheArchive.DB;
using Shiron.TheArchive.DB.Schema;

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
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();
using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<ArchiveDbContext>();
        context.Database.Migrate();

        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        var config = services.GetRequiredService<IConfiguration>();

        var adminEmail = config["ADMIN_EMAIL"] ?? "admin@archive.local";
        var adminPassword = config["ADMIN_PASSWORD"] ?? "archive_dev";
        var adminRole = "Admin";

        if (!await roleManager.RoleExistsAsync(adminRole)) {
            await roleManager.CreateAsync(new IdentityRole<Guid>(adminRole));
        }

        var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "Archive API";
        options.Theme = ScalarTheme.Purple;
    });
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { Status = "OK" }));

app.Run();
