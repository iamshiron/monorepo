namespace Shiron.ResonanceSystem.Core.DTOs;

public record OCRResultDTO {
    public required string Text { get; init; }
    public required float Confidence { get; init; }
}
