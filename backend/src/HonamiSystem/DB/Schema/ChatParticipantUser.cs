namespace Shiron.HonamiSystem.DB.Schema;

public class ChatParticipantUser {
    public required Guid UserID { get; set; }
    public required User User { get; set; }

    public required Guid ChatID { get; set; }
    public required Chat Chat { get; set; }
}
