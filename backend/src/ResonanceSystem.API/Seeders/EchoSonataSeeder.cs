using Shiron.ResonanceSystem.DB;
using Shiron.ResonanceSystem.DB.Schema;

namespace Shiron.ResonanceSystem.API.Seeders;

public static class EchoSonataSeeder {
    public static void SeedEchoSonatas(this IServiceProvider services) {
        using var context = services.GetRequiredService<ResSystemDbContext>();

        var sonatasToSeed = new List<EchoSonata> {
            new("Celestial Light"),
            new("Chromatic Foam"),
            new("Crown of Valor"),
            new("Dream of the Lost"),
            new("Empyrean Anthem"),
            new("Eternal Radiance"),
            new("Flamewing's Shadow"),
            new("Flaming Clawprint"),
            new("Freezing Frost"),
            new("Frosty Resolve"),
            new("Gusts of Welkin"),
            new("Halo of Starry Radiance"),
            new("Havoc Eclipse"),
            new("Law of Harmony"),
            new("Lingering Tunes"),
            new("Midnight Veil"),
            new("Molten Rift"),
            new("Moonlit Clouds"),
            new("Pact of Neonlight Leap"),
            new("Reel of Spliced Memories"),
            new("Rejuvenating Glow"),
            new("Rite of Gilded Revelation"),
            new("Shadow of Shattered Dreams"),
            new("Sierra Gale"),
            new("Sound of True Name"),
            new("Thread of Severed Fate"),
            new("Tidebreaking Courage"),
            new("Trailblazing Star"),
            new("Void Thunder"),
            new("Windward Pilgrimage"),
            new("Wishes of Quiet Snowfall")
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
