using System.Security.Cryptography;
using BlurHashSharp;
using Microsoft.EntityFrameworkCore;
using Shiron.Lib.Types;
using Shiron.Lib.Types.Extension;
using Shiron.TheArchive.DB;
using Shiron.HonamiGit.DB.Schema;
using Shiron.HonamiGit.API.DTOs;
using Shiron.TheArchive.API.Services;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Processing.Processors.Quantization;
using DBImage = Shiron.HonamiGit.DB.Schema.Image;

namespace Shiron.HonamiGit.API.Endpoints;

public static class ImageEndpoints {
    public static void MapImageEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/api/images").WithTags("Images");

        group.MapGet("/", GetImages)
            .WithName("GetImages")
            .WithDescription("Get a paginated list of images")
            .Produces<PaginatedResult<ImageDto>>();

        group.MapGet("/{id:guid}", GetImage)
            .WithName("GetImage")
            .WithDescription("Get image metadata by ID")
            .Produces<ImageDto>()
            .Produces(404);

        group.MapGet("/{id:guid}.webp", GetImageFile)
            .WithName("GetImageFile")
            .WithDescription("Serve the actual image file from storage")
            .Produces(200)
            .Produces(404);

        group.MapPost("/", CreateImage)
            .WithName("CreateImage")
            .WithDescription("Create an image metadata record")
            .RequireAuthorization("Admin")
            .Produces<ImageDto>(201)
            .Produces(401)
            .Produces(403);

        group.MapPost("/upload", UploadImage)
            .WithName("UploadImage")
            .WithDescription("Upload an image file, process it, and store in MinIO")
            .RequireAuthorization("Admin")
            .DisableAntiforgery()
            .Produces<ImageDto>(201)
            .Produces(401)
            .Produces(403);

