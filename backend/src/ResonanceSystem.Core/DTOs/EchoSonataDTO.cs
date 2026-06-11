using System.Text.Json.Serialization;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoSonataDTO {
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong ID { get; init; }
    public required string Name { get; init; }
    public required IList<EchoDTO> Echoes { get; init; } = [];
}

public static class EchoSonataDTOExtensions {
    public static EchoSonataDTO ToDTO(this EchoSonata e) {
        return new EchoSonataDTO {
            ID = e.Id,
            Name = e.Name,
            Echoes = e.Echoes.Select(e => e.ToDTO()).ToList()
        };
    }

    public static EchoSonata ToDatabase(this EchoSonataDTO dto) {
        return new EchoSonata(dto.Name) {
            Echoes = dto.Echoes.Select(e => e.ToDatabase()).ToList()
        };
    }
}
