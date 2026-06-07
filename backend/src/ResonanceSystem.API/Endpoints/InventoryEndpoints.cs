using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static class InventoryEndpoints {
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/inventory").WithTags("Inventory");

        group.MapPost("/resonator", async (AddOwnedResonatorDTO data, ClaimsPrincipal user, ResSystemDbContext db) => {
            var userID = GetUserID(user);
            if (userID is null) return Results.Unauthorized();

            var resonator = db.Characters.FirstOrDefault(c => c.Id == data.ResonatorID);
            if (resonator == null) {
                return Results.BadRequest(new {
                    Message = "Resonator not found"
                });
            }

            var res = db.OwnedCharacters.Add(
                new OwnedCharacter {
                    CharacterID = resonator.Id,
                    UserID = userID.Value,
                    SequenceChain = data.SequenceChain,
                    Level = data.Level,
                    Forte0Level = data.Forte0Level,
                    Forte1Level = data.Forte1Level,
                    Forte2Level = data.Forte2Level,
                    Forte3Level = data.Forte3Level,
                    Forte4Level = data.Forte4Level,
                    Echoes = data.Echoes.Select(e => e.ToDatabase()).ToList()
                }
            );
            await db.SaveChangesAsync();

            return Results.Created();
        });

        group.MapGet("/resonators", async (ClaimsPrincipal user, ResSystemDbContext db) => {
            var userID = GetUserID(user);
            if (userID is null) return Results.Unauthorized();

            var characters = db.OwnedCharacters.Where(c => c.UserID == userID.Value)
                .Include(c => c.Echoes)
                .ThenInclude(e => e.SubStats)
                .Select(c => c.ToDTO());

            return Results.Ok(characters);
        }).Produces<IList<OwnedResonatorDTO>>();

        group.MapGet("/resonators/{id}", async (Guid id, ClaimsPrincipal user, ResSystemDbContext db) => {
            var userID = GetUserID(user);
            if (userID is null) return Results.Unauthorized();

            var character = db.OwnedCharacters
                .Include(c => c.Echoes)
                .ThenInclude(e => e.SubStats)
                .FirstOrDefault(c => c.UserID == userID.Value && c.ID == id);

            if (character is null) return Results.NotFound();

            return Results.Ok(character.ToDTO());
        }).Produces<OwnedResonatorDTO>();
    }

    private static Guid? GetUserID(ClaimsPrincipal principal) {
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return sub is not null ? Guid.Parse(sub) : null;
    }
}
