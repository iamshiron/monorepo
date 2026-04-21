using Shiron.HonamiSystem.SDK;
using Shiron.HonamiSystem.Server.DTOs;
using Shiron.HonamiSystem.Services;

namespace Shiron.HonamiSystem.Server.Endpoints;

public static class PluginEndpoints {
    public static void MapPluginEndpoints(this IEndpointRouteBuilder app) {
        var router = app.MapGroup("/api/plugins").WithTags("Plugins");
        router.MapGet("/", (IPluginRegistry registry) => registry.Plugins
            .Select(p => new PluginResponseDTO(
                p.Group,
                p.Name,
                p.Version,
                p.Triple
            ))
        ).Produces<IEnumerable<PluginResponseDTO>>();
    }
}
