using FuzzySharp;
using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.Core;

public static class EchoCostHelper {
    private static readonly Dictionary<string, EchoCost> KnownCosts = new() {
        { "1", EchoCost.Cost1 },
        { "3", EchoCost.Cost3 },
        { "4", EchoCost.Cost4 }
    };

    public static EchoCost? CostFromString(string text) {
        if (string.IsNullOrWhiteSpace(text))
            return null;

        var normalized = text.ToUpperInvariant().Trim()
            // Misreads for '1'
            .Replace("I", "1")
            .Replace("L", "1")
            .Replace("!", "1")
            .Replace("|", "1")
            .Replace("T", "1")
            .Replace("7", "1")
            // Misreads for '3'
            .Replace("E", "3")
            .Replace("B", "3")
            .Replace("8", "3")
            // Misreads for '4'
            .Replace("A", "4")
            .Replace("H", "4");

        var match = Process.ExtractOne(
            normalized,
            KnownCosts.Keys,
            scorer: ScorerCache.Get<TokenSetScorer>()
        );

        if (match.Score < 75) {
            return null;
        }

        return KnownCosts[match.Value];
    }
}
