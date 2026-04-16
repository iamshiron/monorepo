using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiSystem.DB;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.HonamiSystem.Server.DTOs;

namespace Shiron.HonamiSystem.Server.Endpoints;

public static class ChatEndpoints {
    public static void MapChatEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("");

        group.MapPost("/", CreateChat)
            .WithName("CreateChat")
            .WithDescription("Create a new chat")
            .RequireAuthorization()
            .Produces<ChatResponse>()
            .Produces(400)
            .Produces(401);

        group.MapGet("/", ListChats)
            .WithName("ListChats")
            .WithDescription("List the current user's chats")
            .RequireAuthorization()
            .Produces<List<ChatResponse>>()
            .Produces(401);

        group.MapGet("/{chatId:guid}", GetChat)
            .WithName("GetChat")
            .WithDescription("Get a chat by ID")
            .RequireAuthorization()
            .Produces<ChatDetailResponse>()
            .Produces(404)
            .Produces(401);

        group.MapPut("/{chatId:guid}", UpdateChat)
            .WithName("UpdateChat")
            .WithDescription("Update chat metadata")
            .RequireAuthorization()
            .Produces<ChatResponse>()
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{chatId:guid}", DeleteChat)
            .WithName("DeleteChat")
            .WithDescription("Delete a chat")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);

        group.MapPut("/{chatId:guid}/group", SetChatGroup)
            .WithName("SetChatGroup")
            .WithDescription("Assign or unassign a chat to a group")
            .RequireAuthorization()
            .Produces<ChatResponse>()
            .Produces(400)
            .Produces(404)
            .Produces(401);

        group.MapGet("/{chatId:guid}/participants", GetParticipants)
            .WithName("GetChatParticipants")
            .WithDescription("List all participants in a chat")
            .RequireAuthorization()
            .Produces<ChatParticipantsResponse>()
            .Produces(404)
            .Produces(401);

        group.MapPost("/{chatId:guid}/participants/user", AddUserParticipant)
            .WithName("AddChatUserParticipant")
            .WithDescription("Add a user as a participant to a chat")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(401);

        group.MapPost("/{chatId:guid}/participants/agent", AddAgentParticipant)
            .WithName("AddChatAgentParticipant")
            .WithDescription("Add an agent as a participant to a chat")
            .RequireAuthorization()
            .Produces(200)
            .Produces(400)
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{chatId:guid}/participants/user/{userId:guid}", RemoveUserParticipant)
            .WithName("RemoveChatUserParticipant")
            .WithDescription("Remove a user participant from a chat")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{chatId:guid}/participants/agent/{agentId:guid}", RemoveAgentParticipant)
            .WithName("RemoveChatAgentParticipant")
            .WithDescription("Remove an agent participant from a chat")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);
    }

    private static async Task<IResult> CreateChat(
        CreateChatRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        if (req.GroupId.HasValue) {
            var group = await db.ChatGroups.FindAsync(req.GroupId.Value);
            if (group == null || group.CreatedByID != user.Id)
                return Results.BadRequest(new { Message = "Group not found" });
        }

        var chat = new Chat {
            Title = req.Title,
            Description = req.Description,
            ChatGroupID = req.GroupId,
            CreatedByID = user.Id,
        };

        chat.UserParticipants.Add(new ChatParticipantUser {
            UserID = user.Id,
            User = user,
            ChatID = chat.ID,
            Chat = chat,
        });

        db.Chats.Add(chat);
        await db.SaveChangesAsync();

        return Results.Ok(new ChatResponse(
            chat.ID, chat.Title, chat.Description,
            chat.ChatGroupID, chat.CreatedAt, chat.UpdatedAt
        ));
    }

    private static async Task<IResult> ListChats(
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chats = await db.Chats
            .Where(c => c.CreatedByID == user.Id
                || c.UserParticipants.Any(p => p.UserID == user.Id))
            .OrderByDescending(c => c.UpdatedAt)
            .Select(c => new ChatResponse(
                c.ID, c.Title, c.Description,
                c.ChatGroupID, c.CreatedAt, c.UpdatedAt
            ))
            .ToListAsync();

        return Results.Ok(chats);
    }

    private static async Task<IResult> GetChat(
        Guid chatId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats
            .Include(c => c.UserParticipants).ThenInclude(p => p.User)
            .Include(c => c.AgentParticipants).ThenInclude(p => p.Agent)
            .FirstOrDefaultAsync(c => c.ID == chatId);

        if (chat == null) return Results.NotFound();
        if (chat.CreatedByID != user.Id && !chat.UserParticipants.Any(p => p.UserID == user.Id))
            return Results.NotFound();

        return Results.Ok(MapToDetailResponse(chat));
    }

    private static async Task<IResult> UpdateChat(
        Guid chatId,
        UpdateChatRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        if (req.Title is not null) chat.Title = req.Title;
        if (req.Description is not null) chat.Description = req.Description;
        chat.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok(new ChatResponse(
            chat.ID, chat.Title, chat.Description,
            chat.ChatGroupID, chat.CreatedAt, chat.UpdatedAt
        ));
    }

    private static async Task<IResult> DeleteChat(
        Guid chatId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        db.Chats.Remove(chat);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Chat deleted successfully" });
    }

    private static async Task<IResult> SetChatGroup(
        Guid chatId,
        SetChatGroupRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        if (req.GroupId.HasValue) {
            var group = await db.ChatGroups.FindAsync(req.GroupId.Value);
            if (group == null || group.CreatedByID != user.Id)
                return Results.BadRequest(new { Message = "Group not found" });
        }

        chat.ChatGroupID = req.GroupId;
        chat.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new ChatResponse(
            chat.ID, chat.Title, chat.Description,
            chat.ChatGroupID, chat.CreatedAt, chat.UpdatedAt
        ));
    }

    private static async Task<IResult> GetParticipants(
        Guid chatId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats
            .Include(c => c.UserParticipants).ThenInclude(p => p.User)
            .Include(c => c.AgentParticipants).ThenInclude(p => p.Agent)
            .FirstOrDefaultAsync(c => c.ID == chatId);

        if (chat == null) return Results.NotFound();
        if (chat.CreatedByID != user.Id && !chat.UserParticipants.Any(p => p.UserID == user.Id))
            return Results.NotFound();

        return Results.Ok(new ChatParticipantsResponse(
            chat.UserParticipants.Select(p => new ParticipantUserResponse(p.UserID, p.User.Name)).ToList(),
            chat.AgentParticipants.Select(p => new ParticipantAgentResponse(p.AgentID, p.Agent.Name, p.AllowedTools.ToList())).ToList()
        ));
    }

    private static async Task<IResult> AddUserParticipant(
        Guid chatId,
        AddUserParticipantRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        var targetUser = await userManager.FindByIdAsync(req.UserId.ToString());
        if (targetUser == null) return Results.BadRequest(new { Message = "User not found" });

        var exists = await db.ChatParticipants.AnyAsync(p => p.UserID == req.UserId && p.ChatID == chatId);
        if (exists) return Results.BadRequest(new { Message = "User is already a participant" });

        db.ChatParticipants.Add(new ChatParticipantUser {
            UserID = req.UserId,
            User = targetUser,
            ChatID = chatId,
            Chat = chat,
        });
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "User added as participant" });
    }

    private static async Task<IResult> AddAgentParticipant(
        Guid chatId,
        AddAgentParticipantRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        var agent = await db.Agents.FindAsync(req.AgentId);
        if (agent == null) return Results.BadRequest(new { Message = "Agent not found" });

        var exists = await db.ChatParticipantAgents.AnyAsync(p => p.AgentID == req.AgentId && p.ChatID == chatId);
        if (exists) return Results.BadRequest(new { Message = "Agent is already a participant" });

        db.ChatParticipantAgents.Add(new ChatParticipantAgent {
            AgentID = req.AgentId,
            Agent = agent,
            ChatID = chatId,
            Chat = chat,
            AllowedTools = req.AllowedTools,
        });
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Agent added as participant" });
    }

    private static async Task<IResult> RemoveUserParticipant(
        Guid chatId,
        Guid userId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        var participant = await db.ChatParticipants.FirstOrDefaultAsync(p => p.UserID == userId && p.ChatID == chatId);
        if (participant == null) return Results.NotFound();

        db.ChatParticipants.Remove(participant);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "User removed from chat" });
    }

    private static async Task<IResult> RemoveAgentParticipant(
        Guid chatId,
        Guid agentId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chat = await db.Chats.FindAsync(chatId);
        if (chat == null || chat.CreatedByID != user.Id) return Results.NotFound();

        var participant = await db.ChatParticipantAgents.FirstOrDefaultAsync(p => p.AgentID == agentId && p.ChatID == chatId);
        if (participant == null) return Results.NotFound();

        db.ChatParticipantAgents.Remove(participant);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Agent removed from chat" });
    }

    private static ChatDetailResponse MapToDetailResponse(Chat chat) => new(
        chat.ID, chat.Title, chat.Description,
        chat.ChatGroupID, chat.CreatedAt, chat.UpdatedAt,
        chat.UserParticipants.Select(p => new ParticipantUserResponse(p.UserID, p.User.Name)).ToList(),
        chat.AgentParticipants.Select(p => new ParticipantAgentResponse(p.AgentID, p.Agent.Name, p.AllowedTools.ToList())).ToList()
    );
}
