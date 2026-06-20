using Shiron.Mutils.API.Services.Impl;
using Xunit;

namespace Shiron.Mutils.Tests.Unit;

public class ExportServiceTests {
    private readonly ExportService _service = new();

    [Fact]
    public void ExportToCommaSeparated_ReturnsCommaSeparatedString() {
        var characters = new[] { "Char1", "Char2", "Char3" };

        var result = _service.ExportToCommaSeparated(characters);

        Assert.Equal("Char1, Char2, Char3", result);
    }

    [Fact]
    public void ExportToNewlineSeparated_ReturnsNewlineSeparatedString() {
        var characters = new[] { "Char1", "Char2", "Char3" };

        var result = _service.ExportToNewlineSeparated(characters);

        Assert.Equal("Char1\nChar2\nChar3", result);
    }

    [Fact]
    public void ExportToMudaeFormat_ReturnsCommaSeparatedString() {
        var characters = new[] { "Char1", "Char2" };

        var result = _service.ExportToMudaeFormat(characters);

        Assert.Equal("Char1, Char2", result);
    }

    [Fact]
    public void Export_WithEmptyCollection_ReturnsEmptyString() {
        var result = _service.ExportToCommaSeparated(Array.Empty<string>());
        Assert.Empty(result);
    }

    [Fact]
    public void Export_TrimsWhitespaceFromNames() {
        var characters = new[] { "  Char1  ", " Char2 " };

        var result = _service.ExportToCommaSeparated(characters);

        Assert.Equal("Char1, Char2", result);
    }
}
