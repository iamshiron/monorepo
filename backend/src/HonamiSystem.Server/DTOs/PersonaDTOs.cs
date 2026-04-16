using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.Server.DTOs;

public record CreatePersonaRequest {
    [Required, MaxLength(64)]
    public required string Name { get; init; }

    [MaxLength(256)]
    public string? Brief { get; init; }

    [MaxLength(8192)]
    public string Instruction { get; init; } = "";

    public List<string> Traits { get; init; } = [];

    [MaxLength(512)]
    public string? SpeakingStyle { get; init; }
}

public record UpdatePersonaRequest {
    [MaxLength(64)]
    public string? Name { get; init; }

    [MaxLength(256)]
    public string? Brief { get; init; }

    [MaxLength(8192)]
    public string? Instruction { get; init; }

    public List<string>? Traits { get; init; }

    [MaxLength(512)]
    public string? SpeakingStyle { get; init; }
}

public record PersonaResponse(
    Guid ID,
    string Name,
    string? Brief,
    string Instruction,
    List<string> Traits,
    string? SpeakingStyle,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
