namespace Shiron.ResonanceSystem.DB.Schema;

public class OwnedCharacter {
    public Guid ID { get; set; } = Guid.CreateVersion7();

    public ulong CharacterID { get; set; }
    public Guid UserID { get; set; }
    public Character Character { get; set; }
    public User User { get; set; }

    public int SequenceChain { get; set; }
    public int Level { get; set; }

    public int Forte0Level { get; set; }
    public int Forte1Level { get; set; }
    public int Forte2Level { get; set; }
    public int Forte3Level { get; set; }
    public int Forte4Level { get; set; }

    public IList<OwnedEcho> Echoes { get; set; } = [];
}
