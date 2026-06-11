using System.Text.Json.Serialization;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoDTO {
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong ID { get; init; }
    public required string Name { get; init; }
    public required EchoCost Cost { get; init; }
}

public static class EchoContentExtensions {
    public static EchoDTO ToDTO(this Echo e) {
        return new EchoDTO {
            ID = e.Id,
            Name = e.Name,
            Cost = e.Cost
        };
    }

    public static Echo ToDatabase(this EchoDTO dto) {
        return new Echo(dto.Name) {
            Cost = dto.Cost
        };
    }
}
