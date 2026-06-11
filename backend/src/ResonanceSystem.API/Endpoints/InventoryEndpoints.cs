using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static class InventoryEndpoints {
    public static void MapInventoryEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/inventory").WithTags("Inventory");

        group.MapPost("/resonator", AddResonator);

        group.MapGet("/resonators", GetResonators)
            .Produces<IList<OwnedResonatorDTO>>();

        group.MapGet("/resonators/{id}", GetResonator)
            .Produces<OwnedResonatorDTO>();

        group.MapPost("/resonators/{id}/echoes", UpdateEchoes);
    }

    private static async Task<IResult> AddResonator(
        AddOwnedResonatorDTO data,
        ClaimsPrincipal user,
        ResSystemDbContext db) {
        var userID = IdentityUtils.GetUserID(user);
        if (userID is null) return Results.Unauthorized();

        var resonator = db.Characters.FirstOrDefault(c => c.Id == data.ResonatorID);
        if (resonator == null) {
            return Results.BadRequest(new {
                Message = "Resonator not found"
            });
        }

        db.CharacterInstances.Add(
            new CharacterInstance {
                CharacterID = resonator.Id,
                UserID = userID.Value,
                SequenceChain = data.SequenceChain,
                Level = data.Level,
                Forte0Level = data.Forte0Level,
                Forte1Level = data.Forte1Level,
                Forte2Level = data.Forte2Level,
                Forte3Level = data.Forte3Level,
                Forte4Level = data.Forte4Level,
                EchoInstances = data.Echoes.Select(e => e.ToDatabase()).ToList()
            }
        );
        await db.SaveChangesAsync();

        return Results.Created();
    }

    private static async Task<IResult> GetResonators(ClaimsPrincipal user, ResSystemDbContext db) {
        var userID = IdentityUtils.GetUserID(user);
        if (userID is null) return Results.Unauthorized();

        var characters = db.CharacterInstances.Where(c => c.UserID == userID.Value)
            .Include(c => c.Character)
            .Include(c => c.EchoInstances)
            .ThenInclude(e => e.SubStats)
            .Select(c => c.ToDTO());

        return Results.Ok(characters);
    }

    private static async Task<IResult> GetResonator(Guid id, ClaimsPrincipal user, ResSystemDbContext db) {
        var userID = IdentityUtils.GetUserID(user);
        if (userID is null) return Results.Unauthorized();

        var character = db.CharacterInstances
            .Include(c => c.Character)
            .Include(c => c.EchoInstances)
            .ThenInclude(e => e.SubStats)
            .FirstOrDefault(c => c.UserID == userID.Value && c.ID == id);

        if (character is null) return Results.NotFound();

        return Results.Ok(character.ToDTO());
    }

    private static async Task<IResult> UpdateEchoes(
        Guid id,
        IList<AddEchoDTO> data,
        ClaimsPrincipal user,
        ResSystemDbContext db) {
        var userID = IdentityUtils.GetUserID(user);
        if (userID is null) return Results.Unauthorized();

        var character = db.CharacterInstances
            .Include(c => c.EchoInstances)
            .ThenInclude(e => e.SubStats)
            .FirstOrDefault(c => c.UserID == userID.Value && c.ID == id);
        if (character is null) return Results.NotFound();

        character.EchoInstances.Clear();
        foreach (var dto in data) {
            var newEcho = dto.ToDatabase();
            newEcho.ID = Guid.Empty;

            foreach (var subStat in newEcho.SubStats) {
                subStat.ID = Guid.Empty;
            }

            character.EchoInstances.Add(newEcho);
        }

        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
