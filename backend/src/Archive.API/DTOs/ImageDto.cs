using Shiron.Lib.Types;

namespace Shiron.TheArchive.API.DTOs;

public record ColorPackDto {
    public required Color32Dto Color { get; init; }
    public required LabColorDto Lab { get; init; }
}

public record Color32Dto {
    public byte R { get; init; }
    public byte G { get; init; }
    public byte B { get; init; }
    public byte A { get; init; }

    public int ToRgba => R << 24 | G << 16 | B << 8 | A;
}

public record LabColorDto {
    public double L { get; init; }
    public double A { get; init; }
    public double B { get; init; }
}

public record ImageDto {
    public Guid ID { get; init; }
    public required string Bucket { get; init; }
    public required string ObjectKey { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string BlurHash { get; init; }
    public required ColorPackDto PrimaryColor { get; init; }
    public required ColorPackDto SecondaryColor { get; init; }
    public List<ColorPackDto> Palette { get; init; } = [];
    public List<Guid> CarIDs { get; init; } = [];
    public Guid? CharacterID { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record ImageCreateDto {
    public required string Bucket { get; init; }
    public required string ObjectKey { get; init; }
    public required int Width { get; init; }
    public required int Height { get; init; }
    public required string BlurHash { get; init; }
    public required ColorPackDto PrimaryColor { get; init; }
    public required ColorPackDto SecondaryColor { get; init; }
    public List<ColorPackDto>? Palette { get; init; }
    public List<Guid>? CarIDs { get; init; }
}

public record ImageUpdateDto {
    public string? Bucket { get; init; }
    public string? ObjectKey { get; init; }
    public int? Width { get; init; }
    public int? Height { get; init; }
    public string? BlurHash { get; init; }
    public ColorPackDto? PrimaryColor { get; init; }
    public ColorPackDto? SecondaryColor { get; init; }
    public List<ColorPackDto>? Palette { get; init; }
    public List<Guid>? CarIDs { get; init; }
}
