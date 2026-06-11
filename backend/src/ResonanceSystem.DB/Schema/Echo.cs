namespace Shiron.ResonanceSystem.DB.Schema;

public class Echo {
    public Guid ID { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; }
    public required EchoCost Cost { get; set; }

    public IList<EchoSonata> Sonatas { get; set; } = [];
}