        group.MapPut("/{id:guid}", UpdateImage)
            .WithName("UpdateImage")
            .WithDescription("Update image metadata")
            .RequireAuthorization("Admin")
            .Produces<ImageDto>()
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{id:guid}", DeleteImage)
            .WithName("DeleteImage")
            .WithDescription("Delete image metadata and storage object")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapPost("/{imageId:guid}/cars/{carId:guid}", LinkCar)
            .WithName("LinkImageToCar")
            .WithDescription("Link an image to a car")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);

        group.MapDelete("/{imageId:guid}/cars/{carId:guid}", UnlinkCar)
            .WithName("UnlinkImageFromCar")
            .WithDescription("Unlink an image from a car")
            .RequireAuthorization("Admin")
            .Produces(204)
            .Produces(401)
            .Produces(403)
            .Produces(404);
    }

    private static async Task<IResult> GetImages(
        ArchiveDbContext db,
        Guid? carId,
        int page = 1,
        int pageSize = 20) {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = db.Images.AsQueryable();

        if (carId.HasValue)
            query = query.Where(i => i.Cars.Any(c => c.ID == carId.Value));

        var totalCount = await query.CountAsync();

        var images = await query
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return Results.Ok(new PaginatedResult<ImageDto> {
            Items = images.Select(MapToDto).ToList(),
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        });
    }

    private static async Task<IResult> GetImage(Guid id, ArchiveDbContext db) {
        var image = await db.Images
            .Include(i => i.Cars)
            .FirstOrDefaultAsync(i => i.ID == id);

        return image == null ? Results.NotFound() : Results.Ok(MapToDto(image));
    }

    private static async Task<IResult> GetImageFile(Guid id, ArchiveDbContext db, IStorageService storage) {
        var image = await db.Images.FindAsync(id);
        if (image == null) return Results.NotFound();

        var stream = await storage.GetAsync(image.Bucket, image.ObjectKey);
        if (stream == null) return Results.NotFound("Image file not found in storage");

        return Results.File(stream, "image/webp");
    }

    private static async Task<IResult> CreateImage(ImageCreateDto dto, ArchiveDbContext db) {
        var image = new DBImage {
            Bucket = dto.Bucket,
            ObjectKey = dto.ObjectKey,
            Width = dto.Width,
            Height = dto.Height,
            BlurHash = dto.BlurHash,
            PrimaryColor = MapToColorPack(dto.PrimaryColor),
            SecondaryColor = MapToColorPack(dto.SecondaryColor),
            Palette = (dto.Palette ?? []).Select(MapToColorPack).ToList()
        };

        if (dto.CarIDs is { Count: > 0 }) {
            var cars = await db.Cars.Where(c => dto.CarIDs.Contains(c.ID)).ToListAsync();
            foreach (var car in cars)
                image.Cars.Add(car);
        }

        db.Images.Add(image);
        await db.SaveChangesAsync();

        return Results.Created($"/api/images/{image.ID}", MapToDto(image));
    }

    private static async Task<IResult> UploadImage(
        IFormFile file,
        ArchiveDbContext db,
        IStorageService storage,
        IConfiguration config) {
        if (file.Length == 0) return Results.BadRequest("No file provided");

        await using var inputStream = file.OpenReadStream();
        using var sharpImage = SixLabors.ImageSharp.Image.Load(inputStream);

        var objectKey = $"images/{DateTime.UtcNow:yyyy/MM/dd}/{Guid.CreateVersion7()}.webp";

        var blurHash = EncodeBlurHash(sharpImage);
        var (primaryColor, secondaryColor, palette) = ExtractColors(sharpImage);

        using var uploadStream = new MemoryStream();
        await sharpImage.SaveAsWebpAsync(uploadStream);
        uploadStream.Position = 0;

        var bucket = config["ARCHIVE_MINIO_BUCKET_IMAGES"] ?? "archive-images";
        await storage.StoreAsync(bucket, objectKey, uploadStream, "image/webp");

        var dbImage = new DBImage {
            Bucket = bucket,
            ObjectKey = objectKey,
            Width = sharpImage.Width,
            Height = sharpImage.Height,
            BlurHash = blurHash,
            PrimaryColor = primaryColor,
            SecondaryColor = secondaryColor,
            Palette = palette
        };

        db.Images.Add(dbImage);
        await db.SaveChangesAsync();

        return Results.Created($"/api/images/{dbImage.ID}", MapToDto(dbImage));
    }

    private static async Task<IResult> UpdateImage(Guid id, ImageUpdateDto dto, ArchiveDbContext db) {
        var image = await db.Images
            .Include(i => i.Cars)
            .FirstOrDefaultAsync(i => i.ID == id);

        if (image == null) return Results.NotFound();

        if (dto.Bucket is not null) image.Bucket = dto.Bucket;
        if (dto.ObjectKey is not null) image.ObjectKey = dto.ObjectKey;
        if (dto.Width.HasValue) image.Width = dto.Width.Value;
        if (dto.Height.HasValue) image.Height = dto.Height.Value;
        if (dto.BlurHash is not null) image.BlurHash = dto.BlurHash;

        if (dto.PrimaryColor is not null) image.PrimaryColor = MapToColorPack(dto.PrimaryColor);
        if (dto.SecondaryColor is not null) image.SecondaryColor = MapToColorPack(dto.SecondaryColor);

        if (dto.Palette is not null)
            image.Palette = dto.Palette.Select(MapToColorPack).ToList();

        if (dto.CarIDs is not null) {
            image.Cars.Clear();
            if (dto.CarIDs.Count > 0) {
                var cars = await db.Cars.Where(c => dto.CarIDs.Contains(c.ID)).ToListAsync();
                foreach (var car in cars)
                    image.Cars.Add(car);
            }
        }

        image.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        return Results.Ok(MapToDto(image));
    }

    private static async Task<IResult> DeleteImage(Guid id, ArchiveDbContext db, IStorageService storage) {
        var image = await db.Images.FindAsync(id);
        if (image == null) return Results.NotFound();

        await storage.DeleteAsync(image.Bucket, image.ObjectKey);
        db.Images.Remove(image);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> LinkCar(Guid imageId, Guid carId, ArchiveDbContext db) {
        var image = await db.Images.Include(i => i.Cars).FirstOrDefaultAsync(i => i.ID == imageId);
        if (image == null) return Results.NotFound($"Image with ID {imageId} not found");

        var car = await db.Cars.Include(c => c.Images).FirstOrDefaultAsync(c => c.ID == carId);
        if (car == null) return Results.NotFound($"Car with ID {carId} not found");

        if (!image.Cars.Any(c => c.ID == carId))
            image.Cars.Add(car);

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static async Task<IResult> UnlinkCar(Guid imageId, Guid carId, ArchiveDbContext db) {
        var image = await db.Images.Include(i => i.Cars).FirstOrDefaultAsync(i => i.ID == imageId);
        if (image == null) return Results.NotFound($"Image with ID {imageId} not found");

        var car = image.Cars.FirstOrDefault(c => c.ID == carId);
        if (car != null) image.Cars.Remove(car);

        await db.SaveChangesAsync();
        return Results.NoContent();
    }

    private static ImageDto MapToDto(DBImage image) {
        return new ImageDto {
            ID = image.ID,
            Bucket = image.Bucket,
            ObjectKey = image.ObjectKey,
            Width = image.Width,
            Height = image.Height,
            BlurHash = image.BlurHash,
            PrimaryColor = MapFromColorPack(image.PrimaryColor),
            SecondaryColor = MapFromColorPack(image.SecondaryColor),
            Palette = image.Palette.Select(MapFromColorPack).ToList(),
            CarIDs = image.Cars.Select(c => c.ID).ToList(),
            CharacterID = image.CharacterID,
            CreatedAt = image.CreatedAt,
            UpdatedAt = image.UpdatedAt
        };
    }

    private static ColorPackDto MapFromColorPack(ColorPack pack) {
        return new ColorPackDto {
            Color = new Color32Dto { R = pack.Color.R, G = pack.Color.G, B = pack.Color.B, A = pack.Color.A },
            Lab = new LabColorDto { L = pack.Lab.L, A = pack.Lab.A, B = pack.Lab.B }
        };
    }

    private static ColorPack MapToColorPack(ColorPackDto dto) {
        return new ColorPack {
            Color = new Color32(dto.Color.R, dto.Color.G, dto.Color.B, dto.Color.A),
            Lab = new LabColor(dto.Lab.L, dto.Lab.A, dto.Lab.B)
        };
    }

    private static string EncodeBlurHash(SixLabors.ImageSharp.Image image) {
        using var cloned = image.CloneAs<Rgba32>();
        cloned.Mutate(x => x.Resize(64, 0));
        using var ms = new MemoryStream();
        cloned.SaveAsBmp(ms);
        var pixels = ms.ToArray();

        var bmpHeaderSize = 54;
        var width = cloned.Width;
        var height = cloned.Height;
        var stride = width * 4;

        var pixelSpan = new ReadOnlySpan<byte>(pixels, bmpHeaderSize, width * height * 4);
        return CoreBlurHashEncoder.Encode(4, 3, width, height, pixelSpan, stride, PixelFormat.RGB888x);
    }

    private static (ColorPack Primary, ColorPack Secondary, List<ColorPack> Palette) ExtractColors(SixLabors.ImageSharp.Image image) {
        using var cloned = image.CloneAs<Rgba32>();
        var quantizer = new OctreeQuantizer(new QuantizerOptions { MaxColors = 8 });

        cloned.Mutate(x => x.Quantize(quantizer));

        var colorPacks = new List<ColorPack>();
        var pixelRow = cloned.Frames.RootFrame.PixelBuffer.DangerousGetRowSpan(0);
        foreach (var c in pixelRow.ToArray().Take(8)) {
            if (c.A > 0) {
                var color32 = new Color32(c.R, c.G, c.B, c.A);
                colorPacks.Add(new ColorPack { Color = color32, Lab = color32.ToLabColor() });
            }
        }

        var primary = colorPacks.Count > 0
            ? colorPacks[0]
            : new ColorPack {
                Color = new Color32(128, 128, 128, 255),
                Lab = new LabColor(53.39, 0, 0)
            };
        var secondary = colorPacks.Count > 1 ? colorPacks[1] : primary;

        return (primary, secondary, colorPacks);
    }
}
