namespace Shiron.ResonanceSystem.DB.Schema;

public class EchoSonata {
    public Guid ID { get; set; } = Guid.CreateVersion7();

    public required string Name { get; set; }

    public IList<Echo> Echoes { get; set; } = [];
}
