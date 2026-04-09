using Microsoft.EntityFrameworkCore;
using Shiron.Mutils.API.DTOs;
using Shiron.Mutils.API.DTos.API.Services;
using Shiron.Mutils.API.DTos.DB;
using Shiron.Mutils.DB.Schema;
using Shiron.Mutils.API.Helpers;

namespace Shiron.Mutils.API.Services.Impl;

public class OptimizerService(MutilsDbContext dbContext) : IOptimizerService {
    public async Task<OptimizerAnalysisResponse> AnalyzeAsync(
        Guid userId,
        OptimizerAnalysisRequest request,
        CancellationToken cancellationToken = default) {
        var entries = await dbContext.CollectionEntries
            .Include(e => e.Character)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

        var characters = entries.Select(e => e.Character).ToList();
        var totalCharacters = characters.Count;
        var totalKakera = characters.Sum(c => c.Kakera ?? 0);

        var keyDistribution = characters
            .Select(c => new { KeyType = KeyHelper.GetKeyTypeFromCount(c.KeyCount) })
            .Where(c => c.KeyType is not null)
            .GroupBy(c => c.KeyType!)
            .ToDictionary(g => g.Key, g => g.Count());

        var recommendations = GenerateRecommendations(characters);

        return new OptimizerAnalysisResponse(
            totalCharacters,
            totalKakera,
            keyDistribution,
            recommendations
        );
    }

    public async Task<OptimizerSuggestionsResponse> GetSuggestionsAsync(
        Guid userId,
        CancellationToken cancellationToken = default) {
        var entries = await dbContext.CollectionEntries
            .Include(e => e.Character)
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);

        var suggestions = GenerateSuggestions(entries.Select(e => e.Character).ToList());

        return new OptimizerSuggestionsResponse(suggestions);
    }

    private static List<OptimizerRecommendation> GenerateRecommendations(List<Character> characters) {
        var recommendations = new List<OptimizerRecommendation>();

        var topByKakera = characters
            .OrderByDescending(c => c.Kakera ?? 0)
            .Take(5)
            .ToList();

        foreach (var character in topByKakera) {
            recommendations.Add(new OptimizerRecommendation(
                "priority",
                character.Name,
                $"High kakera value: {character.Kakera} ka",
                character.Kakera >= 600 ? "high" : character.Kakera >= 400 ? "medium" : "low"
            ));
        }

        return recommendations;
    }

    private static List<OptimizerSuggestionDto> GenerateSuggestions(List<Character> characters) {
        var suggestions = new List<OptimizerSuggestionDto>();
        var priority = 1;

        var highValueChars = characters
            .Where(c => (c.Kakera ?? 0) >= 500)
            .OrderByDescending(c => c.Kakera ?? 0)
            .Take(10)
            .ToList();

        if (highValueChars.Count > 0) {
            suggestions.Add(new OptimizerSuggestionDto(
                Guid.CreateVersion7(),
                "enable",
                highValueChars.Select(c => c.Name).ToList(),
                $"Enable {highValueChars.Count} high-value characters (500+ ka)",
                priority++
            ));
        }

        var keyChars = characters
            .Where(c => c.KeyCount.HasValue && c.KeyCount.Value >= 1)
            .Select(c => new { Character = c, KeyType = KeyHelper.GetKeyTypeFromCount(c.KeyCount) })
            .OrderByDescending(c => c.Character.Kakera ?? 0)
            .ToList();

        if (keyChars.Count > 0) {
            var keyGroups = keyChars.GroupBy(c => c.KeyType!);
            foreach (var group in keyGroups.Take(3)) {
                suggestions.Add(new OptimizerSuggestionDto(
                    Guid.CreateVersion7(),
                    "enable",
                    group.Select(c => c.Character.Name).ToList(),
                    $"Enable {group.Count()} {group.Key} characters",
                    priority++
                ));
            }
        }

        return suggestions;
    }
}
