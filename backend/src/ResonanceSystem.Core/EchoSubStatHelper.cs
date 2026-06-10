using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core;

public static class EchoSubStatHelper {
    private static readonly Dictionary<string, SubStatType> KnownStats = new() {
        { "ATK", SubStatType.Attack },
        { "DEF", SubStatType.Defence },
        { "HP", SubStatType.HP },
        { "CRIT RATE", SubStatType.CritRate },
        { "CRIT DMG", SubStatType.CritDMG },
        { "ENERGY REGEN", SubStatType.EnergyRegen },
        { "BASIC ATTACK DMG BONUS", SubStatType.BasicAttackDMG },
        { "HEAVY ATTACK DMG BONUS", SubStatType.HeavyAttackDMG },
        { "RESONANCE SKILL DMG BONUS", SubStatType.ResonanceSkillDMG },
        { "RESONANCE LIBERATION DMG BONUS", SubStatType.ResonanceLiberationDMG }
    };

    public static SubStatType? SubStatFromString(string type, bool isPercent) {
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

        var baseStat = KnownStats[match.Value];
        if (isPercent) {
            return baseStat switch {
                SubStatType.Attack => SubStatType.AttackPercent,
                SubStatType.Defence => SubStatType.DefencePercent,
                SubStatType.HP => SubStatType.HPPercent,
                _ => baseStat
            };
        }

        return baseStat;
    }
}
