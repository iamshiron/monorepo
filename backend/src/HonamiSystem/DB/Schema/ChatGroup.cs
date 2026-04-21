using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class ChatGroup : BaseEntity {
    [MaxLength(64)] public required string Name { get; set; }

    public IList<Chat> Chats { get; set; } = [];
    public Guid CreatedByID { get; set; }
    public User CreatedBy { get; set; } = null!;
}
