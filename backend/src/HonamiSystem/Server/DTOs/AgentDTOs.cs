using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.Server.DTOs;

public record CreateAgentRequest {
    [Required, MaxLength(256)]
    public required string Name { get; init; }

    [MaxLength(256)]
    public string? Description { get; init; }

    public Guid? PersonaId { get; init; }

    public List<string> RequiredTools { get; init; } = [];
    public List<string> SuggestedTools { get; init; } = [];
}

public record UpdateAgentRequest {
    [MaxLength(256)]
    public string? Name { get; init; }

    [MaxLength(256)]
    public string? Description { get; init; }

    public List<string>? RequiredTools { get; init; }
    public List<string>? SuggestedTools { get; init; }
}

public record SetAgentPersonaRequest {
    public Guid? PersonaId { get; init; }
}

public record AgentPersonaResponse(
    Guid ID,
    string Name,
    string? Brief,
    string? SpeakingStyle
);

public record AgentResponse(
    Guid ID,
    string Name,
    string? Description,
    AgentPersonaResponse? Persona,
    List<string> RequiredTools,
    List<string> SuggestedTools,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record AgentChatEntry(
    Guid ChatID,
    string Title,
    List<string> AllowedTools
);
