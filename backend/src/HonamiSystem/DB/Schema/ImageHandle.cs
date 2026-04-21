using System.ComponentModel.DataAnnotations;
using Shiron.Lib.Types;

namespace Shiron.HonamiSystem.DB.Schema;

public class ImageHandle : BaseEntity, IAttachable, IObjectStored, ISummarizable {
    [MaxLength(256)] public required string ObjectKey { get; set; }

    [MaxLength(256)] public required string BlurHash { get; set; }
    [MaxLength(256)] public required string Sha256Hash { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public decimal SizeKb { get; set; }
    public required ColorPack PrimaryColor { get; set; }
    public required ColorPack SecondaryColor { get; set; }
    public List<ColorPack> Palette { get; set; } = [];
    [MaxLength(256)] public required string Summary { get; set; }

    public required User CreatedBy { get; set; }
    public required Guid CreatedByID { get; set; }

    public required Guid MessageID { get; set; }
    public required ChatMessage Message { get; set; }
}
