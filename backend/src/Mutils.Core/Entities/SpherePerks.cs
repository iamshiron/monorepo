namespace Shiron.Mutils.Core.Entities;

public sealed class SpherePerks : BaseEntity {
    public Guid CollectionEntryId { get; set; }
    public CollectionEntry CollectionEntry { get; set; } = null!;

    public int Perk1 { get; set; } = 0;
    public int Perk2 { get; set; } = 0;
    public int Perk3 { get; set; } = 0;
    public int Perk4 { get; set; } = 0;
    public int Perk5 { get; set; } = 0;
    public int Perk6 { get; set; } = 0;
    public int Perk7 { get; set; } = 0;
    public int Perk8 { get; set; } = 0;
    public int Perk9 { get; set; } = 0;
    public int Perk10 { get; set; } = 0;
}
