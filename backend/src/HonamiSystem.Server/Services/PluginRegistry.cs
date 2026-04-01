using Shiron.HonamiSystem.SDK;
using Shiron.HonamiSystem.Services;

namespace Shiron.HonamiSystem.Server.Services;

public class PluginRegistry : IPluginRegistry {
    private readonly List<HonamiPlugin> _plugins = [];
    private HonamiPlugin[] _pluginSnapshot = [];

    public IEnumerable<HonamiPlugin> Plugins => _pluginSnapshot;
    public void RegisterPlugin(HonamiPlugin plugin) {
        _plugins.Add(plugin);
        _pluginSnapshot = [.._plugins];
    }
}
