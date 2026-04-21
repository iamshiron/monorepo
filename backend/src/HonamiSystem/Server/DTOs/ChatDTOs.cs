using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.Server.DTOs;

public record CreateChatRequest {
    [Required, MaxLength(64)]
    public required string Title { get; init; }

    [MaxLength(256)]
    public string? Description { get; init; }

    public Guid? GroupId { get; init; }
}

public record UpdateChatRequest {
    [MaxLength(64)]
    public string? Title { get; init; }

    [MaxLength(256)]
    public string? Description { get; init; }
}

public record SetChatGroupRequest {
    public Guid? GroupId { get; init; }
}

public record ChatResponse(
    Guid ID,
    string Title,
    string? Description,
    Guid? GroupId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record ChatDetailResponse(
    Guid ID,
    string Title,
    string? Description,
    Guid? GroupId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ParticipantUserResponse> Users,
    List<ParticipantAgentResponse> Agents
);

public record AddUserParticipantRequest {
    [Required]
    public required Guid UserId { get; init; }
}

public record AddAgentParticipantRequest {
    [Required]
    public required Guid AgentId { get; init; }

    public List<string> AllowedTools { get; init; } = [];
}

public record ParticipantUserResponse(
    Guid UserId,
    string Name
);

public record ParticipantAgentResponse(
    Guid AgentId,
    string Name,
    List<string> AllowedTools
);

public record ChatParticipantsResponse(
    List<ParticipantUserResponse> Users,
    List<ParticipantAgentResponse> Agents
);
