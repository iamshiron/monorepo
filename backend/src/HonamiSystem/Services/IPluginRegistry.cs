using Microsoft.Extensions.Logging;
using Shiron.HonamiSystem.SDK;

namespace Shiron.HonamiSystem.Services;

public interface IPluginRegistry {
    void RegisterPlugin(HonamiPlugin plugin, ILogger logger);
    void Initialize();
    void Dispose();

    IEnumerable<HonamiPlugin> Plugins { get; }
}
