using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record EchoDTO {
    public required string Name { get; set; }
    public required int Level { get; set; }
    public required EchoCost Cost { get; set; }
    public required MainStatType MainStatType { get; set; }
    public required int MainStatValue { get; set; }

    public int Index { get; set; } = 0;

    public IList<EchoSubStatDTO> SubStats { get; set; } = [];
}

public static class EchoDTOExtensions {
    public static OwnedEcho ToDatabase(this EchoDTO dto) {
        return new OwnedEcho {
            Name = dto.Name,
            Level = dto.Level,
            Cost = dto.Cost,
            MainStatType = dto.MainStatType,
            MainStatValue = dto.MainStatValue,
            Index = dto.Index,
            SubStats = dto.SubStats.Select(s => s.ToDatabase()).ToList()
        };
    }
}
