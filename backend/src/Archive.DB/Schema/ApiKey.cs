using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Shiron.TheArchive.DB.Schema;

public class ApiKey {
    public Guid ID { get; set; } = Guid.CreateVersion7();

    [MaxLength(64)] public required string Name { get; set; }

    public required Guid UserId { get; set; }
    public User User { get; set; } = null!;

    [MaxLength(128)] public required string KeyPrefix { get; set; }
    public required string KeyHash { get; set; }

    public DateTime? ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public List<ApiKeyClaim> Claims { get; set; } = [];
}

public class ApiKeyClaim {
    public Guid ID { get; set; } = Guid.CreateVersion7();
    public required Guid ApiKeyId { get; set; }
    public ApiKey ApiKey { get; set; } = null!;

    [MaxLength(256)] public required string ClaimType { get; set; }
    [MaxLength(1024)] public required string ClaimValue { get; set; }
}
