using Microsoft.EntityFrameworkCore;
using Shiron.TheArchive.DB;
using Shiron.TheArchive.DB.Schema;
using Shiron.TheArchive.API.DTOs;

namespace Shiron.TheArchive.API.Endpoints;

public static class BrandEndpoints {
    public static void MapBrandEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/api/brands").WithTags("Brands");

        group.MapGet("/", GetBrands)
            .WithName("GetBrands")
            .WithDescription("Get a paginated list of brands")
            .Produces<PaginatedResult<BrandDto>>();

        group.MapGet("/{id:guid}", GetBrand)
            .WithName("GetBrand")
            .WithDescription("Get a brand by ID")
            .Produces<BrandDto>()
            .Produces(404);

        group.MapPost("/", CreateBrand)
            .WithName("CreateBrand")
            .WithDescription("Create a new brand")
            .RequireAuthorization("Admin")
            .Produces<BrandDto>(201)
            .Produces(401)
            .Produces(403);

        group.MapPut("/{id:guid}", UpdateBrand)
            .WithName("UpdateBrand")
            .WithDescription("Update a brand")
            .RequireAuthorization("Admin")
            .Produces<BrandDto>()
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{id:guid}", DeleteBrand)
            .WithName("DeleteBrand")
            .WithDescription("Delete a brand")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapGet("/{id:guid}/models", GetModelsByBrand)
            .WithName("GetModelsByBrand")
            .WithDescription("Get models for a brand")
            .RequireAuthorization("Admin")
            .Produces<List<ModelDto>>()
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> GetBrands(
        ArchiveDbContext db,
        string? search,
        int page = 1,
        int pageSize = 20) {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Brands.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(b => b.Name.Contains(search));

        var totalCount = await query.CountAsync();

        var brands = await query
            .OrderBy(b => b.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(b => new BrandDto {
                ID = b.ID,
                Name = b.Name,
                CarCount = b.Cars.Count,
                ModelCount = b.Models.Count,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .ToListAsync();

        return Results.Ok(new PaginatedResult<BrandDto> {
            Items = brands,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private static async Task<IResult> GetBrand(Guid id, ArchiveDbContext db) {
        var brand = await db.Brands
            .Select(b => new BrandDto {
                ID = b.ID,
                Name = b.Name,
                CarCount = b.Cars.Count,
                ModelCount = b.Models.Count,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync(b => b.ID == id);

        return brand == null ? Results.NotFound() : Results.Ok(brand);
    }

    private static async Task<IResult> CreateBrand(BrandCreateDto dto, ArchiveDbContext db) {
        var brand = new Brand { Name = dto.Name };
        db.Brands.Add(brand);
        await db.SaveChangesAsync();

        return Results.Created($"/api/brands/{brand.ID}", new BrandDto {
            ID = brand.ID,
            Name = brand.Name,
            CarCount = 0,
            ModelCount = 0,
            CreatedAt = brand.CreatedAt,
            UpdatedAt = brand.UpdatedAt
        });
    }

    private static async Task<IResult> UpdateBrand(Guid id, BrandUpdateDto dto, ArchiveDbContext db) {
        var brand = await db.Brands.FindAsync(id);
        if (brand == null) return Results.NotFound();

        if (dto.Name is not null) brand.Name = dto.Name;
        brand.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(new BrandDto {
            ID = brand.ID,
            Name = brand.Name,
            CarCount = brand.Cars.Count,
            ModelCount = brand.Models.Count,
            CreatedAt = brand.CreatedAt,
            UpdatedAt = brand.UpdatedAt
        });
    }

    private static async Task<IResult> DeleteBrand(Guid id, ArchiveDbContext db) {
        var brand = await db.Brands
            .Include(b => b.Cars)
            .Include(b => b.Models)
            .FirstOrDefaultAsync(b => b.ID == id);

        if (brand == null) return Results.NotFound();

        if (brand.Cars.Count > 0 || brand.Models.Count > 0)
            return Results.BadRequest("Cannot delete brand with existing cars or models");

        db.Brands.Remove(brand);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> GetModelsByBrand(Guid id, ArchiveDbContext db) {
        var brandExists = await db.Brands.AnyAsync(b => b.ID == id);
        if (!brandExists) return Results.NotFound();

        var models = await db.Models
            .Where(m => m.BrandID == id)
            .OrderBy(m => m.Name)
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

        return Results.Ok(models);
    }
}
