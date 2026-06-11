using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoInstanceDTO {
    public required string Name { get; set; }
    public required int Level { get; set; }
    public required EchoCost Cost { get; set; }
    public required MainStatType MainStatType { get; set; }
    public required decimal MainStatValue { get; set; }

    public int Index { get; set; } = 0;

    public IList<EchoSubStatDTO> SubStats { get; set; } = [];
}

public static class EchoInstanceDTOExtensions {
    public static EchoInstance ToDatabase(this EchoInstanceDTO dto) {
        return new EchoInstance {
            Name = dto.Name,
            Level = dto.Level,
            Cost = dto.Cost,
            MainStatType = dto.MainStatType,
            MainStatValue = dto.MainStatValue,
            Index = dto.Index,
            SubStats = dto.SubStats.Select(s => s.ToDatabase()).ToList()
        };
    }

    public static EchoInstanceDTO ToDTO(this EchoInstance data) {
        return new EchoInstanceDTO {
            Name = data.Name,
            Level = data.Level,
            Cost = data.Cost,
            MainStatType = data.MainStatType,
            MainStatValue = data.MainStatValue,
            Index = data.Index,
            SubStats = data.SubStats.Select(s => s.ToDTO()).ToList()
        };
    }
}
