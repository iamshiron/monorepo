using System.Text.Json.Serialization;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record CreateCharacterInstanceDTO {
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong CharacterID { get; set; }

    public required int SequenceChain { get; set; }
    public required int Level { get; set; }

    public required int Forte0Level { get; set; }
    public required int Forte1Level { get; set; }
    public required int Forte2Level { get; set; }
    public required int Forte3Level { get; set; }
    public required int Forte4Level { get; set; }

    public IList<EchoInstanceDTO> Echoes { get; set; } = [];
}

public record CharacterInstanceDTO : CreateCharacterInstanceDTO {
    public Guid ID { get; set; } = Guid.CreateVersion7();
    public required CharacterDTO Character { get; set; }
}

public static class CharacterInstanceDTOExtensions {
    public static CharacterInstanceDTO ToDTO(this CharacterInstance data) {
        return new CharacterInstanceDTO {
            ID = data.ID,
            Character = data.Character.ToDTO(),
            CharacterID = data.CharacterID,
            SequenceChain = data.SequenceChain,
            Level = data.Level,
            Forte0Level = data.Forte0Level,
            Forte1Level = data.Forte1Level,
            Forte2Level = data.Forte2Level,
            Forte3Level = data.Forte3Level,
            Forte4Level = data.Forte4Level,
            Echoes = data.EchoInstances.Select(e => e.ToDTO()).ToList()
        };
    }
}
