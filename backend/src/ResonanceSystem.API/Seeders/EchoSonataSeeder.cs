using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.API.Seeders;

public static class EchoSonataSeeder {
    public static void SeedEchoSonatas(this IServiceProvider services) {
        using var context = services.GetRequiredService<ResSystemDbContext>();

        var sonatasToSeed = new List<EchoSonata> {
            new() { Name = "Celestial Light" },
            new() { Name = "Chromatic Foam" },
            new() { Name = "Crown of Valor" },
            new() { Name = "Dream of the Lost" },
            new() { Name = "Empyrean Anthem" },
            new() { Name = "Eternal Radiance" },
            new() { Name = "Flamewing's Shadow" },
            new() { Name = "Flaming Clawprint" },
            new() { Name = "Freezing Frost" },
            new() { Name = "Frosty Resolve" },
            new() { Name = "Gusts of Welkin" },
            new() { Name = "Halo of Starry Radiance" },
            new() { Name = "Havoc Eclipse" },
            new() { Name = "Law of Harmony" },
            new() { Name = "Lingering Tunes" },
            new() { Name = "Midnight Veil" },
            new() { Name = "Molten Rift" },
            new() { Name = "Moonlit Clouds" },
            new() { Name = "Pact of Neonlight Leap" },
            new() { Name = "Reel of Spliced Memories" },
            new() { Name = "Rejuvenating Glow" },
            new() { Name = "Rite of Gilded Revelation" },
            new() { Name = "Shadow of Shattered Dreams" },
            new() { Name = "Sierra Gale" },
            new() { Name = "Sound of True Name" },
            new() { Name = "Thread of Severed Fate" },
            new() { Name = "Tidebreaking Courage" },
            new() { Name = "Trailblazing Star" },
            new() { Name = "Void Thunder" },
            new() { Name = "Windward Pilgrimage" },
            new() { Name = "Wishes of Quiet Snowfall" }
        };

        var existingSonataNames = context.EchoSonatas
            .Select(s => s.Name)
            .ToHashSet();

        var sonatasToAdd = sonatasToSeed
            .Where(s => !existingSonataNames.Contains(s.Name))
            .ToList();

        if (sonatasToAdd.Count > 0) {
            context.EchoSonatas.AddRange(sonatasToAdd);
            context.SaveChanges();
        }

        context.SaveChanges();
    }
}
