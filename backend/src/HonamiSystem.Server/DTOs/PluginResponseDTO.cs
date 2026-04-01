namespace Shiron.HonamiSystem.Server.DTOs;

public record PluginResponseDTO(
    string Group,
    string Name,
    string Version,
    string Triple
);
