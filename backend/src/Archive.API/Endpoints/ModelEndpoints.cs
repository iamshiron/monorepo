using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB;
using Shiron.TheArchive.DB.Schema.Car;
using Shiron.TheArchive.API.DTOs;

namespace Shiron.TheArchive.API.Endpoints;

public static class ModelEndpoints {
    public static void MapModelEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/api/models").WithTags("Models");

        group.MapGet("/", GetModels)
            .WithName("GetModels")
            .WithDescription("Get a paginated list of models")
            .Produces<PaginatedResult<ModelDto>>();

        group.MapGet("/{id:guid}", GetModel)
            .WithName("GetModel")
            .WithDescription("Get a model by ID")
            .Produces<ModelDto>()
            .Produces(404);

        group.MapPost("/", CreateModel)
            .WithName("CreateModel")
            .WithDescription("Create a new model")
            .RequireAuthorization("Admin")
            .Produces<ModelDto>(201)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapPut("/{id:guid}", UpdateModel)
            .WithName("UpdateModel")
            .WithDescription("Update a model")
            .RequireAuthorization("Admin")
            .Produces<ModelDto>()
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{id:guid}", DeleteModel)
            .WithName("DeleteModel")
            .WithDescription("Delete a model")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> GetModels(
        ArchiveDbContext db,
        Guid? brandId,
        int page = 1,
        int pageSize = 20) {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Models.AsQueryable();

        if (brandId.HasValue)
            query = query.Where(m => m.BrandID == brandId.Value);

        var totalCount = await query.CountAsync();

        var models = await query
            .OrderBy(m => m.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new ModelDto {
                ID = m.ID,
                Name = m.Name,
                BrandID = m.BrandID,
                BrandName = m.Brand.Name,
                CarCount = m.Cars.Count,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .ToListAsync();

        return Results.Ok(new PaginatedResult<ModelDto> {
            Items = models,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private static async Task<IResult> GetModel(Guid id, ArchiveDbContext db) {
        var model = await db.Models
            .Select(m => new ModelDto {
                ID = m.ID,
                Name = m.Name,
                BrandID = m.BrandID,
                BrandName = m.Brand.Name,
                CarCount = m.Cars.Count,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            })
            .FirstOrDefaultAsync(m => m.ID == id);

        return model == null ? Results.NotFound() : Results.Ok(model);
    }

    private static async Task<IResult> CreateModel(ModelCreateDto dto, ArchiveDbContext db) {
        var brand = await db.Brands.FindAsync(dto.BrandID);
        if (brand == null) return Results.NotFound($"Brand with ID {dto.BrandID} not found");

        var model = new Model { Name = dto.Name, BrandID = dto.BrandID, Brand = brand };
        db.Models.Add(model);
        await db.SaveChangesAsync();

        return Results.Created($"/api/models/{model.ID}", new ModelDto {
            ID = model.ID,
            Name = model.Name,
            BrandID = model.BrandID,
            BrandName = brand.Name,
            CarCount = 0,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        });
    }

    private static async Task<IResult> UpdateModel(Guid id, ModelUpdateDto dto, ArchiveDbContext db) {
        var model = await db.Models.FindAsync(id);
        if (model == null) return Results.NotFound();

        if (dto.BrandID.HasValue) {
            var brand = await db.Brands.FindAsync(dto.BrandID.Value);
            if (brand == null) return Results.NotFound($"Brand with ID {dto.BrandID} not found");
            model.BrandID = dto.BrandID.Value;
            model.Brand = brand;
        }

        if (dto.Name is not null) model.Name = dto.Name;
        model.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new ModelDto {
            ID = model.ID,
            Name = model.Name,
            BrandID = model.BrandID,
            BrandName = model.Brand.Name,
            CarCount = model.Cars.Count,
            CreatedAt = model.CreatedAt,
            UpdatedAt = model.UpdatedAt
        });
    }

    private static async Task<IResult> DeleteModel(Guid id, ArchiveDbContext db) {
        var model = await db.Models
            .Include(m => m.Cars)
            .FirstOrDefaultAsync(m => m.ID == id);

        if (model == null) return Results.NotFound();

        if (model.Cars.Count > 0)
            return Results.BadRequest("Cannot delete model with existing cars");

        db.Models.Remove(model);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
