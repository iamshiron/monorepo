namespace Shiron.HonamiGit.API.DTOs;

public record ResponseSSHKeyDTO(
    Guid ID,
    string Name,
    string Description,
    DateTime CreatedAt,
    DateTime? ExpiresAt
);
