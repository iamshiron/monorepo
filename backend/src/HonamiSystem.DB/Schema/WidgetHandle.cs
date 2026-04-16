using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shiron.HonamiSystem.DB.Schema;

public class WidgetHandle : BaseEntity, IAttachable {
    [MaxLength(256)] public required string WidgetKey { get; set; }
    [Column(TypeName = "jsonb")] public required object Metadata { get; set; }

    public required Guid MessageID { get; set; }
    public required ChatMessage Message { get; set; }
}
