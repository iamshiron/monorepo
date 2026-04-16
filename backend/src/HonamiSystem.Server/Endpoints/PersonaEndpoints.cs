using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiSystem.DB;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.HonamiSystem.Server.DTOs;

namespace Shiron.HonamiSystem.Server.Endpoints;

public static class PersonaEndpoints {
    public static void MapPersonaEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("");

        group.MapPost("/", CreatePersona)
            .WithName("CreatePersona")
            .WithDescription("Create a new persona")
            .RequireAuthorization()
            .Produces<PersonaResponse>()
            .Produces(400)
            .Produces(401);

        group.MapGet("/", ListPersonas)
            .WithName("ListPersonas")
            .WithDescription("List the current user's personas")
            .RequireAuthorization()
            .Produces<List<PersonaResponse>>()
            .Produces(401);

        group.MapGet("/{personaId:guid}", GetPersona)
            .WithName("GetPersona")
            .WithDescription("Get a persona by ID")
            .RequireAuthorization()
            .Produces<PersonaResponse>()
            .Produces(404)
            .Produces(401);

        group.MapPut("/{personaId:guid}", UpdatePersona)
            .WithName("UpdatePersona")
            .WithDescription("Update persona metadata")
            .RequireAuthorization()
            .Produces<PersonaResponse>()
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{personaId:guid}", DeletePersona)
            .WithName("DeletePersona")
            .WithDescription("Delete a persona")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);
    }

    private static async Task<IResult> CreatePersona(
        CreatePersonaRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var persona = new Persona {
            Name = req.Name,
            Brief = req.Brief,
            Instruction = req.Instruction,
            Traits = req.Traits,
            SpeakingStyle = req.SpeakingStyle,
            CreatedByID = user.Id,
        };

        db.Personas.Add(persona);
        await db.SaveChangesAsync();

        return Results.Ok(MapToResponse(persona));
    }

    private static async Task<IResult> ListPersonas(
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var personas = await db.Personas
            .Where(p => p.CreatedByID == user.Id)
            .OrderByDescending(p => p.UpdatedAt)
            .Select(p => MapToResponse(p))
            .ToListAsync();

        return Results.Ok(personas);
    }

    private static async Task<IResult> GetPersona(
        Guid personaId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var persona = await db.Personas.FindAsync(personaId);
        if (persona == null || persona.CreatedByID != user.Id)
            return Results.NotFound();

        return Results.Ok(MapToResponse(persona));
    }

    private static async Task<IResult> UpdatePersona(
        Guid personaId,
        UpdatePersonaRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var persona = await db.Personas.FindAsync(personaId);
        if (persona == null || persona.CreatedByID != user.Id)
            return Results.NotFound();

        if (req.Name is not null) persona.Name = req.Name;
        if (req.Brief is not null) persona.Brief = req.Brief;
        if (req.Instruction is not null) persona.Instruction = req.Instruction;
        if (req.Traits is not null) persona.Traits = req.Traits;
        if (req.SpeakingStyle is not null) persona.SpeakingStyle = req.SpeakingStyle;
        persona.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok(MapToResponse(persona));
    }

    private static async Task<IResult> DeletePersona(
        Guid personaId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var persona = await db.Personas.FindAsync(personaId);
        if (persona == null || persona.CreatedByID != user.Id)
            return Results.NotFound();

        db.Personas.Remove(persona);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Persona deleted successfully" });
    }

    private static PersonaResponse MapToResponse(Persona p) => new(
        p.ID, p.Name, p.Brief, p.Instruction,
        p.Traits.ToList(), p.SpeakingStyle,
        p.CreatedAt, p.UpdatedAt
    );
}
