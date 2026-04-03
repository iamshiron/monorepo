namespace Shiron.HonamiGit.API.Endpoints;

public static class RepositoryEndpoints {
    public static void MapRepositoryEndpoints(this IEndpointRouteBuilder app) {
        var router = app.MapGroup("/repositories").WithTags("Repositories").WithDisplayName("Repository Management");
    }
}
