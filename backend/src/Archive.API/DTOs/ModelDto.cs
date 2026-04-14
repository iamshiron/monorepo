namespace Shiron.Archive.API.DTOs;

public record ModelDto {
    public Guid ID { get; init; }
    public required string Name { get; init; }
    public Guid BrandID { get; init; }
    public required string BrandName { get; init; }
    public int CarCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record ModelCreateDto {
    public required string Name { get; init; }
    public required Guid BrandID { get; init; }
}

public record ModelUpdateDto {
    public string? Name { get; init; }
    public Guid? BrandID { get; init; }
}
