using System.ComponentModel.DataAnnotations;
using Pgvector;

namespace Shiron.HonamiSystem.DB.Schema;

public class Memory : BaseEntity {
    [MaxLength(64)] public required string Key { get; set; }
    [MaxLength(255)] public required string Content { get; set; }
    public required Vector Embedding { get; set; }

    public required Guid AgentID { get; set; }
    public required Agent Agent { get; set; }

    // Chat is optional for local context
    public required Guid? ChatID { get; set; }
    public required Chat? Chat { get; set; }
}
