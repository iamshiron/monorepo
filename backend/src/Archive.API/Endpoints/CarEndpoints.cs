using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB;
using Shiron.HonamiGit.DB.Schema;
using Shiron.HonamiGit.API.DTOs;

namespace Shiron.HonamiGit.API.Endpoints;

public static class CarEndpoints {
    public static void MapCarEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/api/cars").WithTags("Cars");

        group.MapGet("/", GetCars)
            .WithName("GetCars")
            .WithDescription("Get a paginated list of cars with optional filtering")
            .Produces<PaginatedResult<CarSummaryDto>>();

        group.MapGet("/{id:guid}", GetCar)
            .WithName("GetCar")
            .WithDescription("Get a car by ID")
            .Produces<CarDto>()
            .Produces(404);

        group.MapPost("/", CreateCar)
            .WithName("CreateCar")
            .WithDescription("Create a new car")
            .RequireAuthorization("Admin")
            .Produces<CarDto>(201)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapPut("/{id:guid}", UpdateCar)
            .WithName("UpdateCar")
            .WithDescription("Update a car")
            .RequireAuthorization("Admin")
            .Produces<CarDto>()
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{id:guid}", DeleteCar)
            .WithName("DeleteCar")
            .WithDescription("Delete a car")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> GetCars(
        ArchiveDbContext db,
        Guid? brandId,
        Guid? modelId,
        Condition? condition,
        BodyType? bodyType,
        FuelType? fuelType,
        Transmission? transmission,
        ExteriorColor? color,
        int? minPrice,
        int? maxPrice,
        int? minYear,
        int? maxYear,
        int? minPower,
        int? maxPower,
        int? minSeats,
        bool? damaged,
        string? search,
        int page = 1,
        int pageSize = 20,
        string? sortBy = null,
        bool sortDescending = false) {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Cars
            .Include(c => c.Brand)
            .Include(c => c.Model)
            .Include(c => c.Images)
            .AsQueryable();

        if (brandId.HasValue) query = query.Where(c => c.BrandID == brandId.Value);
        if (modelId.HasValue) query = query.Where(c => c.ModelID == modelId.Value);
        if (condition.HasValue) query = query.Where(c => c.Condition == condition.Value);
        if (bodyType.HasValue) query = query.Where(c => c.BodyType == bodyType.Value);
        if (fuelType.HasValue) query = query.Where(c => c.FuelType == fuelType.Value);
        if (transmission.HasValue) query = query.Where(c => c.Transmission == transmission.Value);
        if (color.HasValue) query = query.Where(c => c.Color == color.Value);
        if (minPrice.HasValue) query = query.Where(c => c.PriceEur >= minPrice.Value);
        if (maxPrice.HasValue) query = query.Where(c => c.PriceEur <= maxPrice.Value);
        if (minYear.HasValue) query = query.Where(c => c.RegistrationDate.Year >= minYear.Value);
        if (maxYear.HasValue) query = query.Where(c => c.RegistrationDate.Year <= maxYear.Value);
        if (minPower.HasValue) query = query.Where(c => c.PowerKw >= minPower.Value);
        if (maxPower.HasValue) query = query.Where(c => c.PowerKw <= maxPower.Value);
        if (minSeats.HasValue) query = query.Where(c => c.Seats >= minSeats.Value);
        if (damaged.HasValue) query = query.Where(c => c.Damaged == damaged.Value);
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Variant.Contains(search));

        query = ApplySorting(query, sortBy, sortDescending);

        var totalCount = await query.CountAsync();

