using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiGit.API.DTOs;
using Shiron.HonamiGit.DB;
using Shiron.HonamiGit.DB.Schema;

namespace Shiron.HonamiGit.API.Endpoints;

public static class SshKeyEndpoints {
    public static void MapKeyEndpoints(this IEndpointRouteBuilder endpoints) {
        var route = endpoints.MapGroup("/sshkeys").WithTags("SSHKeys");

        route.MapGet("/", ListKeys)
            .WithName("ListSSHKeys")
            .WithDescription("List SSH keys for the authenticated user")
            .RequireAuthorization()
            .Produces<IList<ResponseSSHKeyDTO>>()
            .Produces(401);

        route.MapPost("/", CreateKey)
            .WithName("CreateSSHKey")
            .WithDescription("Add a new SSH public key")
            .RequireAuthorization()
            .Produces<ResponseSSHKeyDTO>(201)
            .Produces(401)
            .ProducesValidationProblem();

        route.MapPut("/{keyID:guid}", UpdateKey)
            .WithName("UpdateSSHKey")
            .WithDescription("Update the name or description of an SSH key")
            .RequireAuthorization()
            .Produces<ResponseSSHKeyDTO>()
            .Produces(401)
            .Produces(404);

        route.MapDelete("/{keyID:guid}", DeleteKey)
            .WithName("DeleteSSHKey")
            .WithDescription("Delete an SSH key by ID")
            .RequireAuthorization()
            .Produces(204)
            .Produces(401)
            .Produces(404);
    }

    private static async Task<IResult> ListKeys(ClaimsPrincipal user, HonamiGitDb db) {
        var userId = GetUserId(user);
        var keys = await db.UserSSHKeys
            .Where(k => k.UserID == userId)
            .OrderByDescending(k => k.CreatedAt)
            .Select(k => MapToDto(k))
            .ToListAsync();
        return Results.Ok(keys);
    }

    private static async Task<IResult> CreateKey(RequestCreateSshKeyDTO dto, ClaimsPrincipal user, HonamiGitDb db) {
        var userId = GetUserId(user);

        if (string.IsNullOrWhiteSpace(dto.Key))
            return Results.ValidationProblem(new Dictionary<string, string[]> {
                { "Key", ["SSH public key is required."] }
            });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return Results.ValidationProblem(new Dictionary<string, string[]> {
                { "Name", ["Key name is required."] }
            });

        var sshKey = new UserSSHKey {
            Name = dto.Name,
            Key = dto.Key.Trim(),
            ExpiresAt = dto.ExpiresAt,
            UserID = userId,
            User = null!
        };

        db.UserSSHKeys.Add(sshKey);
        await db.SaveChangesAsync();

        return Results.Created($"/sshkeys/{sshKey.ID}", MapToDto(sshKey));
    }

    private static async Task<IResult> UpdateKey(Guid keyID, RequestUpdateSshKeyDTO dto, ClaimsPrincipal user, HonamiGitDb db) {
        var userId = GetUserId(user);
        var key = await db.UserSSHKeys.FirstOrDefaultAsync(k => k.ID == keyID && k.UserID == userId);
        if (key == null) return Results.NotFound();

        if (dto.Name is not null) key.Name = dto.Name;
        if (dto.Description is not null) key.Description = dto.Description;
        key.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();
        return Results.Ok(MapToDto(key));
    }

    private static async Task<IResult> DeleteKey(Guid keyID, ClaimsPrincipal user, HonamiGitDb db) {
        var userId = GetUserId(user);
        var key = await db.UserSSHKeys.FirstOrDefaultAsync(k => k.ID == keyID && k.UserID == userId);
        if (key == null) return Results.NotFound();

        db.UserSSHKeys.Remove(key);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static Guid GetUserId(ClaimsPrincipal user) {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();
        return Guid.Parse(claim);
    }

    private static ResponseSSHKeyDTO MapToDto(UserSSHKey key) {
        return new ResponseSSHKeyDTO(
            key.ID,
            key.Name,
            key.Description,
            key.CreatedAt,
            key.ExpiresAt
        );
    }
}
