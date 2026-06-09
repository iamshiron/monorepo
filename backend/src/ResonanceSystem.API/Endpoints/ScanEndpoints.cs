using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.Services;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static class ScanEndpoints {
    public static void MapScanEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/scan").WithTags("Scan");

        group.MapPost("/wuwa-bot/{id}", ScanWuWaBotImage).DisableAntiforgery().Produces<OCRResultDTO>();
    }

    private static async Task<IResult> ScanWuWaBotImage(
        IFormFile file,
        IOCRService ocr,
        ResSystemDbContext db,
        ClaimsPrincipal principal,
        CancellationToken ct,
        [FromQuery] string id) {
        var userID = IdentityUtils.GetUserID(principal);
        if (userID == null) return Results.Unauthorized();
        if (file == null || file.Length == 0) return Results.BadRequest("No file provided");
        if (file.Length > 10 * 1024 * 1024) return Results.BadRequest("File too large");

        if (!ulong.TryParse(id, out var resonatorID)) return Results.BadRequest("Invalid resonator ID");
        var resonator = db.Characters.FirstOrDefault(c => c.Id == resonatorID);
        if (resonator == null) return Results.BadRequest("Resonator not found");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        var buffer = stream.ToArray();
        if (!IsValidImageSignature(buffer)) return Results.BadRequest("Invalid image file. Allowed formats: JPEG, PNG");

        var res = ocr.Process(buffer);
        if (res == null) return Results.BadRequest("Failed to process image");
        return Results.Ok(res);
    }

    /// <summary>
    /// Checks the first few bytes of the array against known image file signatures.
    /// </summary>
    private static bool IsValidImageSignature(byte[] bytes) {
        if (bytes == null || bytes.Length < 8) {
            return false;
        }

        // JPEG Magic Number: FF D8 FF
        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF) {
            return true;
        }

        // PNG Magic Number: 89 50 4E 47 0D 0A 1A 0A
        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A) {
            return true;
        }

        return false;
    }
}
