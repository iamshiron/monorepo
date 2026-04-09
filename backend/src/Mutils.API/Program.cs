using System.Security.Claims;
using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Scalar.AspNetCore;
using Shiron.Mutils.API.Endpoints;
using Shiron.Mutils.API.Configuration;
using Shiron.Mutils.API.Services;
using Shiron.Mutils.DB;
using Shiron.Mutils.API.Services.Impl;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<MutilsDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("Default")
        ?? builder.Configuration.GetSection("MUTILS_ConnectionStrings")["Default"]
        ?? throw new InvalidOperationException("Database connection string not configured")
    ));

builder.Services.Configure<StorageOptions>(options => {
    options.Endpoint = builder.Configuration["MUTILS_MINIO_ENDPOINT"] ?? "localhost:9000";
    options.AccessKey = builder.Configuration["MUTILS_MINIO_ACCESS_KEY"] ?? "minioadmin";
    options.SecretKey = builder.Configuration["MUTILS_MINIO_SECRET_KEY"] ?? "minioadmin";
    options.UseSsl = bool.TryParse(builder.Configuration["MUTILS_MINIO_USE_SSL"], out var ssl) && ssl;
    options.BucketAssets = builder.Configuration["MUTILS_MINIO_BUCKET_ASSETS"] ?? "mutils-assets";
    options.BucketUserData = builder.Configuration["MUTILS_MINIO_BUCKET_USER_DATA"] ?? "mutils-user-data";
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMudaeParser, MudaeParser>();
builder.Services.AddScoped<IKakeraLogParser, KakeraLogParser>();
builder.Services.AddScoped<IOptimizerService, OptimizerService>();
builder.Services.AddScoped<IExportService, ExportService>();
builder.Services.AddScoped<IStorageService, MinioStorageService>();
builder.Services.AddHostedService<ImageProcessingService>();

builder.Services.AddSingleton<IMinioClient>(sp => {
    var opts = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
    return new MinioClient()
        .WithEndpoint(opts.Endpoint)
        .WithCredentials(opts.AccessKey, opts.SecretKey)
        .WithSSL(opts.UseSsl)
        .Build();
});

var jwtSecret = builder.Configuration["MUTILS_JWT_SECRET"]
    ?? throw new InvalidOperationException("MUTILS_JWT_SECRET not configured");
var jwtIssuer = builder.Configuration["MUTILS_JWT_ISSUER"] ?? "mutils";
var jwtAudience = builder.Configuration["MUTILS_JWT_AUDIENCE"] ?? "mutils-users";

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options => {
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.WithOrigins("http://localhost:1910", "http://127.0.0.1:1910")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "Mutils API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapCollectionEndpoints();
app.MapSphereEndpoints();
app.MapListEndpoints();
app.MapOptimizerEndpoints();
app.MapUserEndpoints();
app.MapKakeraEndpoints();
app.MapCalculatorEndpoints();
app.MapProfileEndpoints();

using (var scope = app.Services.CreateScope()) {
    var db = scope.ServiceProvider.GetRequiredService<MutilsDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
