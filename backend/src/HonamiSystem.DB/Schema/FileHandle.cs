using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public enum FileType {
    Text,
    Pdf,
    Markdown
}

public class FileHandle : BaseEntity, IAttachable, IObjectStored, ISummarizable {
    [MaxLength(256)] public required string ObjectKey { get; set; }
    [MaxLength(256)] public required string Summary { get; set; }
    public required decimal SizeKb { get; set; }
    public required FileType Type { get; set; }

    public required Guid CreatedByID { get; set; }
    public required User CreatedBy { get; set; }

    public required Guid MessageID { get; set; }
    public required ChatMessage Message { get; set; }
}
