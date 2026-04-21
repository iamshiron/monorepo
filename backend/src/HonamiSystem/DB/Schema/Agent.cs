using System.ComponentModel.DataAnnotations;

namespace Shiron.HonamiSystem.DB.Schema;

public class Agent : BaseEntity {
    [MaxLength(256)] public required string Name { get; set; }
    [MaxLength(256)] public string? Description { get; set; }

    public Guid? PersonaID { get; set; }
    public Persona? Persona { get; set; }
    public IList<Memory> Memories { get; set; } = [];
    public IList<string> RequiredTools { get; set; } = [];
    public IList<string> SuggestedTools { get; set; } = [];

    public Guid CreatedByID { get; set; }
    public User CreatedBy { get; set; } = null!;
}
