using System.Security.Claims;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Shiron.ResonanceSystem.API.Exceptions;
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
    private static readonly int[] EchoCostXValues = [328, 705, 1082, 1455, 1828];

    [GeneratedRegex("[\\w\\s]+\\w", RegexOptions.IgnoreCase)]
    private static partial Regex NameRegex();

    [GeneratedRegex("([\\w \\.]+) (\\d+(?:\\.\\d)?%?)", RegexOptions.IgnoreCase)]
    private static partial Regex StatRegex();

    [GeneratedRegex("(\\d{1,2}(?:\\.\\d)?)%?")]
    private static partial Regex StatValueRegex();

    [GeneratedRegex("[0-9]")]
    private static partial Regex SingleDigitRegex();

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
        [FromQuery] byte threshold = 128) {
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

        try {
            return Results.Ok(new {
                ResonatorName = ParseResonatorName(image, ocr),
                Echoes = await ParseEchoes(image, ocr).ToListAsync(ct)
            });
        } catch (OCRException ex) {
            return Results.BadRequest(new { Error = ex.Message, RawData = ex.RawData });
        }
    }

    private static async IAsyncEnumerable<AddEchoDTO> ParseEchoes(SKBitmap image, IOCRService ocr) {
        for (var i = 0; i < 5; ++i) {
            var echoCost = ParseEchoCost(image, ocr, i);
            var (mainStatType, mainStatValue) = ParseEchoMainStat(image, ocr, i);
            var subStats = ParseEchoSubStats(image, ocr, i);

            yield return new AddEchoDTO {
                Cost = echoCost,
                Level = 0,
                MainStatType = mainStatType,
                MainStatValue = mainStatValue,
                Name = $"Echo {i + 1}",
                Index = i,
                SubStats = subStats
            };
        }
    }

    private static EchoCost ParseEchoCost(SKBitmap image, IOCRService ocr, int index) {
        if (index < 0 || index > 4)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 4");

        var res = ocr.Process(image, new Rect(EchoCostXValues[index], 675, 31, 27), PageSegMode.SingleChar);
        if (res is null)
            throw new EchoCostParseException(index, "(no OCR result)");

        var cost = EchoCostHelper.CostFromString(res.Text);
        if (cost is null)
            throw new EchoCostParseException(index, res.Text);

        return cost.Value;
    }

    private static string ParseResonatorName(SKBitmap image, IOCRService ocr) {
        var res = ocr.Process(image, Rect.FromCoords(70, 26, 1000, 86));
        if (res is null)
            throw new ResonatorNameParseException("(no OCR result)");

        var name = NameRegex().Match(res.Text);
        if (!name.Success)
            throw new ResonatorNameParseException(res.Text);

        return name.Value;
    }

    private static EchoSubStatDTO[] ParseEchoSubStats(SKBitmap image, IOCRService ocr, int index) {
        if (index < 0 || index > 4)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 4");

        var area = new Rect(EchoSubStatsXValues[index], 880, 310, 160);
        var res = ocr.Process(image, area, PageSegMode.SingleBlock);
        if (res is null)
            throw new EchoSubStatParseException(index, "(no OCR result)");

        var subStats = new List<EchoSubStatDTO>();

        foreach (var line in res.Text.Split("\n")) {
            var match = StatRegex().Match(line);
            if (!match.Success) continue;

            var statName = match.Groups[1].Value.Trim();
            var statValueString = match.Groups[2].Value.Trim();

            var statType = EchoSubStatHelper.SubStatFromString(statName, statValueString.Contains('%'));
            if (statType is null) continue;

            var statValue = decimal.Parse(StatValueRegex().Match(statValueString).Groups[1].Value);
            subStats.Add(new EchoSubStatDTO {
                Type = statType.Value,
                Value = statValue,
                Index = subStats.Count
            });
        }

        if (subStats.Count == 0)
            throw new EchoSubStatParseException(index, res.Text);

        return [.. subStats];
    }

    private static (MainStatType Type, decimal Value) ParseEchoMainStat(SKBitmap image, IOCRService ocr, int index) {
        if (index < 0 || index > 4)
            throw new ArgumentOutOfRangeException(nameof(index), index, "Index must be between 0 and 4");

        var statNameRes = ocr.Process(image, new Rect(EchoMainStatsXValues[index], 726, 185, 21), PageSegMode.SingleLine);
        if (statNameRes is null)
            throw new EchoMainStatParseException(index, "(no OCR result for stat name)");

        var statValueRes = ocr.Process(image, new Rect(EchoMainStatsXValues[index], 753, 165, 28), PageSegMode.SingleLine);
        if (statValueRes is null)
            throw new EchoMainStatParseException(index, $"(stat name: '{statNameRes.Text}', no OCR result for stat value)");

        var rawData = $"{statNameRes.Text} {statValueRes.Text}";

        var statType = EchoMainStatHelper.MainStatFromString(statNameRes.Text.Trim());
        if (statType is null)
            throw new EchoMainStatParseException(index, rawData);

        var statValue = decimal.Parse(StatValueRegex().Match(statValueRes.Text).Groups[1].Value);
        return (statType.Value, statValue);
    }

    private static bool IsValidImageSignature(byte[] bytes) {
        if (bytes == null || bytes.Length < 8)
            return false;

        if (bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return true;

        if (bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A && bytes[6] == 0x1A && bytes[7] == 0x0A)
            return true;

        return false;
    }
}
