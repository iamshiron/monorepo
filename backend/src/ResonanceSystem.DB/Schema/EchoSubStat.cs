namespace Shiron.ResonanceSystem.DB.Schema;

public class EchoSubStat {
    public Guid ID { get; set; } = Guid.CreateVersion7();
    public Guid EchoID { get; set; }
    public OwnedEcho Echo { get; set; } = null!;

    public int Index { get; set; }

    public SubStatType Type { get; set; }
    public decimal Value { get; set; }
}
