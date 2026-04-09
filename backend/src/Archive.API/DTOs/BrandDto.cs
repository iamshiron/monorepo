namespace Shiron.HonamiGit.API.DTOs;

public record BrandDto {
    public Guid ID { get; init; }
    public required string Name { get; init; }
    public int CarCount { get; init; }
    public int ModelCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record BrandCreateDto {
    public required string Name { get; init; }
}

public record BrandUpdateDto {
    public string? Name { get; init; }
}
