using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB;
using Shiron.HonamiGit.DB.Schema;
using Shiron.Archive.API.DTOs;

namespace Shiron.HonamiGit.API.Endpoints;

public static class StatisticsEndpoints {
    public static void MapStatisticsEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/api/statistics").WithTags("Statistics");

        group.MapGet("/", GetStatistics)
            .WithName("GetStatistics")
            .WithDescription("Get full car statistics overview")
            .Produces<StatisticsResponse>();

        group.MapGet("/brands", GetBrandShares)
            .WithName("GetBrandShares")
            .WithDescription("Get brand share breakdown")
            .Produces<List<BrandShareDto>>();

        group.MapGet("/prices", GetPriceStats)
            .WithName("GetPriceStats")
            .WithDescription("Get price statistics")
            .Produces<CarStatisticsDto>();

        group.MapGet("/distributions", GetDistributions)
            .WithName("GetDistributions")
            .WithDescription("Get all distributions")
            .Produces<DistributionsResponse>();
    }

    private static async Task<IResult> GetStatistics(ArchiveDbContext db) {
        var cars = await db.Cars.ToListAsync();
        var totalCount = cars.Count;

        if (totalCount == 0) {
            return Results.Ok(new StatisticsResponse {
                PriceStats = new CarStatisticsDto(),
                BrandShares = [],
                BodyTypeDistribution = [],
                FuelTypeDistribution = [],
                ConditionDistribution = [],
                TransmissionDistribution = [],
                ColorDistribution = [],
                InteriorColorDistribution = []
            });
        }

        var priceStats = CalculatePriceStats(cars);
        var brandShares = await CalculateBrandShares(db, totalCount);
        var bodyTypeDist = BuildDistribution(cars.Where(c => c.BodyType.HasValue).Select(c => c.BodyType!.Value.ToString()));
        var fuelTypeDist = BuildDistribution(cars.Where(c => c.FuelType.HasValue).Select(c => c.FuelType!.Value.ToString()));
        var conditionDist = BuildDistribution(cars.Select(c => c.Condition.ToString()));
        var transmissionDist = BuildDistribution(cars.Where(c => c.Transmission.HasValue).Select(c => c.Transmission!.Value.ToString()));
        var colorDist = BuildDistribution(cars.Where(c => c.Color.HasValue).Select(c => c.Color!.Value.ToString()));
        var interiorColorDist = BuildDistribution(cars.Where(c => c.InteriorColor.HasValue).Select(c => c.InteriorColor!.Value.ToString()));

        return Results.Ok(new StatisticsResponse {
            PriceStats = priceStats,
            BrandShares = brandShares,
            BodyTypeDistribution = bodyTypeDist,
            FuelTypeDistribution = fuelTypeDist,
            ConditionDistribution = conditionDist,
            TransmissionDistribution = transmissionDist,
            ColorDistribution = colorDist,
            InteriorColorDistribution = interiorColorDist
        });
    }

    private static async Task<IResult> GetBrandShares(ArchiveDbContext db) {
        var totalCount = await db.Cars.CountAsync();
        if (totalCount == 0) return Results.Ok(Array.Empty<BrandShareDto>());

        var shares = await CalculateBrandShares(db, totalCount);
        return Results.Ok(shares);
    }

    private static async Task<IResult> GetPriceStats(ArchiveDbContext db) {
        var cars = await db.Cars.ToListAsync();
        return Results.Ok(cars.Count == 0 ? new CarStatisticsDto() : CalculatePriceStats(cars));
    }

    private static async Task<IResult> GetDistributions(ArchiveDbContext db) {
        var cars = await db.Cars.ToListAsync();

        return Results.Ok(new DistributionsResponse {
            BodyTypeDistribution = BuildDistribution(cars.Where(c => c.BodyType.HasValue).Select(c => c.BodyType!.Value.ToString())),
            FuelTypeDistribution = BuildDistribution(cars.Where(c => c.FuelType.HasValue).Select(c => c.FuelType!.Value.ToString())),
            ConditionDistribution = BuildDistribution(cars.Select(c => c.Condition.ToString())),
            TransmissionDistribution = BuildDistribution(cars.Where(c => c.Transmission.HasValue).Select(c => c.Transmission!.Value.ToString())),
            ColorDistribution = BuildDistribution(cars.Where(c => c.Color.HasValue).Select(c => c.Color!.Value.ToString())),
            InteriorColorDistribution = BuildDistribution(cars.Where(c => c.InteriorColor.HasValue).Select(c => c.InteriorColor!.Value.ToString()))
        });
    }

    private static CarStatisticsDto CalculatePriceStats(List<Car> cars) {
        var prices = cars.Select(c => c.PriceEur).OrderBy(p => p).ToList();
        var median = prices.Count % 2 == 0
            ? (prices[prices.Count / 2 - 1] + prices[prices.Count / 2]) / 2.0
            : prices[prices.Count / 2];

        return new CarStatisticsDto {
            TotalCars = cars.Count,
            AveragePrice = prices.Average(),
            MedianPrice = median,
            MinPrice = prices.Min(),
            MaxPrice = prices.Max(),
            AveragePower = cars.Where(c => c.PowerKw.HasValue).Select(c => c.PowerKw!.Value).DefaultIfEmpty(0).Average(),
            AverageMileage = cars.Where(c => c.MileageKm.HasValue).Select(c => c.MileageKm!.Value).DefaultIfEmpty(0).Average()
        };
    }

    private static async Task<List<BrandShareDto>> CalculateBrandShares(ArchiveDbContext db, int totalCount) {
        return await db.Cars
            .GroupBy(c => new { c.BrandID, c.Brand.Name })
            .Select(g => new BrandShareDto {
                BrandID = g.Key.BrandID,
                BrandName = g.Key.Name,
                CarCount = g.Count(),
                Percentage = Math.Round((double) g.Count() / totalCount * 100, 2)
            })
            .OrderByDescending(b => b.CarCount)
            .ToListAsync();
    }

    private static List<DistributionEntry> BuildDistribution(IEnumerable<string> values) {
        var grouped = values
            .GroupBy(v => v)
            .Select(g => new { Label = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToList();

        var total = grouped.Sum(x => x.Count);
        return grouped.Select(x => new DistributionEntry {
            Label = x.Label,
            Count = x.Count,
            Percentage = total > 0 ? Math.Round((double) x.Count / total * 100, 2) : 0
        }).ToList();
    }
}

public record DistributionsResponse {
    public List<DistributionEntry> BodyTypeDistribution { get; init; } = [];
    public List<DistributionEntry> FuelTypeDistribution { get; init; } = [];
    public List<DistributionEntry> ConditionDistribution { get; init; } = [];
    public List<DistributionEntry> TransmissionDistribution { get; init; } = [];
    public List<DistributionEntry> ColorDistribution { get; init; } = [];
    public List<DistributionEntry> InteriorColorDistribution { get; init; } = [];
}
