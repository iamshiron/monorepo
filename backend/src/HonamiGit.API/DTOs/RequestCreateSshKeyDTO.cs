namespace Shiron.Archive.API.DTOs;

public record RequestCreateSshKeyDTO(
    string Name,
    DateTime? ExpiresAt,
    string Key
);
