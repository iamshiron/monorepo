namespace Shiron.HonamiGit.API.DTOs;

public record HealthResponseDTO(string Status) {
    public static readonly HealthResponseDTO Ok = new("OK");
}
