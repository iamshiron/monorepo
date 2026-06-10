using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Shiron.ResonanceSystem.Core;
using Shiron.ResonanceSystem.Core.DTOs;
using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;
using Shiron.ResonanceSystem.Services;
using SkiaSharp;
using Tesseract;

namespace Shiron.ResonanceSystem.API.Endpoints;

public static partial class ScanEndpoints {
    private static readonly int[] EchoSubStatsXValues = [64, 443, 815, 1190, 1565];
    private static readonly int[] EchoMainStatsXValues = [215, 587, 962, 1337, 1711];

    [GeneratedRegex("[\\w\\s]+\\w", RegexOptions.IgnoreCase)]
    private static partial Regex NameRegex();

    [GeneratedRegex("([\\w \\.]+) (\\d+(?:\\.\\d)?%?)", RegexOptions.IgnoreCase)]
    private static partial Regex StatRegex();

    [GeneratedRegex("(\\d{1,2}(?:\\.\\d)?)%?")]
    private static partial Regex StatValueRegex();

    public static void MapScanEndpoints(this IEndpointRouteBuilder endpoints) {
        var group = endpoints.MapGroup("/scan").WithTags("Scan");

        group.MapPost("/wuwa-bot/{id}", ScanWuWaBotImage).DisableAntiforgery().Produces<OCRResultDTO>();
    }

    private static async Task<IResult> ScanWuWaBotImage(
        IFormFile file,
        IOCRService ocr,
        IImageProcessingService imageProcessing,
        ResSystemDbContext db,
        ClaimsPrincipal principal,
        CancellationToken ct,
        [FromQuery] byte threshold = 80) {
        var userID = IdentityUtils.GetUserID(principal);
        if (userID == null) return Results.Unauthorized();
        if (file == null || file.Length == 0) return Results.BadRequest("No file provided");
        if (file.Length > 10 * 1024 * 1024) return Results.BadRequest("File too large");

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream, ct);
        var buffer = stream.ToArray();
        if (!IsValidImageSignature(buffer)) return Results.BadRequest("Invalid image file.");

        using var image = imageProcessing.BinarizeFromBytes(buffer, threshold);
        if (image == null) return Results.BadRequest("Invalid image file.");

        var res = ocr.Process(image);
        if (res == null) return Results.BadRequest("Failed to process image");
        return Results.Ok(new {
            ResonatorName = ExtractResonatorName(image, ocr),
            EchoSubStats = ExtractEchoSubStats(image, ocr),
            EchoMainStats = ParseMainStat(image, ocr)
        });
    }

    private static string? ExtractResonatorName(SKBitmap image, IOCRService ocr) {
        var res = ocr.Process(image, Rect.FromCoords(70, 26, 1000, 86));
        if (res == null) return null;
        var name = NameRegex().Match(res.Text);
        if (!name.Success) return null;
        return name.Value;
    }

    private static List<object> ExtractEchoSubStats(SKBitmap image, IOCRService ocr) {
        var result = new List<object>(5);

        for (var i = 0; i < EchoSubStatsXValues.Length; ++i) {
            var area = new Rect(EchoSubStatsXValues[i], 880, 310, 160);
            var res = ocr.Process(image, area, PageSegMode.SingleBlock);
            if (res == null) continue;

            var echoSubStats = new List<EchoSubStatDTO>(5);
            var lines = res.Text.Split("\n");
            foreach (var line in lines) {
                Console.WriteLine($"Trying to match {line}");

                var match = StatRegex().Match(line);
                if (match.Success) {
                    var statName = match.Groups[1].Value.Trim();
                    var statValueString = match.Groups[2].Value.Trim();

                    var statType = EchoSubStatHelper.SubStatFromString(statName, statValueString.Contains('%'));
                    if (statType == null) {
                        Console.WriteLine($"Unable to parse stat: {statName} {statValueString}");
                        continue;
                    }

                    var statValue = decimal.Parse(StatValueRegex().Match(statValueString).Groups[1].Value);
                    var stat = new EchoSubStatDTO {
                        Type = statType.Value,
                        Value = statValue,
                        Index = 0
                    };

                    echoSubStats.Add(stat);
                }
            }

            result.Add(echoSubStats);
        }

        return result;
    }

    private static List<Tuple<MainStatType, decimal>> ParseMainStat(SKBitmap image, IOCRService ocr) {
        var result = new List<Tuple<MainStatType, decimal>>(5);
        for (var i = 0; i < EchoMainStatsXValues.Length; ++i) {
            var statNameRes = ocr.Process(image, new Rect(EchoMainStatsXValues[i], 726, 185, 21), PageSegMode.SingleLine);
            if (statNameRes == null) continue;
            var statValueRes = ocr.Process(image, new Rect(EchoMainStatsXValues[i], 753, 165, 28), PageSegMode.SingleLine);
            if (statValueRes == null) continue;

            var statValue = decimal.Parse(StatValueRegex().Match(statValueRes.Text).Groups[1].Value);
            var statType = EchoMainStatHelper.MainStatFromString(statNameRes.Text.Trim(), true);
            if (statType.HasValue) {
                result.Add(new Tuple<MainStatType, decimal>(statType.Value, statValue));
            }
        }
        return result;
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
