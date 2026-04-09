using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.Mutils.API.DTOs;
using Shiron.Mutils.API.DTos.DB;

namespace Shiron.Mutils.API.Endpoints;

public static class UserEndpoints {
    public static void MapUserEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/user").RequireAuthorization().WithTags("User");

        group.MapGet("/me", async (
                ClaimsPrincipal user,
                MutilsDbContext db) => {
                    var userId = GetUserId(user);
                    if (userId is null) return Results.Unauthorized();

                    var dbUser = await db.Users.FindAsync(userId);
                    if (dbUser is null) return Results.NotFound();

                    return Results.Ok(new UserDto(
                        dbUser.Id,
                        dbUser.DiscordId,
                        dbUser.Username,
                        dbUser.AvatarUrl
                    ));
                })
            .Produces<UserDto>()
            .Produces(401)
            .Produces(404);
    }

    private static Guid? GetUserId(ClaimsPrincipal user) {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return sub is not null ? Guid.Parse(sub) : null;
    }
}