        var cars = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new CarSummaryDto {
                ID = c.ID,
                BrandName = c.Brand.Name,
                ModelName = c.Model.Name,
                Variant = c.Variant,
                PriceEur = c.PriceEur,
                Condition = c.Condition,
                BodyType = c.BodyType,
                MileageKm = c.MileageKm,
                RegistrationDate = c.RegistrationDate,
                ThumbnailImageID = c.Images.FirstOrDefault() != null ? c.Images.FirstOrDefault()!.ID : null
            })
            .ToListAsync();

        return Results.Ok(new PaginatedResult<CarSummaryDto> {
            Items = cars,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private static async Task<IResult> GetCar(Guid id, ArchiveDbContext db) {
        var car = await db.Cars
            .Include(c => c.Brand)
            .Include(c => c.Model)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (car == null) return Results.NotFound();

        return Results.Ok(new CarDto {
            ID = car.ID,
            BrandID = car.BrandID,
            BrandName = car.Brand.Name,
            ModelID = car.ModelID,
            ModelName = car.Model.Name,
            Variant = car.Variant,
            Description = car.Description,
            Seats = car.Seats,
            Doors = car.Doors,
            PriceEur = car.PriceEur,
            RegistrationDate = car.RegistrationDate,
            MileageKm = car.MileageKm,
            PowerKw = car.PowerKw,
            Damaged = car.Damaged,
            Condition = car.Condition,
            BodyType = car.BodyType,
            FuelType = car.FuelType,
            Transmission = car.Transmission,
            Color = car.Color,
            InteriorColor = car.InteriorColor,
            InteriorType = car.InteriorType,
            ImageIDs = car.Images.Select(i => i.ID).ToList(),
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        });
    }

    private static async Task<IResult> CreateCar(CarCreateDto dto, ArchiveDbContext db) {
        var brand = await db.Brands.FindAsync(dto.BrandID);
        if (brand == null) return Results.NotFound($"Brand with ID {dto.BrandID} not found");

        var model = await db.Models.Include(m => m.Brand).FirstOrDefaultAsync(m => m.ID == dto.ModelID);
        if (model == null) return Results.NotFound($"Model with ID {dto.ModelID} not found");

        var car = new Car {
            BrandID = dto.BrandID,
            Brand = brand,
            ModelID = dto.ModelID,
            Model = model,
            Variant = dto.Variant,
            Description = dto.Description ?? string.Empty,
            Seats = dto.Seats,
            Doors = dto.Doors,
            PriceEur = dto.PriceEur,
            RegistrationDate = dto.RegistrationDate,
            MileageKm = dto.MileageKm,
            PowerKw = dto.PowerKw,
            Damaged = dto.Damaged,
            Condition = dto.Condition,
            BodyType = dto.BodyType,
            FuelType = dto.FuelType,
            Transmission = dto.Transmission,
            Color = dto.Color,
            InteriorColor = dto.InteriorColor,
            InteriorType = dto.InteriorType
        };

        if (dto.ImageIDs is { Count: > 0 }) {
            var images = await db.Images.Where(i => dto.ImageIDs.Contains(i.ID)).ToListAsync();
            foreach (var image in images)
                car.Images.Add(image);
        }

        db.Cars.Add(car);
        await db.SaveChangesAsync();

        return Results.Created($"/api/cars/{car.ID}", new CarDto {
            ID = car.ID,
            BrandID = car.BrandID,
            BrandName = brand.Name,
            ModelID = car.ModelID,
            ModelName = model.Name,
            Variant = car.Variant,
            Description = car.Description,
            Seats = car.Seats,
            Doors = car.Doors,
            PriceEur = car.PriceEur,
            RegistrationDate = car.RegistrationDate,
            MileageKm = car.MileageKm,
            PowerKw = car.PowerKw,
            Damaged = car.Damaged,
            Condition = car.Condition,
            BodyType = car.BodyType,
            FuelType = car.FuelType,
            Transmission = car.Transmission,
            Color = car.Color,
            InteriorColor = car.InteriorColor,
            InteriorType = car.InteriorType,
            ImageIDs = car.Images.Select(i => i.ID).ToList(),
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        });
    }

    private static async Task<IResult> UpdateCar(Guid id, CarUpdateDto dto, ArchiveDbContext db) {
        var car = await db.Cars
            .Include(c => c.Brand)
            .Include(c => c.Model)
            .Include(c => c.Images)
            .FirstOrDefaultAsync(c => c.ID == id);

        if (car == null) return Results.NotFound();

        if (dto.BrandID.HasValue) {
            var brand = await db.Brands.FindAsync(dto.BrandID.Value);
            if (brand == null) return Results.NotFound($"Brand with ID {dto.BrandID} not found");
            car.BrandID = dto.BrandID.Value;
            car.Brand = brand;
        }

        if (dto.ModelID.HasValue) {
            var model = await db.Models.Include(m => m.Brand).FirstOrDefaultAsync(m => m.ID == dto.ModelID.Value);
            if (model == null) return Results.NotFound($"Model with ID {dto.ModelID} not found");
            car.ModelID = dto.ModelID.Value;
            car.Model = model;
        }

        if (dto.Variant is not null) car.Variant = dto.Variant;
        if (dto.Description is not null) car.Description = dto.Description;
        if (dto.Seats.HasValue) car.Seats = dto.Seats.Value;
        if (dto.Doors.HasValue) car.Doors = dto.Doors.Value;
        if (dto.PriceEur.HasValue) car.PriceEur = dto.PriceEur.Value;
        if (dto.RegistrationDate.HasValue) car.RegistrationDate = dto.RegistrationDate.Value;
        if (dto.MileageKm.HasValue) car.MileageKm = dto.MileageKm.Value;
        if (dto.PowerKw.HasValue) car.PowerKw = dto.PowerKw.Value;
        if (dto.Damaged.HasValue) car.Damaged = dto.Damaged.Value;
        if (dto.Condition.HasValue) car.Condition = dto.Condition.Value;
        if (dto.BodyType.HasValue) car.BodyType = dto.BodyType.Value;
        if (dto.FuelType.HasValue) car.FuelType = dto.FuelType.Value;
        if (dto.Transmission.HasValue) car.Transmission = dto.Transmission.Value;
        if (dto.Color.HasValue) car.Color = dto.Color.Value;
        if (dto.InteriorColor.HasValue) car.InteriorColor = dto.InteriorColor.Value;
        if (dto.InteriorType.HasValue) car.InteriorType = dto.InteriorType.Value;

        if (dto.ImageIDs is not null) {
            car.Images.Clear();
            if (dto.ImageIDs.Count > 0) {
                var images = await db.Images.Where(i => dto.ImageIDs.Contains(i.ID)).ToListAsync();
                foreach (var image in images)
                    car.Images.Add(image);
            }
        }

        car.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new CarDto {
            ID = car.ID,
            BrandID = car.BrandID,
            BrandName = car.Brand.Name,
            ModelID = car.ModelID,
            ModelName = car.Model.Name,
            Variant = car.Variant,
            Description = car.Description,
            Seats = car.Seats,
            Doors = car.Doors,
            PriceEur = car.PriceEur,
            RegistrationDate = car.RegistrationDate,
            MileageKm = car.MileageKm,
            PowerKw = car.PowerKw,
            Damaged = car.Damaged,
            Condition = car.Condition,
            BodyType = car.BodyType,
            FuelType = car.FuelType,
            Transmission = car.Transmission,
            Color = car.Color,
            InteriorColor = car.InteriorColor,
            InteriorType = car.InteriorType,
            ImageIDs = car.Images.Select(i => i.ID).ToList(),
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        });
    }

    private static async Task<IResult> DeleteCar(Guid id, ArchiveDbContext db) {
        var car = await db.Cars.FindAsync(id);
        if (car == null) return Results.NotFound();

        db.Cars.Remove(car);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static IQueryable<Car> ApplySorting(IQueryable<Car> query, string? sortBy, bool descending) {
        return sortBy?.ToLowerInvariant() switch {
            "price" => descending ? query.OrderByDescending(c => c.PriceEur) : query.OrderBy(c => c.PriceEur),
            "year" => descending ? query.OrderByDescending(c => c.RegistrationDate) : query.OrderBy(c => c.RegistrationDate),
            "power" => descending ? query.OrderByDescending(c => c.PowerKw) : query.OrderBy(c => c.PowerKw),
            "mileage" => descending ? query.OrderByDescending(c => c.MileageKm) : query.OrderBy(c => c.MileageKm),
            _ => descending ? query.OrderByDescending(c => c.CreatedAt) : query.OrderBy(c => c.CreatedAt)
        };
    }
}
