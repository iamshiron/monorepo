namespace Shiron.Archive.API.DTOs;

public record ResponseSSHKeyDTO(
    Guid ID,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
