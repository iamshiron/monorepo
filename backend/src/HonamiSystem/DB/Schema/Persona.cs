using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class Persona : BaseEntity {
    [MaxLength(64)] public required string Name { get; set; }
    [MaxLength(256)] public string? Brief { get; set; }

    [MaxLength(8192)] public string Instruction { get; set; } = "";
    public IList<string> Traits { get; set; } = [];
    [MaxLength(512)] public string? SpeakingStyle { get; set; }

    public Guid CreatedByID { get; set; }
    public User CreatedBy { get; set; } = null!;
}
