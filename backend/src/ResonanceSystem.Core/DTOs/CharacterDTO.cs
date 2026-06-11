using System.Text.Json.Serialization;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core.DTOs;

public record CharacterDTO {
    [JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong ID { get; init; }
    public required string Hash { get; init; }
    public required string Name { get; init; }
    public required WeaponType Weapon { get; init; }
    public required Rarity Rarity { get; init; }
}

public static class CharacterDTOExtensions {
    public static CharacterDTO ToDTO(this Character c) {
        return new CharacterDTO {
            ID = c.Id,
            Hash = c.Id.ToString("X"),
            Name = c.Name,
            Weapon = c.WeaponType,
            Rarity = c.Rarity
        };
    }
}
