using System.ComponentModel.DataAnnotations;
using System.IO.Hashing;
using System.Text;

namespace Shiron.ResonanceSystem.DB.Schema;

public class Character(string name) {
    public ulong Id { get; set; } = XxHash3.HashToUInt64(Encoding.UTF8.GetBytes(name));
    [MaxLength(32)] public string Name { get; set; } = name;
    public required Attribute Attribute { get; set; }
    public required WeaponType WeaponType { get; set; }
    public required Rarity Rarity { get; set; }
}
