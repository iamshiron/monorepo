using Shiron.HonamiSystem.SDK;
using Shiron.HonamiSystem.Services;

namespace Shiron.HonamiSystem.Server.Services;

public class PluginRegistry : IPluginRegistry {
    private readonly List<HonamiPlugin> _plugins = [];
    private HonamiPlugin[] _pluginSnapshot = [];

    public IEnumerable<HonamiPlugin> Plugins => _pluginSnapshot;
    public void RegisterPlugin(HonamiPlugin plugin, ILogger logger) {
        plugin.Logger = logger;

        _plugins.Add(plugin);
        _pluginSnapshot = [.. _plugins];
    }

    public void Initialize() {
        foreach (var plugin in _pluginSnapshot) {
            plugin.Initialize();
        }
    }
    public void Dispose() {
        foreach (var plugin in _pluginSnapshot) {
            plugin.Dispose();
        }
    }
}
