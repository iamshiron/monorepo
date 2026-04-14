namespace Shiron.Archive.API.DTOs;

public record RequestUpdateSshKeyDTO(
    string? Name,
    string? Description
);
