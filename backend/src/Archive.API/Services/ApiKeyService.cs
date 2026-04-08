using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB;
using Shiron.TheArchive.DB.Schema;

namespace Shiron.TheArchive.API.Services;

public interface IApiKeyService {
    Task<(ApiKey ApiKey, string RawKey)> CreateAsync(Guid userId, string name, DateTime? expiresAt, List<string>? roles);
    Task<ApiKey?> ValidateAsync(string rawKey);
    Task<List<ApiKey>> ListAsync(Guid userId);
    Task<ApiKey?> UpdateAsync(Guid id, Guid userId, string? name, DateTime? expiresAt, bool? isRevoked, List<string>? roles);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(ApiKey apiKey);
}

public class ApiKeyService(ArchiveDbContext db) : IApiKeyService {
    private const string KeyPrefix = "tar_live_";
    private const int KeyBytes = 32;

    public async Task<(ApiKey ApiKey, string RawKey)> CreateAsync(Guid userId, string name, DateTime? expiresAt, List<string>? roles) {
        var rawKey = GenerateRawKey();
        var hash = HashKey(rawKey);
        var prefix = rawKey[..Math.Min(KeyPrefix.Length + 8, rawKey.Length)];

        var apiKey = new ApiKey {
            Name = name,
            UserId = userId,
            KeyPrefix = prefix,
            KeyHash = hash,
            ExpiresAt = expiresAt,
            Claims = []
        };

        if (roles is { Count: > 0 }) {
            foreach (var role in roles) {
                apiKey.Claims.Add(new ApiKeyClaim {
                    ApiKeyId = apiKey.ID,
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = role
                });
            }
        }

        db.ApiKeys.Add(apiKey);
        await db.SaveChangesAsync();
        return (apiKey, rawKey);
    }

    public async Task<ApiKey?> ValidateAsync(string rawKey) {
        var hash = HashKey(rawKey);
        var prefix = rawKey[..Math.Min(KeyPrefix.Length + 8, rawKey.Length)];

        var apiKey = await db.ApiKeys
            .Include(a => a.Claims)
            .FirstOrDefaultAsync(a => a.KeyPrefix == prefix && a.KeyHash == hash);

        if (apiKey == null) return null;
        if (apiKey.IsRevoked) return null;
        if (apiKey.ExpiresAt.HasValue && apiKey.ExpiresAt.Value < DateTime.UtcNow) return null;

        apiKey.LastUsedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return apiKey;
    }

    public async Task<List<ApiKey>> ListAsync(Guid userId) {
        return await db.ApiKeys
            .Include(a => a.Claims)
            .Where(a => a.UserId == userId)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
    }

    public async Task<ApiKey?> UpdateAsync(Guid id, Guid userId, string? name, DateTime? expiresAt, bool? isRevoked, List<string>? roles) {
        var apiKey = await db.ApiKeys.Include(a => a.Claims).FirstOrDefaultAsync(a => a.ID == id && a.UserId == userId);
        if (apiKey == null) return null;

        if (name is not null) apiKey.Name = name;
        if (expiresAt.HasValue) apiKey.ExpiresAt = expiresAt;
        if (isRevoked.HasValue) apiKey.IsRevoked = isRevoked.Value;

        if (roles is not null) {
            var existingRoleClaims = apiKey.Claims.Where(c => c.ClaimType == ClaimTypes.Role).ToList();
            foreach (var claim in existingRoleClaims) {
                db.ApiKeyClaims.Remove(claim);
            }
            foreach (var role in roles) {
                apiKey.Claims.Add(new ApiKeyClaim {
                    ApiKeyId = apiKey.ID,
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = role
                });
            }
        }

        apiKey.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return apiKey;
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId) {
        var apiKey = await db.ApiKeys.FirstOrDefaultAsync(a => a.ID == id && a.UserId == userId);
        if (apiKey == null) return false;

        db.ApiKeys.Remove(apiKey);
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<ClaimsPrincipal> BuildClaimsPrincipalAsync(ApiKey apiKey) {
        var user = await db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == apiKey.UserId);
        if (user == null) return new ClaimsPrincipal(new ClaimsIdentity());

        var userManagerClaims = await db.UserClaims
            .Where(c => c.UserId == user.Id)
            .Select(c => new Claim(c.ClaimType!, c.ClaimValue!))
            .ToListAsync();

        var userRoles = await db.UserRoles
            .Where(ur => ur.UserId == user.Id)
            .Join(db.Roles, ur => ur.RoleId, r => r.Id, (_, r) => r.Name!)
            .ToListAsync();

        var claims = new List<Claim> {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? ""),
            new(ClaimTypes.Email, user.Email ?? ""),
            new("ApiKeyId", apiKey.ID.ToString()),
            new("AuthMethod", "ApiKey")
        };

        claims.AddRange(userManagerClaims);

        foreach (var role in userRoles) {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        foreach (var apiKeyClaim in apiKey.Claims) {
            claims.Add(new Claim(apiKeyClaim.ClaimType, apiKeyClaim.ClaimValue));
        }

        var identity = new ClaimsIdentity(claims, "ApiKey", ClaimTypes.Name, ClaimTypes.Role);
        return new ClaimsPrincipal(identity);
    }

    private static string GenerateRawKey() {
        return KeyPrefix + Convert.ToBase64String(RandomNumberGenerator.GetBytes(KeyBytes))
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    private static string HashKey(string rawKey) {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawKey));
        return Convert.ToBase64String(bytes);
    }
}
