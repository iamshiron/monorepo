using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shiron.HonamiSystem.DB;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.HonamiSystem.Plugins.ExamplePlugin;
using Shiron.HonamiSystem.SDK;
using Shiron.HonamiSystem.Server.Endpoints;
using Shiron.HonamiSystem.Server.Services;
using Shiron.HonamiSystem.Services;

Env.TraversePath().Load();
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddOpenApi();

var pluginRegistry = new PluginRegistry();
builder.Services.AddSingleton<IPluginRegistry>(pluginRegistry);

builder.Services.AddIdentity<User, IdentityRole<Guid>>(c => {
    c.Password.RequireDigit = false;
    c.Password.RequiredLength = 4;
    c.Password.RequireNonAlphanumeric = false;
    c.Password.RequireUppercase = false;
    c.Password.RequireLowercase = false;

    c.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<HonamiSystemDb>().AddDefaultTokenProviders();

builder.Services.AddDbContext<HonamiSystemDb>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetSection("HONAMI_SYSTEM_ConnectionStrings")["Default"]
        ?? throw new InvalidOperationException("Database connection string not configured"),
        o => o.UseVector()
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

var app = builder.Build();
pluginRegistry.RegisterPlugin(new ExamplePlugin(), app.Logger);
pluginRegistry.Initialize();

app.Logger.LogInformation("Registered plugin components:");
foreach (var plugin in pluginRegistry.Plugins) {
    app.Logger.LogInformation($"- {plugin.Name}: {string.Join(", ", plugin.Components.Select(c => $"{c.Value.Name} ({c.Key})"))}");
}

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "HonamiSystem API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();
app.MapPluginEndpoints();

using (var scope = app.Services.CreateScope()) {
    var services = scope.ServiceProvider;
    try {
        var context = services.GetRequiredService<HonamiSystemDb>();
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
                Name = "Admin",
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

app.MapGroup("/api/account").WithTags("Account").MapIdentityEndpoints();
app.MapGroup("/api/chat").WithTags("Chat").MapChatEndpoints();
app.MapGroup("/api/chat").WithTags("Chat").MapChatGroupEndpoints();

app.Run();
pluginRegistry.Dispose();
