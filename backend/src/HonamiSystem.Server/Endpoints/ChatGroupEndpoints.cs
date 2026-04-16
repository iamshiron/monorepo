using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiSystem.DB;
using Shiron.HonamiSystem.DB.Schema;
using Shiron.HonamiSystem.Server.DTOs;

namespace Shiron.HonamiSystem.Server.Endpoints;

public static class ChatGroupEndpoints {
    public static void MapChatGroupEndpoints(this IEndpointRouteBuilder routes) {
        var group = routes.MapGroup("/groups");

        group.MapGet("/", ListGroups)
            .WithName("ListChatGroups")
            .WithDescription("List the current user's chat groups")
            .RequireAuthorization()
            .Produces<List<ChatGroupResponse>>()
            .Produces(401);

        group.MapPost("/", CreateGroup)
            .WithName("CreateChatGroup")
            .WithDescription("Create a new chat group")
            .RequireAuthorization()
            .Produces<ChatGroupResponse>()
            .Produces(401);

        group.MapPut("/{groupId:guid}", UpdateGroup)
            .WithName("UpdateChatGroup")
            .WithDescription("Update a chat group name")
            .RequireAuthorization()
            .Produces<ChatGroupResponse>()
            .Produces(404)
            .Produces(401);

        group.MapDelete("/{groupId:guid}", DeleteGroup)
            .WithName("DeleteChatGroup")
            .WithDescription("Delete a chat group")
            .RequireAuthorization()
            .Produces(200)
            .Produces(404)
            .Produces(401);
    }

    private static async Task<IResult> ListGroups(
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var groups = await db.ChatGroups
            .Include(g => g.Chats)
            .Where(g => g.CreatedByID == user.Id)
            .OrderByDescending(g => g.UpdatedAt)
            .Select(g => new ChatGroupResponse(
                g.ID, g.Name, g.CreatedAt, g.UpdatedAt,
                g.Chats.Select(c => new ChatGroupChatEntry(c.ID, c.Title)).ToList()
            ))
            .ToListAsync();

        return Results.Ok(groups);
    }

    private static async Task<IResult> CreateGroup(
        CreateChatGroupRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var chatGroup = new ChatGroup {
            Name = req.Name,
            CreatedByID = user.Id
        };

        db.ChatGroups.Add(chatGroup);
        await db.SaveChangesAsync();

        return Results.Ok(new ChatGroupResponse(chatGroup.ID, chatGroup.Name, chatGroup.CreatedAt, chatGroup.UpdatedAt, []));
    }

    private static async Task<IResult> UpdateGroup(
        Guid groupId,
        UpdateChatGroupRequest req,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var group = await db.ChatGroups.FindAsync(groupId);
        if (group == null || group.CreatedByID != user.Id) return Results.NotFound();

        group.Name = req.Name;
        group.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new ChatGroupResponse(group.ID, group.Name, group.CreatedAt, group.UpdatedAt, []));
    }

    private static async Task<IResult> DeleteGroup(
        Guid groupId,
        ClaimsPrincipal principal,
        UserManager<User> userManager,
        HonamiSystemDb db) {
        var user = await userManager.GetUserAsync(principal);
        if (user == null) return Results.Unauthorized();

        var group = await db.ChatGroups.FindAsync(groupId);
        if (group == null || group.CreatedByID != user.Id) return Results.NotFound();

        db.ChatGroups.Remove(group);
        await db.SaveChangesAsync();

        return Results.Ok(new { Message = "Chat group deleted successfully" });
    }
}
