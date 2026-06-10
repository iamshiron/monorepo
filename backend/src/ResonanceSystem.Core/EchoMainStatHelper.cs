using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core;

public class EchoMainStatHelper {
    private static readonly Dictionary<string, MainStatType> KnownStats = new() {
        { "ATK", MainStatType.AttackPercent },
        { "DEF", MainStatType.DefencePercent },
        { "HP", MainStatType.HPPercent },
        { "GLACIO DMG BONUS", MainStatType.GlacioDMGPercent },
        { "SPECTRO DMG BONUS", MainStatType.SpectroDMGPercent },
        { "AERO DMG BONUS", MainStatType.AeroDMGPercent },
        { "ELECTRO DMG BONUS", MainStatType.ElectroDMGPercent },
        { "HAVOC DMG BONUS", MainStatType.HavocDMGPercent },
        { "FUSION DMG BONUS", MainStatType.FusionDMGPercent },
        { "ENERGY REGEN", MainStatType.EnergyRegen },
        { "CRIT RATE", MainStatType.CritRate },
        { "CRIT DMG", MainStatType.CritDMG },
        { "HEALING BONUS", MainStatType.HealingBonus }
    };

    public static MainStatType? MainStatFromString(string type) {
        if (string.IsNullOrWhiteSpace(type))
            return null;

        var normalized = type.ToUpperInvariant().Trim()
            .Replace("0", "O")
            .Replace("1", "I")
            .Replace("8", "B")
            .Replace("5", "S")
            .Replace("!", "I");

        var match = Process.ExtractOne(
            normalized,
            KnownStats.Keys,
            scorer: ScorerCache.Get<TokenSetScorer>()
        );

        if (match.Score < 75) {
            return null;
        }

        return KnownStats[match.Value];
    }
}
