using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;
using Attribute = Shiron.ResonanceSystem.DB.Schema.Attribute;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static class ContentEndpoints {
    public static void MapContentEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/content").WithTags("Content");

        group.MapGet("/resonators", async (
            [FromQuery] string? name,
            [FromQuery] Attribute? attribute,
            [FromQuery] Rarity? rarity,
            [FromQuery] WeaponType? weapon,
            ResSystemDbContext db,
            CancellationToken ct) => {
                var query = db.Characters.AsQueryable();

                if (!string.IsNullOrWhiteSpace(name)) {
                    query = query.Where(c => c.Name.Contains(name));
                }
                if (attribute.HasValue) {
                    query = query.Where(c => c.Attribute == attribute);
                }
                if (rarity.HasValue) {
                    query = query.Where(c => c.Rarity == rarity.Value);
                }
                if (weapon.HasValue) {
                    query = query.Where(c => c.WeaponType == weapon);
                }

                var resonators = await query.Select(c => c.ToDTO()).ToListAsync(ct);
                return Results.Ok(resonators);
            }).Produces<IList<ResonatorDTO>>();

        group.MapGet("/resonators/{id}", (ulong id, ResSystemDbContext db) => {
            var c = db.Characters.FirstOrDefault(c => c.Id == id);

            if (c == null) {
                return Results.NotFound(new {
                    Message = "Resonator not found"
                });
            }

            return Results.Ok(c.ToDTO());
        }).Produces<ResonatorDTO>().Produces(404);
    }
}
