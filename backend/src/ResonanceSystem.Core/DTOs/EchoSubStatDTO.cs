using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoSubStatDTO {
    public required SubStatType Type { get; set; }
    public required decimal Value { get; set; }
    public required int Index { get; set; }
}

public static class EchoSubStatDTOExtensions {
    public static EchoSubStat ToDatabase(this EchoSubStatDTO dto) {
        return new EchoSubStat {
            Type = dto.Type,
            Value = dto.Value,
            Index = dto.Index
        };
    }

    public static EchoSubStatDTO ToDTO(this EchoSubStat data) {
        return new EchoSubStatDTO {
            Type = data.Type,
            Value = data.Value,
            Index = data.Index
        };
    }
}
