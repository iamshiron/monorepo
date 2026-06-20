using Shiron.Mutils.API.Services.Impl;
using Xunit;

namespace Shiron.Mutils.Tests.Unit;

public class MudaeParserTests {
    private readonly MudaeParser _parser = new();

    [Fact]
    public void ParseCollection_WithValidData_ReturnsParsedCharacters() {
        var data = """
                   #72 - Gawr Gura => 67 al, 118 img + 12 gif, 14 series · :bronzekey:   (1) 792 ka - https://mudae.net/uploads/test.png
                   #117 - Itsuki Nakano => 4 al, 58 img + 3 gif, 5 series 647 ka - https://mudae.net/uploads/test2.png
                   #144 - Hitori Gotou => 36 al, 54 img + 8 gif, 2 series 589 ka
                   """;

        var result = _parser.ParseCollection(data).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("Gawr Gura", result[0].Name);
        Assert.Equal(72, result[0].Rank);
        Assert.Equal(792, result[0].Kakera);
        Assert.Equal("bronzekey", result[0].KeyType);
        Assert.Equal("Itsuki Nakano", result[1].Name);
        Assert.Equal(117, result[1].Rank);
        Assert.Equal("Hitori Gotou", result[2].Name);
    }

    [Fact]
    public void ParseCollection_WithEmptyLines_SkipsThem() {
        var data = """
                   #72 - Gawr Gura => 67 al, 118 img + 12 gif, 14 series 792 ka

                   #117 - Itsuki Nakano => 4 al, 58 img + 3 gif, 5 series 647 ka

                   """;

        var result = _parser.ParseCollection(data).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void ParseCollection_WithKeyData_ParsesKeyInfo() {
        var data = "#497 - Keqing => 8 al, 49 img + 8 gif · :goldkey:   (8) 446 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Keqing", result[0].Name);
        Assert.Equal("goldkey", result[0].KeyType);
        Assert.Equal(8, result[0].KeyCount);
    }

    [Fact]
    public void ParseCollection_WithoutKeyData_ReturnsNullKey() {
        var data = "#333 - Kafka => 4 al, 33 img + 3 gif, 2 series 376 ka";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Null(result[0].KeyType);
        Assert.Null(result[0].KeyCount);
    }

    [Fact]
    public void ParseCollection_WithEmptyData_ReturnsEmpty() {
        var result = _parser.ParseCollection("").ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void ParseCollection_WithMinimalData_ParsesCorrectly() {
        var data = "#257 - Godzilla => 41 al, 124 img + 5 gif, 50 series 435 ka";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Godzilla", result[0].Name);
        Assert.Equal(257, result[0].Rank);
        Assert.Equal(41, result[0].Claims);
        Assert.Equal(124, result[0].Images);
        Assert.Equal(5, result[0].Gifs);
        Assert.Equal(50, result[0].SeriesCount);
        Assert.Equal(435, result[0].Kakera);
    }

    [Fact]
    public void ParseCollection_WithCommaFormattedNumbers_ParsesCorrectly() {
        var data = """
                   #2,092 - Skirk => 6 al, 29 img + 5 gif · :bronzekey:   (1) 125 ka - https://mudae.net/uploads/test.png
                   #2,508 - Fu Xuan => 10 al, 24 img + 3 gif · :silverkey:   (3) 123 ka - https://mudae.net/uploads/test2.png
                   """;

        var result = _parser.ParseCollection(data).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("Skirk", result[0].Name);
        Assert.Equal(2092, result[0].Rank);
        Assert.Equal(125, result[0].Kakera);
        Assert.Equal("bronzekey", result[0].KeyType);
        Assert.Equal("Fu Xuan", result[1].Name);
        Assert.Equal(2508, result[1].Rank);
        Assert.Equal("silverkey", result[1].KeyType);
    }

    [Fact]
    public void ParseCollection_WithSpValue_ParsesCorrectly() {
        var data = "#4,765 - Chisa => 3 al, 40 img + 3 gif · :silverkey:   (3) 88 ka 600 sp - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Chisa", result[0].Name);
        Assert.Equal(4765, result[0].Rank);
        Assert.Equal(88, result[0].Kakera);
        Assert.Equal(600, result[0].Sp);
        Assert.Equal("silverkey", result[0].KeyType);
        Assert.Equal(3, result[0].KeyCount);
        Assert.Equal("https://mudae.net/uploads/test.png", result[0].ImageUrl);
    }

