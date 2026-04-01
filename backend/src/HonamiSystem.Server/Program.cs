using Scalar.AspNetCore;
using Shiron.HonamiSystem.Plugins.ExamplePlugin;
using Shiron.HonamiSystem.SDK;
using Shiron.HonamiSystem.Server.Endpoints;
using Shiron.HonamiSystem.Server.Services;
using Shiron.HonamiSystem.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var pluginRegistry = new PluginRegistry();
pluginRegistry.RegisterPlugin(new ExamplePlugin());
builder.Services.AddSingleton<IPluginRegistry>(pluginRegistry);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.MapScalarApiReference(options => {
        options.Title = "HonamiSystem API";
        options.Theme = ScalarTheme.Purple;
    });
}

app.UseHttpsRedirection();
app.MapPluginEndpoints();

app.Run();
