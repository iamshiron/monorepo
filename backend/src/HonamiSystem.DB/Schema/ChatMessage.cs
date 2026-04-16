using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class ChatMessage : BaseEntity {
    public required Guid ChatID { get; set; }
    public required Chat Chat { get; set; }
    public Guid? AgentID { get; set; }
    public Agent? Agent { get; set; }
    public Guid? UserID { get; set; }
    public User? User { get; set; }

    [MaxLength(1024)] public required string Content { get; set; }
    public Guid? ParentMessageID { get; set; }
    public ChatMessage? ParentMessage { get; set; }

    public IList<MessageAttachment> Attachments { get; set; } = [];

    public bool IsUser => UserID.HasValue;
    public bool IsAgent => AgentID.HasValue;
    public bool IsSystem => !IsUser && !IsAgent;
}