    [Fact]
    public void ParseCollection_WithoutSeries_ParsesCorrectly() {
        var data = "#153 - Raiden Shogun => 29 al, 50 img + 8 gif 571 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Raiden Shogun", result[0].Name);
        Assert.Null(result[0].SeriesCount);
        Assert.Equal(571, result[0].Kakera);
    }

    [Fact]
    public void ParseCollection_WithOnlyImagesNoGifs_ParsesCorrectly() {
        var data = "#261 - Monika => 5 al, 42 img  431 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Monika", result[0].Name);
        Assert.Equal(42, result[0].Images);
        Assert.Null(result[0].Gifs);
        Assert.Null(result[0].SeriesCount);
    }

    [Fact]
    public void ParseCollection_WithZeroClaims_ParsesCorrectly() {
        var data = "#1,640 - Pucca => 0 al, 4 img + 1 gif 144 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Pucca", result[0].Name);
        Assert.Equal(0, result[0].Claims);
        Assert.Equal(4, result[0].Images);
        Assert.Equal(1, result[0].Gifs);
    }

    [Fact]
    public void ParseCollection_WithParenthesesInName_ParsesCorrectly() {
        var data = "#1,206 - Robin (HSR) => 11 al, 53 img + 9 gif 173 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Robin (HSR)", result[0].Name);
    }

    [Fact]
    public void ParseCollection_WithHighRank_ParsesCorrectly() {
        var data = "#10,278 - Zarya => 10 al, 27 img , 4 series 56 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Zarya", result[0].Name);
        Assert.Equal(10278, result[0].Rank);
    }

    [Fact]
    public void ParseCollection_WithNoKeyButWithSeries_ParsesCorrectly() {
        var data = "#184 - Ishtar => 15 al, 42 img + 5 gif, 3 series 520 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Ishtar", result[0].Name);
        Assert.Equal(3, result[0].SeriesCount);
        Assert.Null(result[0].KeyType);
    }

    [Fact]
    public void ParseCollection_WithBronzeKeyAndSp_ParsesCorrectly() {
        var data = "#2,082 - Kiriko => 9 al, 74 img + 3 gif, 4 series · :bronzekey:   (1) 125 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Kiriko", result[0].Name);
        Assert.Equal("bronzekey", result[0].KeyType);
        Assert.Equal(1, result[0].KeyCount);
        Assert.Equal(4, result[0].SeriesCount);
    }

    [Fact]
    public void ParseCollection_WithSilverKey_ParsesCorrectly() {
        var data = "#377 - Mizuki Akiyama => 4 al, 119 img + 5 gif, 2 series · :silverkey:   (3) 384 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Mizuki Akiyama", result[0].Name);
        Assert.Equal("silverkey", result[0].KeyType);
        Assert.Equal(3, result[0].KeyCount);
    }

    [Fact]
    public void ParseCollection_WithNoImagesField_ParsesCorrectly() {
        var data = "#1,200 - Fami => 11 al, 3 img  173 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Fami", result[0].Name);
        Assert.Equal(11, result[0].Claims);
        Assert.Equal(3, result[0].Images);
        Assert.Null(result[0].Gifs);
    }

    [Fact]
    public void ParseCollection_WithGoldKey_ParsesCorrectly() {
        var data = "#2,854 - Sandrone => 4 al, 18 img + 1 gif · :goldkey:   (7) 146 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Sandrone", result[0].Name);
        Assert.Equal("goldkey", result[0].KeyType);
        Assert.Equal(7, result[0].KeyCount);
    }

    [Fact]
    public void ParseCollection_WithChaosKey_ParsesCorrectly() {
        var data = "#500 - Test Character => 5 al, 20 img · :goldkey:   (10) 500 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("chaoskey", result[0].KeyType);
        Assert.Equal(10, result[0].KeyCount);
    }

    [Fact]
    public void ParseCollection_KeyTypeComputedFromKeyCount() {
        var testData = new[] {
            (1, "bronzekey"),
            (2, "bronzekey"),
            (3, "silverkey"),
            (5, "silverkey"),
            (6, "goldkey"),
            (9, "goldkey"),
            (10, "chaoskey"),
            (500, "chaoskey")
        };

        foreach (var (keyCount, expectedKeyType) in testData) {
            var data = $"#100 - Test => 5 al, 10 img · :bronzekey:   ({keyCount}) 100 ka";
            var result = _parser.ParseCollection(data).ToList();
            Assert.Single(result);
            Assert.Equal(expectedKeyType, result[0].KeyType);
            Assert.Equal(keyCount, result[0].KeyCount);
        }
    }

    [Fact]
    public void ParseCollection_WithVeryHighRank_ParsesCorrectly() {
        var data = "#85,224 - Chinami Komuro => 1 al, 9 img  · :bronzekey:   (2) 35 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Chinami Komuro", result[0].Name);
        Assert.Equal(85224, result[0].Rank);
    }

    [Fact]
    public void ParseCollection_WithCommaInKakera_ParsesCorrectly() {
        var data = "#1,024 - Chise Hatori => 5 al, 63 img + 4 gif, 2 series · :bronzekey:   (1) 191 ka - https://mudae.net/uploads/test.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Chise Hatori", result[0].Name);
        Assert.Equal(1024, result[0].Rank);
        Assert.Equal(191, result[0].Kakera);
    }

    [Fact]
    public void ParseCollection_WithSpecialCharactersInName_ParsesCorrectly() {
        var data = "#2,082 - Kiriko => 9 al, 74 img + 3 gif, 4 series · :bronzekey:   (1) 125 ka - https://mudae.net/uploads/4098338/xCgtuec~IiKGPud.png";

        var result = _parser.ParseCollection(data).ToList();

        Assert.Single(result);
        Assert.Equal("Kiriko", result[0].Name);
        Assert.Contains("xCgtuec~IiKGPud.png", result[0].ImageUrl);
    }

    [Fact]
    public void ParseCollection_WithFullTestData_ParsesAllLines() {
        var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "collection-sample.txt");
        var data = File.ReadAllText(dataPath);
        var lines = File.ReadAllLines(dataPath).Count(l => !string.IsNullOrWhiteSpace(l));

        var result = _parser.ParseCollection(data).ToList();

        Assert.Equal(lines, result.Count);
    }
}
