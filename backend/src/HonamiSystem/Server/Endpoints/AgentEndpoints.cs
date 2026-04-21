using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiSystem.DB;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.HonamiSystem.Server.DTOs;

namespace Shiron.HonamiSystem.Server.Endpoints;

public static class AgentEndpoints {
    public static void MapAgentEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("");

        group.MapPost("/", CreateAgent)
            .WithName("CreateAgent")
            .WithDescription("Create a new AI agent")
            .RequireAuthorization()
            .Produces<AgentResponse>()
            .Produces(400)
            .Produces(401);

        group.MapGet("/", ListAgents)
            .WithName("ListAgents")
            .WithDescription("List the current user's agents")
            .RequireAuthorization()
            .Produces<List<AgentResponse>>()
            .Produces(401);

        group.MapGet("/{agentId:guid}", GetAgent)
            .WithName("GetAgent")
            .WithDescription("Get an agent by ID")
            .RequireAuthorization()
            .Produces<AgentResponse>()
            .Produces(404)
            .Produces(401);

        group.MapPut("/{agentId:guid}", UpdateAgent)
            .WithName("UpdateAgent")
            .WithDescription("Update agent metadata")
            .RequireAuthorization()
            .Produces<AgentResponse>()
            .Produces(404)
            .Produces(401);

        group.MapPut("/{agentId:guid}/persona", SetAgentPersona)
            .WithName("SetAgentPersona")
            .WithDescription("Assign or unassign a persona to an agent")
            .RequireAuthorization()
            .Produces<AgentResponse>()
            .Produces(400)
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{agentId:guid}", DeleteAgent)
            .WithName("DeleteAgent")
            .WithDescription("Delete an agent")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);

        group.MapGet("/{agentId:guid}/chats", ListAgentChats)
            .WithName("ListAgentChats")
            .WithDescription("List all chats an agent is assigned to")
            .RequireAuthorization()
            .Produces<List<AgentChatEntry>>()
            .Produces(404)
            .Produces(401);
    }

    private static async Task<IResult> CreateAgent(
        CreateAgentRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        if (req.PersonaId.HasValue) {
            var persona = await db.Personas.FindAsync(req.PersonaId.Value);
            if (persona == null || persona.CreatedByID != user.Id)
                return Results.BadRequest(new { Message = "Persona not found" });
        }

        var agent = new Agent {
            Name = req.Name,
            Description = req.Description,
            PersonaID = req.PersonaId,
            RequiredTools = req.RequiredTools,
            SuggestedTools = req.SuggestedTools,
            CreatedByID = user.Id
        };

        if (req.PersonaId.HasValue) {
            agent.Persona = await db.Personas.FindAsync(req.PersonaId.Value);
        }

        db.Agents.Add(agent);
        await db.SaveChangesAsync();

        return Results.Ok(await MapToResponseAsync(agent, db));
    }

    private static async Task<IResult> ListAgents(
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agents = await db.Agents
            .Include(a => a.Persona)
            .Where(a => a.CreatedByID == user.Id)
            .OrderByDescending(a => a.UpdatedAt)
            .ToListAsync();

        var responses = new List<AgentResponse>();
        foreach (var agent in agents) {
            responses.Add(await MapToResponseAsync(agent, db));
        }

        return Results.Ok(responses);
    }

    private static async Task<IResult> GetAgent(
        Guid agentId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agent = await db.Agents
            .Include(a => a.Persona)
            .FirstOrDefaultAsync(a => a.ID == agentId);

        if (agent == null || agent.CreatedByID != user.Id)
            return Results.NotFound();

        return Results.Ok(await MapToResponseAsync(agent, db));
    }

    private static async Task<IResult> UpdateAgent(
        Guid agentId,
        UpdateAgentRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agent = await db.Agents
            .Include(a => a.Persona)
            .FirstOrDefaultAsync(a => a.ID == agentId);

        if (agent == null || agent.CreatedByID != user.Id)
            return Results.NotFound();

        if (req.Name is not null) agent.Name = req.Name;
        if (req.Description is not null) agent.Description = req.Description;
        if (req.RequiredTools is not null) agent.RequiredTools = req.RequiredTools;
        if (req.SuggestedTools is not null) agent.SuggestedTools = req.SuggestedTools;
        agent.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok(await MapToResponseAsync(agent, db));
    }

    private static async Task<IResult> SetAgentPersona(
        Guid agentId,
        SetAgentPersonaRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agent = await db.Agents
            .Include(a => a.Persona)
            .FirstOrDefaultAsync(a => a.ID == agentId);

        if (agent == null || agent.CreatedByID != user.Id)
            return Results.NotFound();

        if (req.PersonaId.HasValue) {
            var persona = await db.Personas.FindAsync(req.PersonaId.Value);
            if (persona == null || persona.CreatedByID != user.Id)
                return Results.BadRequest(new { Message = "Persona not found" });
            agent.Persona = persona;
            agent.PersonaID = req.PersonaId.Value;
        } else {
            agent.Persona = null;
            agent.PersonaID = null;
        }

        agent.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(await MapToResponseAsync(agent, db));
    }

    private static async Task<IResult> DeleteAgent(
        Guid agentId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agent = await db.Agents.FindAsync(agentId);
        if (agent == null || agent.CreatedByID != user.Id)
            return Results.NotFound();

        db.Agents.Remove(agent);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Agent deleted successfully" });
    }

    private static async Task<IResult> ListAgentChats(
        Guid agentId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var agent = await db.Agents.FindAsync(agentId);
        if (agent == null || agent.CreatedByID != user.Id)
            return Results.NotFound();

        var chatEntries = await db.ChatParticipantAgents
            .Include(p => p.Chat)
            .Where(p => p.AgentID == agentId)
            .Select(p => new AgentChatEntry(
                p.ChatID,
                p.Chat.Title,
                p.AllowedTools.ToList()
            ))
            .ToListAsync();

        return Results.Ok(chatEntries);
    }

    private static async Task<AgentResponse> MapToResponseAsync(Agent agent, HonamiSystemDb db) {
        AgentPersonaResponse? personaResponse = null;
        if (agent.PersonaID.HasValue && agent.Persona is not null) {
            personaResponse = new AgentPersonaResponse(
                agent.Persona.ID,
                agent.Persona.Name,
                agent.Persona.Brief,
                agent.Persona.SpeakingStyle
            );
        }

        return new AgentResponse(
            agent.ID,
            agent.Name,
            agent.Description,
            personaResponse,
            agent.RequiredTools.ToList(),
            agent.SuggestedTools.ToList(),
            agent.CreatedAt,
            agent.UpdatedAt
        );
    }
}
