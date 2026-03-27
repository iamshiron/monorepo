namespace Shiron.Mutils.Core.Entities;

public sealed class SpherePerks : BaseEntity {
    public Guid CollectionEntryId { get; set; }
    public CollectionEntry CollectionEntry { get; set; } = null!;

    public int Perk1 { get; set; } = 0;
    public int Perk2 { get; set; } = 0;
    public int Perk3 { get; set; } = 0;
    public int Perk4 { get; set; } = 0;
    public int Perk5 { get; set; } = 0;
    public bool Perk6 { get; set; } = false;
    public bool Perk7 { get; set; } = false;
    public bool Perk8 { get; set; } = false;
    public bool Perk9 { get; set; } = false;
    public bool Perk10 { get; set; } = false;
}
