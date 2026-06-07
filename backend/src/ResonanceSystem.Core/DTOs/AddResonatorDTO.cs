using System.Text.Json.Serialization;

namespace Shiron.ResonanceSystem.Core.DTOs;

public class AddResonatorDTO {
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong ResonatorID { get; set; }

    public required int SequenceChain { get; set; }
    public required int Level { get; set; }

    public required int Forte0Level { get; set; }
    public required int Forte1Level { get; set; }
    public required int Forte2Level { get; set; }
    public required int Forte3Level { get; set; }
    public required int Forte4Level { get; set; }

    public IList<EchoDTO> Echoes { get; set; } = [];
}
