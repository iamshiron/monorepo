namespace Shiron.HonamiSystem.SDK;

public abstract class HonamiPlugin(string group, string name, string version) {
    public string Group { get; } = group;
    public string Name { get; } = name;
    public string Version { get; } = version;

    public string Triple => $"{Group}:{Name}@{Version}";
}
