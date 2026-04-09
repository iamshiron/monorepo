using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiGit.API.DTOs;

public record ApiKeyCreateDto {
    [Required][MaxLength(64)] public required string Name { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public List<string>? Roles { get; init; }
}

public record ApiKeyUpdateDto {
    [MaxLength(64)] public string? Name { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public bool? IsRevoked { get; init; }
    public List<string>? Roles { get; init; }
}

public record ApiKeyDto {
    public Guid ID { get; init; }
    public string Name { get; init; } = default!;
    public string KeyPrefix { get; init; } = default!;
    public DateTime? ExpiresAt { get; init; }
    public bool IsRevoked { get; init; }
    public DateTime? LastUsedAt { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<string> Roles { get; init; } = [];
}

public record ApiKeyCreatedDto : ApiKeyDto {
    public string Key { get; init; } = default!;
}
