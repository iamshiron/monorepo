using System.Security.Claims;
using Shiron.Mutils.API.Services;
using Shiron.Mutils.API.DTOs;

namespace Shiron.Mutils.API.Endpoints;

public static class OptimizerEndpoints {
    public static void MapOptimizerEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/optimizer").RequireAuthorization().WithTags("Optimizer");

        group.MapPost("/analyze", async (
                ClaimsPrincipal user,
                OptimizerAnalysisRequest request,
                IOptimizerService optimizerService) => {
                    var userId = GetUserId(user);
                    if (userId is null) return Results.Unauthorized();

                    var result = await optimizerService.AnalyzeAsync(userId.Value, request);
                    return Results.Ok(result);
                })
            .Produces<OptimizerAnalysisResponse>()
            .Produces(401);

        group.MapGet("/suggest", async (
                ClaimsPrincipal user,
                IOptimizerService optimizerService) => {
                    var userId = GetUserId(user);
                    if (userId is null) return Results.Unauthorized();

                    var result = await optimizerService.GetSuggestionsAsync(userId.Value);
                    return Results.Ok(result);
                })
            .Produces<OptimizerSuggestionsResponse>()
            .Produces(401);
    }

    private static Guid? GetUserId(ClaimsPrincipal user) {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return sub is not null ? Guid.Parse(sub) : null;
    }
}
