using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.Mutils.Core.DTOs;
using Shiron.Mutils.Infrastructure.Data;
using Shiron.Mutils.Core.Entities;

namespace Shiron.Mutils.Api.Endpoints;

public static class SphereEndpoints {
    public static void MapSphereEndpoints(this IEndpointRouteBuilder app) {
        var group = app.MapGroup("/api/collection").RequireAuthorization().WithTags("Collection");

        group.MapGet("/{id}/spheres", async (Guid id, ClaimsPrincipal user, MutilsDbContext db) => {
            var userId = GetUserId(user);
            if (userId is null) return Results.Unauthorized();

            var entry = await db.CollectionEntries
                .Include(e => e.SpherePerks)
                .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

            if (entry is null) return Results.NotFound();
            if (entry.SpherePerks is null) return Results.Ok(CollectionEntrySpherePerks.Empty);

            return Results.Ok(new CollectionEntrySpherePerks(
                entry.SpherePerks.Perk1,
                entry.SpherePerks.Perk2,
                entry.SpherePerks.Perk3,
                entry.SpherePerks.Perk4,
                entry.SpherePerks.Perk5,
                entry.SpherePerks.Perk6,
                entry.SpherePerks.Perk7,
                entry.SpherePerks.Perk8,
                entry.SpherePerks.Perk9,
                entry.SpherePerks.Perk10
            ));
        });

        group.MapPost("/{id}/spheres", async (
            Guid id,
            ClaimsPrincipal user,
            CollectionEntrySpherePerks perks,
            MutilsDbContext db) => {
                var userId = GetUserId(user);
                if (userId is null) return Results.Unauthorized();

                var entry = await db.CollectionEntries
                    .Include(e => e.SpherePerks)
                    .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId);

                if (entry is null) return Results.NotFound();
                if (entry.SpherePerks is null) {
                    entry.SpherePerks = new SpherePerks {
                        Perk1 = perks.Perk1,
                        Perk2 = perks.Perk2,
                        Perk3 = perks.Perk3,
                        Perk4 = perks.Perk4,
                        Perk5 = perks.Perk5,
                        Perk6 = perks.Perk6,
                        Perk7 = perks.Perk7,
                        Perk8 = perks.Perk8,
                        Perk9 = perks.Perk9,
                        Perk10 = perks.Perk10
                    };
                } else {
                    db.Entry(entry.SpherePerks).CurrentValues.SetValues(perks);
                }

                await db.SaveChangesAsync();

                return Results.Ok(new { message = "Updated successfully" });
            });
    }

    private static Guid? GetUserId(ClaimsPrincipal user) {
        var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return sub is not null ? Guid.Parse(sub) : null;
    }
}
