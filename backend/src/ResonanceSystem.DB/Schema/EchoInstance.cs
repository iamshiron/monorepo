using System.ComponentModel.DataAnnotations;

namespace Shiron.ResonanceSystem.DB.Schema;

public class EchoInstance {
    public Guid ID { get; set; } = Guid.CreateVersion7();

    [MaxLength(64)] public required string Name { get; set; }
    public required int Level { get; set; }

    public required EchoCost Cost { get; set; }
    public required MainStatType MainStatType { get; set; }
    public required decimal MainStatValue { get; set; }

    public IList<EchoSubStat> SubStats { get; set; } = [];

    public int Index { get; set; }
    public Guid CharacterInstanceID { get; set; }
    public CharacterInstance CharacterInstance { get; set; } = null!;
}
