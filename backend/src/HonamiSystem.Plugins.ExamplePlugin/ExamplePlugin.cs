using Microsoft.Extensions.Logging;
using Shiron.HonamiSystem.SDK;

namespace Shiron.HonamiSystem.Plugins.ExamplePlugin;

public class ExamplePlugin() : HonamiPlugin("io.shiron", "example", "0.0.0") {
    public override void Initialize() {
        Logger.LogInformation("Example plugin initialized");
    }
    public override void Dispose() {
        Logger.LogInformation("Example plugin disposed");
    }
}
