using Shiron.HonamiSystem.SDK;

namespace Shiron.HonamiSystem.Services;

public interface IPluginRegistry {
    void RegisterPlugin(HonamiPlugin plugin);
    IEnumerable<HonamiPlugin> Plugins { get; }
}
