using System.ComponentModel.DataAnnotations;
using Shiron.Lib.Types;

namespace Shiron.HonamiSystem.Schema;

using CarEntity = Car;

public class ColorPack {
    public required Color32 Color { get; set; }
    public required LabColor Lab { get; set; }
}

public class Image : BaseEntity {
    [MaxLength(63)] public required string Bucket { get; set; }
    [MaxLength(255)] public required string ObjectKey { get; set; }
    public required int Width { get; set; }
    public required int Height { get; set; }
    [MaxLength(1024)] public required string BlurHash { get; set; }

    public required ColorPack PrimaryColor { get; set; }
    public required ColorPack SecondaryColor { get; set; }

    public IList<ColorPack> Palette { get; set; } = [];

    // Foreign Keys
    public Character? Character { get; set; }
    public Guid? CharacterID { get; set; }

    public IList<CarEntity> Cars { get; set; } = [];
}
