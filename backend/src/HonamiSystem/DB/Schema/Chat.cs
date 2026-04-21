using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class Chat : BaseEntity {
    [MaxLength(64)] public required string Title { get; set; }
    [MaxLength(256)] public string? Description { get; set; }

    public Guid? ChatGroupID { get; set; }
    public ChatGroup? ChatGroup { get; set; }

    public IList<Memory> Memories { get; set; } = [];
    public IList<ChatMessage> Messages { get; set; } = [];
    public IList<MessageAttachment> Attachments { get; set; } = [];
    public IList<ChatParticipantUser> UserParticipants { get; set; } = [];
    public IList<ChatParticipantAgent> AgentParticipants { get; set; } = [];

    public Guid CreatedByID { get; set; }
    public User CreatedBy { get; set; } = null!;
}
