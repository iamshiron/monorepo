using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class Skill : BaseEntity {
    [MaxLength(64)] public required string Name { get; set; }
    [MaxLength(255)] public required string Description { get; set; }

    [MaxLength(2048)] public required string Instruction { get; set; } = "";

    public required Guid CreatedByID { get; set; }
    public required User CreatedBy { get; set; }
}
