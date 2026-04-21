namespace Shiron.HonamiSystem.DB.Schema;

public class ChatParticipantAgent {
    public required Guid AgentID { get; set; }
    public required Agent Agent { get; set; }

    public required Guid ChatID { get; set; }
    public required Chat Chat { get; set; }

    public IList<string> AllowedTools { get; set; } = [];
}
