using Microsoft.Extensions.Logging;

namespace Shiron.HonamiSystem.SDK;

public abstract class HonamiPlugin(string group, string name, string version) {
    public string Group { get; } = group;
    public string Name { get; } = name;
    public string Version { get; } = version;
    public ILogger Logger { get; set; } = null!;

    public string Triple => $"{Group}:{Name}@{Version}";

    public abstract void Initialize();
    public abstract void Dispose();
}
