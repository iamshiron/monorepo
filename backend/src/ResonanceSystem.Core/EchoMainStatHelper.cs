using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core;

public class EchoMainStatHelper {
    public static MainStatType? MainStatFromString(string type, bool isPercent) {
        type = type.ToLower();

        if (type.Contains("crit")) {
            return type.Contains("dmg") || type.Contains("bmg") ? MainStatType.CritDMG : MainStatType.CritRate;
        }

        switch (type) {
            case "atk":
                return MainStatType.AttackPercent;
            case "def":
                return MainStatType.DefencePercent;
            case "hp":
                return MainStatType.HPPercent;
            case "crit. rate":
                return MainStatType.CritRate;
            case "energy regen":
                return MainStatType.EnergyRegen;
        }

        if (type.Contains("fusion")) return MainStatType.FusionDMGPercent;
        if (type.Contains("glacio")) return MainStatType.GlacioDMGPercent;
        if (type.Contains("electro")) return MainStatType.ElectroDMGPercent;
        if (type.Contains("havoc")) return MainStatType.HavocDMGPercent;
        if (type.Contains("aero")) return MainStatType.AeroDMGPercent;
        if (type.Contains("spectro")) return MainStatType.SpectroDMGPercent;
        if (type.Contains("healing")) return MainStatType.HealingBonus;

        return null;
    }
}
