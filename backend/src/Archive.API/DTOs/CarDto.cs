using Shiron.HonamiGit.DB.Schema;

namespace Shiron.Archive.API.DTOs;

public record CarDto {
    public Guid ID { get; init; }
    public Guid BrandID { get; init; }
    public required string BrandName { get; init; }
    public Guid ModelID { get; init; }
    public required string ModelName { get; init; }
    public required string Variant { get; init; }
    public string Description { get; init; } = string.Empty;
    public required int Seats { get; init; }
    public required int Doors { get; init; }
    public required int PriceEur { get; init; }
    public required DateOnly RegistrationDate { get; init; }
    public int? MileageKm { get; init; }
    public int? PowerKw { get; init; }
    public bool Damaged { get; init; }
    public required Condition Condition { get; init; }
    public BodyType? BodyType { get; init; }
    public FuelType? FuelType { get; init; }
    public Transmission? Transmission { get; init; }
    public ExteriorColor? Color { get; init; }
    public InteriorColor? InteriorColor { get; init; }
    public InteriorType? InteriorType { get; init; }
    public List<SimpleImageDto> Images { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CarSummaryDto {
    public Guid ID { get; init; }
    public required string BrandName { get; init; }
    public required string ModelName { get; init; }
    public required string Variant { get; init; }
    public required int PriceEur { get; init; }
    public required Condition Condition { get; init; }
    public BodyType? BodyType { get; init; }
    public int? MileageKm { get; init; }
    public DateOnly RegistrationDate { get; init; }
    public Guid? ThumbnailImageID { get; init; }
}

public record CarCreateDto {
    public required Guid BrandID { get; init; }
    public required Guid ModelID { get; init; }
    public required string Variant { get; init; }
    public string? Description { get; init; }
    public required int Seats { get; init; }
    public required int Doors { get; init; }
    public required int PriceEur { get; init; }
    public required DateOnly RegistrationDate { get; init; }
    public int? MileageKm { get; init; }
    public int? PowerKw { get; init; }
    public bool Damaged { get; init; }
    public required Condition Condition { get; init; }
    public BodyType? BodyType { get; init; }
    public FuelType? FuelType { get; init; }
    public Transmission? Transmission { get; init; }
    public ExteriorColor? Color { get; init; }
    public InteriorColor? InteriorColor { get; init; }
    public InteriorType? InteriorType { get; init; }
    public List<Guid>? ImageIDs { get; init; }
}

public record CarUpdateDto {
    public Guid? BrandID { get; init; }
    public Guid? ModelID { get; init; }
    public string? Variant { get; init; }
    public string? Description { get; init; }
    public int? Seats { get; init; }
    public int? Doors { get; init; }
    public int? PriceEur { get; init; }
    public DateOnly? RegistrationDate { get; init; }
    public int? MileageKm { get; init; }
    public int? PowerKw { get; init; }
    public bool? Damaged { get; init; }
    public Condition? Condition { get; init; }
    public BodyType? BodyType { get; init; }
    public FuelType? FuelType { get; init; }
    public Transmission? Transmission { get; init; }
    public ExteriorColor? Color { get; init; }
    public InteriorColor? InteriorColor { get; init; }
    public InteriorType? InteriorType { get; init; }
    public List<Guid>? ImageIDs { get; init; }
}

public record CarFilterParams : PaginationParams {
    public Guid? BrandID { get; init; }
    public Guid? ModelID { get; init; }
    public Condition? Condition { get; init; }
    public BodyType? BodyType { get; init; }
    public FuelType? FuelType { get; init; }
    public Transmission? Transmission { get; init; }
    public ExteriorColor? Color { get; init; }
    public int? MinPrice { get; init; }
    public int? MaxPrice { get; init; }
    public int? MinYear { get; init; }
    public int? MaxYear { get; init; }
    public int? MinPower { get; init; }
    public int? MaxPower { get; init; }
    public int? MinSeats { get; init; }
    public bool? Damaged { get; init; }
    public string? Search { get; init; }
}
