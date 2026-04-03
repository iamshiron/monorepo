using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Shiron.HonamiGit.API.DTOs;
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
        ?? builder.Configuration.GetSection("HONAMIG_ConnectionStrings")["Default"]
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
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(c => {
        c.Title = "HonamiGit API";
        c.Theme = ScalarTheme.Purple;
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();
app.MapGet("/api/health", () => Results.Ok(HealthResponseDTO.Ok)).WithName("Health").Produces<HealthResponseDTO>();

app.Run();
