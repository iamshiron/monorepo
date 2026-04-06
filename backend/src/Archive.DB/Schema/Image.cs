using Shiron.Lib.Types;

namespace Shiron.TheArchive.DB.Schema;

public record ColorPack(Color32 Color, LabColor Lab);

public class Image : BaseEntity {
    public required string Bucket { get; set; }
    public required string ObjectKey { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    public required string BlurHash { get; set; }

    public required ColorPack PrimaryColor { get; set; }
    public required ColorPack SecondaryColor { get; set; }

    public IList<ColorPack> Palette { get; set; } = [];

    // Foreign Keys
    public Character? Character { get; set; }
    public Guid CharacterID { get; set; }
}
