using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core;

public static class EchoSubStatHelper {
    public static SubStatType? SubStatFromString(string type, bool isPercent) {
        type = type.ToLower();

        if (type.Contains("crit")) {
            return type.Contains("dmg") || type.Contains("bmg") ? SubStatType.CritDMG : SubStatType.CritRate;
        }
        if (type.Contains("energy") || type.Contains("regen")) {
            return SubStatType.EnergyRegen;
        }

        switch (type) {
            case "atk":
                return isPercent ? SubStatType.AttackPercent : SubStatType.Attack;
            case "def":
                return isPercent ? SubStatType.Defence : SubStatType.DefencePercent;
            case "hp":
                return isPercent ? SubStatType.HPPercent : SubStatType.HP;
        }

        if (type.Contains("basic")) return SubStatType.BasicAttackDMG;
        if (type.Contains("heavy")) return SubStatType.HeavyAttackDMG;
        if (type.Contains("resonance skill")) return SubStatType.ResonanceSkillDMG;
        if (type.Contains("resonance lib")) return SubStatType.ResonanceLiberationDMG;
        return null;
    }
}
