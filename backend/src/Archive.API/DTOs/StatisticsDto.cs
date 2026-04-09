namespace Shiron.HonamiGit.API.DTOs;

public record CarStatisticsDto {
    public int TotalCars { get; init; }
    public double AveragePrice { get; init; }
    public double MedianPrice { get; init; }
    public int MinPrice { get; init; }
    public int MaxPrice { get; init; }
    public double AveragePower { get; init; }
    public double AverageMileage { get; init; }
}

public record BrandShareDto {
    public Guid BrandID { get; init; }
    public required string BrandName { get; init; }
    public int CarCount { get; init; }
    public double Percentage { get; init; }
}

public record DistributionEntry {
    public required string Label { get; init; }
    public int Count { get; init; }
    public double Percentage { get; init; }
}

public record StatisticsResponse {
    public required CarStatisticsDto PriceStats { get; init; }
    public List<BrandShareDto> BrandShares { get; init; } = [];
    public List<DistributionEntry> BodyTypeDistribution { get; init; } = [];
    public List<DistributionEntry> FuelTypeDistribution { get; init; } = [];
    public List<DistributionEntry> ConditionDistribution { get; init; } = [];
    public List<DistributionEntry> TransmissionDistribution { get; init; } = [];
    public List<DistributionEntry> ColorDistribution { get; init; } = [];
    public List<DistributionEntry> InteriorColorDistribution { get; init; } = [];
}
