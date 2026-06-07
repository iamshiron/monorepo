using System.Security.Claims;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static class InventoryEndpoints {
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/inventory").WithTags("Inventory");

        group.MapPost("/resonator", async (AddResonatorDTO data, ClaimsPrincipal user, ResSystemDbContext db) => {
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
                    Echos = data.Echoes.Select(e => e.ToDatabase()).ToList()
                }
            );
            await db.SaveChangesAsync();

            return Results.Created();
        });
    }

    private static Guid? GetUserID(ClaimsPrincipal principal) {
        var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return sub is not null ? Guid.Parse(sub) : null;
    }
}
