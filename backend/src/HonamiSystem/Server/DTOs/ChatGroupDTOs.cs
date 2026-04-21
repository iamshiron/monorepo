using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.Server.DTOs;

public record CreateChatGroupRequest {
    [Required, MaxLength(64)]
    public required string Name { get; init; }
}

public record UpdateChatGroupRequest {
    [Required, MaxLength(64)]
    public required string Name { get; init; }
}

public record ChatGroupResponse(
    Guid ID,
    string Name,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    List<ChatGroupChatEntry> Chats
);

public record ChatGroupChatEntry(
    Guid ID,
    string Title
);
