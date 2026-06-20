using Shiron.Mutils.API.Services.Impl;
using Shiron.Mutils.DB.Schema;
using Xunit;

namespace Shiron.Mutils.Tests.Unit;

public class KakeraLogParserTests {
    private readonly KakeraLogParser _parser = new();

    [Fact]
    public void ParseKakeraLog_WithBlueKakera_ParsesCorrectly() {
        var data = ":kakera:iamshiron +121 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Blue, result[0].Type);
        Assert.Equal(121, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithPurpleKakera_ParsesCorrectly() {
        var data = ":kakeraP:iamshiron +110 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Purple, result[0].Type);
        Assert.Equal(110, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithTealKakera_ParsesCorrectly() {
        var data = ":kakeraT:iamshiron +241 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Teal, result[0].Type);
        Assert.Equal(241, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithGreenKakera_ParsesCorrectly() {
        var data = ":kakeraG:iamshiron +283 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Green, result[0].Type);
        Assert.Equal(283, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithYellowKakera_ParsesCorrectly() {
        var data = ":kakeraY:iamshiron +458 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Yellow, result[0].Type);
        Assert.Equal(458, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithOrangeKakera_ParsesCorrectly() {
        var data = ":kakeraO:iamshiron +776 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Orange, result[0].Type);
        Assert.Equal(776, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithRedKakera_ParsesCorrectly() {
        var data = ":kakeraR:iamshiron +1,616 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Red, result[0].Type);
        Assert.Equal(1616, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithRainbowKakera_ParsesCorrectly() {
        var data = ":kakeraW:iamshiron +3,387 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Rainbow, result[0].Type);
        Assert.Equal(3387, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithLightKakera_ParsesCorrectly() {
        var data = ":kakeraL:breaks down into:kakera:+:kakera:+:kakeraG:+:kakera: => iamshiron +760 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Light, result[0].Type);
        Assert.Equal(760, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithChaosKakera_ParsesCorrectly() {
        var data = ":kakeraC:iamshiron +880 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Chaos, result[0].Type);
        Assert.Equal(880, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithDarkTransformingToOrange_ParsesAsDark() {
        var data = """
                   :kakeraD:turns into:kakeraO:
                   :kakeraO:iamshiron +800 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Dark, result[0].Type);
        Assert.Equal(800, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithDarkTransformingToPurple_ParsesAsDark() {
        var data = """
                   :kakeraD:turns into:kakeraP:
                   :kakeraP:(Free) iamshiron +110 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Dark, result[0].Type);
        Assert.Equal(110, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithMultipleLines_ParsesAll() {
        var data = """
                   :kakera:iamshiron +121 ($k)
                   :kakeraT:iamshiron +241 ($k)
                   :kakeraG:iamshiron +283 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal(KakeraType.Blue, result[0].Type);
        Assert.Equal(121, result[0].Value);
        Assert.Equal(KakeraType.Teal, result[1].Type);
        Assert.Equal(241, result[1].Value);
        Assert.Equal(KakeraType.Green, result[2].Type);
        Assert.Equal(283, result[2].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithCommaFormattedValue_ParsesCorrectly() {
        var data = ":kakeraR:iamshiron +1,616 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(1616, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithFreePrefix_ParsesCorrectly() {
        var data = ":kakeraP:(Free) iamshiron +110 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Purple, result[0].Type);
        Assert.Equal(110, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithInvalidLine_SkipsLine() {
        var data = """
                   :rollstack:
                   :kakera:iamshiron +121 ($k)
                   :kakeraC:+30 :sp:
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Blue, result[0].Type);
        Assert.Equal(121, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithEmptyData_ReturnsEmpty() {
        var result = _parser.ParseKakeraLog("").ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void ParseKakeraLog_WithRealWorldData_ParsesCorrectly() {
        var data = """
                   Mudae
                   APP
                    — 10/22/2025 7:06 PM
                   :kakera:iamshiron +121 ($k)
                   Mudae
                   APP
                    — 10/22/2025 12:28 PM
                   :kakera:iamshiron +142 ($k)
                   Mudae
                   APP
                    — 10/19/2025 6:46 PM
                   :kakera:iamshiron +147 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Equal(3, result.Count);
        Assert.All(result, c => Assert.Equal(KakeraType.Blue, c.Type));
        Assert.Equal(121, result[0].Value);
        Assert.Equal(142, result[1].Value);
        Assert.Equal(147, result[2].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithLightBreakdown_ParsesAsLight() {
        var data = ":kakeraL:breaks down into:kakeraP:+:kakera:+:kakeraT:+:kakeraP: => iamshiron +606 ($k)";

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Light, result[0].Type);
        Assert.Equal(606, result[0].Value);
    }

    [Fact]
    public void ParseKakeraLog_WithDateOnPreviousLine_ParsesDateCorrectly() {
        var data = """
                   Mudae
                   APP
                    — 10/22/2025 7:06 PM
                   :kakera:iamshiron +121 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Blue, result[0].Type);
        Assert.Equal(121, result[0].Value);
        Assert.NotNull(result[0].ClaimedAt);
        var claimedAt = result[0].ClaimedAt!.Value;
        Assert.Equal(10, claimedAt.Month);
        Assert.Equal(22, claimedAt.Day);
        Assert.Equal(2025, claimedAt.Year);
        Assert.Equal(19, claimedAt.Hour);
        Assert.Equal(6, claimedAt.Minute);
    }

    [Fact]
    public void ParseKakeraLog_WithYesterdayFormat_ParsesDateCorrectly() {
        var data = """
                   Mudae
                   APP
                    — Yesterday at 12:04 PM
                   :kakeraL:breaks down into:kakeraP:+:kakera:+:kakeraT:+:kakeraP: => iamshiron +606 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Single(result);
        Assert.Equal(KakeraType.Light, result[0].Type);
        Assert.Equal(606, result[0].Value);
        Assert.NotNull(result[0].ClaimedAt);
        var yesterday = DateTime.Today.AddDays(-1);
        var claimedAt = result[0].ClaimedAt!.Value;
        Assert.Equal(yesterday.Year, claimedAt.Year);
        Assert.Equal(yesterday.Month, claimedAt.Month);
        Assert.Equal(yesterday.Day, claimedAt.Day);
        Assert.Equal(12, claimedAt.Hour);
        Assert.Equal(4, claimedAt.Minute);
    }

    [Fact]
    public void ParseKakeraLog_WithMultipleDates_TracksDatePerClaim() {
        var data = """
                    — 10/22/2025 7:06 PM
                   :kakera:iamshiron +121 ($k)
                    — 10/21/2025 3:30 PM
                   :kakeraT:iamshiron +241 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(22, result[0].ClaimedAt!.Value.Day);
        Assert.Equal(21, result[1].ClaimedAt!.Value.Day);
    }

    [Fact]
    public void ParseKakeraLog_WithIsoDateFormat_ParsesDateCorrectly() {
        var data = """
                    — 2025-10-11 19:08
                   :kakera:iamshiron +163 ($k)
                   Mudae
                   APP
                    — 2025-10-09 16:41
                   :kakeraT:iamshiron +218 ($k)
                   """;

        var result = _parser.ParseKakeraLog(data).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal(KakeraType.Blue, result[0].Type);
        Assert.Equal(163, result[0].Value);
        Assert.NotNull(result[0].ClaimedAt);
        Assert.Equal(2025, result[0].ClaimedAt!.Value.Year);
        Assert.Equal(10, result[0].ClaimedAt!.Value.Month);
        Assert.Equal(11, result[0].ClaimedAt!.Value.Day);
        Assert.Equal(19, result[0].ClaimedAt!.Value.Hour);
        Assert.Equal(8, result[0].ClaimedAt!.Value.Minute);

        Assert.Equal(KakeraType.Teal, result[1].Type);
        Assert.Equal(218, result[1].Value);
        Assert.NotNull(result[1].ClaimedAt);
        Assert.Equal(2025, result[1].ClaimedAt!.Value.Year);
        Assert.Equal(10, result[1].ClaimedAt!.Value.Month);
        Assert.Equal(9, result[1].ClaimedAt!.Value.Day);
        Assert.Equal(16, result[1].ClaimedAt!.Value.Hour);
        Assert.Equal(41, result[1].ClaimedAt!.Value.Minute);
    }
}
