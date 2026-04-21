using Microsoft.Extensions.Logging;
using Shiron.HonamiSystem.SDK.Components;

namespace Shiron.HonamiSystem.SDK;

public abstract class HonamiPlugin(string group, string name, string version) {
    public string Group { get; } = group;
    public string Name { get; } = name;
    public string Version { get; } = version;
    public ILogger Logger { get; set; } = null!;
    public Dictionary<string, PluginComponent> Components { get; } = [];

    public string Triple => $"{Group}:{Name}@{Version}";

    // Plugin Lifecycle Events
    public abstract void Initialize();
    public abstract void Dispose();

    // Plugin Functions
    protected void RegisterComponent(string id, PluginComponent component) {
        Components[id] = component;
    }
}
