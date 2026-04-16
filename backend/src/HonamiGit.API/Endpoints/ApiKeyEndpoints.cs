using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Shiron.HonamiGit.API.DTOs;
using Shiron.HonamiGit.API.Services;

namespace Shiron.HonamiGit.API.Endpoints;

public static class ApiKeyEndpoints {
    public static void MapApiKeyEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/keys").WithTags("API Keys");

        group.MapGet("/", ListKeys)
            .WithName("ListApiKeys")
            .WithDescription("List API keys for the authenticated user")
            .RequireAuthorization()
            .Produces<List<ApiKeyDto>>()
            .Produces(401);

        group.MapPost("/", CreateKey)
            .WithName("CreateApiKey")
            .WithDescription("Create a new API key")
            .RequireAuthorization("Admin")
            .Produces<ApiKeyCreatedDto>(201)
            .Produces(401)
            .Produces(403);

        group.MapPut("/{id:guid}", UpdateKey)
            .WithName("UpdateApiKey")
            .WithDescription("Update an API key")
            .RequireAuthorization("Admin")
            .Produces<ApiKeyDto>()
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{id:guid}", DeleteKey)
            .WithName("DeleteApiKey")
            .WithDescription("Delete an API key")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> ListKeys(ClaimsPrincipal user, IApiKeyService apiKeyService) {
        var userId = GetUserId(user);
        var keys = await apiKeyService.ListAsync(userId);
        return Results.Ok(keys.Select(MapToDto).ToList());
    }

    private static async Task<IResult> CreateKey(ApiKeyCreateDto dto, ClaimsPrincipal user, IApiKeyService apiKeyService) {
        var userId = GetUserId(user);
        var (apiKey, rawKey) = await apiKeyService.CreateAsync(userId, dto.Name, dto.ExpiresAt, dto.Roles);

        var result = new ApiKeyCreatedDto {
            ID = apiKey.ID,
            Name = apiKey.Name,
            KeyPrefix = apiKey.KeyPrefix,
            ExpiresAt = apiKey.ExpiresAt,
            IsRevoked = apiKey.IsRevoked,
            LastUsedAt = apiKey.LastUsedAt,
            CreatedAt = apiKey.CreatedAt,
            UpdatedAt = apiKey.UpdatedAt,
            Key = rawKey,
            Roles = apiKey.Claims.Where(c => c.ClaimType == ClaimTypes.Role).Select(c => c.ClaimValue).ToList()
        };

        return Results.Created($"/api/keys/{apiKey.ID}", result);
    }

    private static async Task<IResult> UpdateKey(Guid id, ApiKeyUpdateDto dto, ClaimsPrincipal user, IApiKeyService apiKeyService) {
        var userId = GetUserId(user);
        var apiKey = await apiKeyService.UpdateAsync(id, userId, dto.Name, dto.ExpiresAt, dto.IsRevoked, dto.Roles);
        return apiKey == null ? Results.NotFound() : Results.Ok(MapToDto(apiKey));
    }

    private static async Task<IResult> DeleteKey(Guid id, ClaimsPrincipal user, IApiKeyService apiKeyService) {
        var userId = GetUserId(user);
        var deleted = await apiKeyService.DeleteAsync(id, userId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }

    private static Guid GetUserId(ClaimsPrincipal user) {
        var claim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? throw new UnauthorizedAccessException();
        return Guid.Parse(claim);
    }

    private static ApiKeyDto MapToDto(DB.Schema.ApiKey apiKey) {
        return new ApiKeyDto {
            ID = apiKey.ID,
            Name = apiKey.Name,
            KeyPrefix = apiKey.KeyPrefix,
            ExpiresAt = apiKey.ExpiresAt,
            IsRevoked = apiKey.IsRevoked,
            LastUsedAt = apiKey.LastUsedAt,
            CreatedAt = apiKey.CreatedAt,
            UpdatedAt = apiKey.UpdatedAt,
            Roles = apiKey.Claims.Where(c => c.ClaimType == ClaimTypes.Role).Select(c => c.ClaimValue).ToList()
        };
    }
}
