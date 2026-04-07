using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using Spectre.Console.Rendering;
using Shiron.BeatDash.Data.Models;

namespace Shiron.BeatDash.Recorder.Commands;

public sealed class RecordCommand : AsyncCommand<RecordCommand.Settings> {
    public sealed class Settings : CommandSettings {
        [CommandArgument(0, "[output]")]
        public string Output { get; set; } = "./.recordings";

        [CommandOption("-h|--host")]
        public string Host { get; set; } = "ws://127.0.0.1:2946";
    }

    protected override async Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        if (!Directory.Exists(settings.Output))
            Directory.CreateDirectory(settings.Output);

        var outFile = Path.Join(settings.Output, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");

        var endpoints = new (string Name, string Uri)[] {
            ("MapData", $"{settings.Host}/BSDataPuller/MapData"),
            ("LiveData", $"{settings.Host}/BSDataPuller/LiveData")
        };

        var messages = new ConcurrentBag<RecordedMessage>();
        var state = new RecordingState();
        foreach (var (name, _) in endpoints)
            state.GetOrCreateEndpoint(name);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        Console.CancelKeyPress += (_, e) => {
            e.Cancel = true;
            cts.Cancel();
        };

        var receiveTasks = endpoints
            .Select(ep => ConnectAndReceiveAsync(ep.Name, ep.Uri, messages, state, cts.Token))
            .ToArray();

        var display = new RecorderDisplay(state, outFile);

        await AnsiConsole.Live(display)
            .StartAsync(async ctx => {
                while (!cts.IsCancellationRequested) {
                    ctx.Refresh();
                    try {
                        await Task.Delay(250, cts.Token);
                    } catch (OperationCanceledException) {
                        break;
                    }
                }
                ctx.Refresh();
            });

        await Task.WhenAll(receiveTasks);

        foreach (var ws in state.Connections) {
            try {
                if (ws.State == WebSocketState.Open)
                    await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Recording stopped", CancellationToken.None);
            } catch { }
            ws.Dispose();
        }

        var sorted = messages.OrderBy(m => m.Timestamp).ToList();
        var json = JsonSerializer.Serialize(sorted, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(outFile, json, CancellationToken.None);

        AnsiConsole.WriteLine();
        AnsiConsole.MarkupLine($"[green]Recording saved to [white]{outFile.EscapeMarkup()}[/] ({sorted.Count} messages)[/]");

        return 0;
    }

    private static async Task ConnectAndReceiveAsync(
        string name,
        string uri,
        ConcurrentBag<RecordedMessage> messages,
        RecordingState state,
        CancellationToken cancellationToken) {

        while (!cancellationToken.IsCancellationRequested) {
            var ws = new ClientWebSocket();
            state.AddConnection(ws);
            var endpoint = state.GetOrCreateEndpoint(name);
            endpoint.Status = ConnectionStatus.Connecting;

            try {
                await ws.ConnectAsync(new Uri(uri), cancellationToken);
                endpoint.Status = ConnectionStatus.Connected;

                var buffer = new byte[8192];

                while (ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested) {
                    var messageBuilder = new StringBuilder();
                    WebSocketReceiveResult result;

                    do {
                        result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    } while (!result.EndOfMessage);

                    if (result.MessageType == WebSocketMessageType.Close) {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                        break;
                    }

                    if (result.MessageType == WebSocketMessageType.Text) {
                        var rawMessage = messageBuilder.ToString();
                        endpoint.MessageCount++;
                        endpoint.LastMessageTime = DateTimeOffset.UtcNow;
                        messages.Add(new RecordedMessage(DateTimeOffset.UtcNow, name, rawMessage));
                        state.ProcessMessage(name, rawMessage);
                    }
                }
            } catch (OperationCanceledException) {
                endpoint.Status = ConnectionStatus.Disconnected;
                break;
            } catch (WebSocketException) {
                endpoint.Status = ConnectionStatus.Reconnecting;
                try {
                    await Task.Delay(5000, cancellationToken);
                } catch (OperationCanceledException) {
                    endpoint.Status = ConnectionStatus.Disconnected;
                    break;
                }
            } catch (Exception) {
                endpoint.Status = ConnectionStatus.Disconnected;
                break;
            }
        }
    }

    private record RecordedMessage(DateTimeOffset Timestamp, string Endpoint, string Message);
}

public enum ConnectionStatus {
    Disconnected,
    Connecting,
    Connected,
    Reconnecting
}

public class EndpointState {
    public string Name { get; }
    public ConnectionStatus Status { get; set; } = ConnectionStatus.Disconnected;
    public int MessageCount { get; set; }
    public DateTimeOffset? LastMessageTime { get; set; }

    public EndpointState(string name) {
        Name = name;
    }
}

public class MapSession {
    public string SongName { get; init; } = "";
    public string SongAuthor { get; init; } = "";
    public string Mapper { get; init; } = "";
    public string Difficulty { get; init; } = "";
    public string MapType { get; init; } = "";
    public string? BSRKey { get; init; }
    public int Duration { get; init; }
    public int BPM { get; init; }
    public double NJS { get; init; }
    public bool NoFailEnabled { get; init; }
    public DateTimeOffset StartedAt { get; init; }

    public int TimeElapsed { get; set; }
    public int Score { get; set; }
    public int Combo { get; set; }
    public int Misses { get; set; }
    public bool FullCombo { get; set; } = true;
    public string Rank { get; set; } = "SSS";
    public double Accuracy { get; set; } = 100;
    public double PlayerHealth { get; set; } = 50;
    public bool IsPaused { get; set; }
}

public class RecordingState {
    public ConcurrentBag<ClientWebSocket> Connections { get; } = [];
    public ConcurrentDictionary<string, EndpointState> Endpoints { get; } = new();
    private readonly object _lock = new();
    private MapSession? _currentSession;

    public MapSession? CurrentSession {
        get { lock (_lock) { return _currentSession; } }
    }

    public void AddConnection(ClientWebSocket ws) => Connections.Add(ws);

    public EndpointState GetOrCreateEndpoint(string name) =>
        Endpoints.GetOrAdd(name, n => new EndpointState(n));

    public void ProcessMessage(string endpointName, string rawMessage) {
        try {
            if (endpointName == "MapData") {
                ProcessMapData(rawMessage);
            } else if (endpointName == "LiveData") {
                ProcessLiveData(rawMessage);
            }
        } catch { }
    }

    private void ProcessMapData(string rawMessage) {
        var mapData = JsonSerializer.Deserialize<MapData>(rawMessage);
        if (mapData?.Hash == null) return;

        lock (_lock) {
            var isEndSignal = mapData.LevelFinished || mapData.LevelQuit
                || (mapData.LevelFailed && !mapData.Modifiers.NoFailOn0Energy);

            if (isEndSignal) {
                _currentSession = null;
                return;
            }

            if (mapData.LevelFailed && mapData.Modifiers.NoFailOn0Energy) {
                return;
            }

            if (!mapData.LevelFinished && !mapData.LevelFailed && !mapData.LevelQuit) {
                if (_currentSession == null) {
                    _currentSession = new MapSession {
                        SongName = mapData.SongName,
                        SongAuthor = mapData.SongAuthor,
                        Mapper = mapData.Mapper,
                        Difficulty = mapData.Difficulty,
                        MapType = mapData.MapType,
                        BSRKey = mapData.BSRKey,
                        Duration = mapData.Duration,
                        BPM = mapData.BPM,
                        NJS = mapData.NJS,
                        NoFailEnabled = mapData.Modifiers.NoFailOn0Energy,
                        StartedAt = DateTimeOffset.UtcNow,
                    };
                } else {
                    _currentSession.IsPaused = mapData.LevelPaused;
                }
            }
        }
    }

    private void ProcessLiveData(string rawMessage) {
        var liveData = JsonSerializer.Deserialize<LiveData>(rawMessage);
        if (liveData == null) return;

        lock (_lock) {
            if (_currentSession != null) {
                _currentSession.TimeElapsed = liveData.TimeElapsed;
                _currentSession.Score = liveData.Score;
                _currentSession.Combo = liveData.Combo;
                _currentSession.Misses = liveData.Misses;
                _currentSession.FullCombo = liveData.FullCombo;
                _currentSession.Rank = liveData.Rank;
                _currentSession.Accuracy = liveData.Accuracy;
                _currentSession.PlayerHealth = liveData.PlayerHealth;
            }
        }
    }
}

file class RecorderDisplay(RecordingState state, string outFile) : IRenderable {
    public Measurement Measure(RenderOptions options, int maxWidth) {
        return BuildContent().Measure(options, maxWidth);
    }

    public IEnumerable<Segment> Render(RenderOptions options, int maxWidth) {
        return BuildContent().Render(options, maxWidth);
    }

    private IRenderable BuildContent() {
        var content = new List<IRenderable> {
            BuildConnectionsSection(),
            new Rule().DoubleBorder(),
            BuildMapStatusSection(),
            new Rule().DoubleBorder(),
            new Markup($"  [grey]{outFile.EscapeMarkup()}[/]"),
        };

        return new Panel(new Rows(content))
            .Header(" [bold]BeatDash Recorder[/] ")
            .Border(BoxBorder.Rounded)
            .BorderColor(Color.Grey);
    }

    private IRenderable BuildConnectionsSection() {
        var now = DateTimeOffset.UtcNow;
        var rows = new List<IRenderable> {
            new Markup("  [bold]Connections[/]"),
        };

        foreach (var ep in state.Endpoints.Values.OrderBy(e => e.Name)) {
            var (icon, label) = ep.Status switch {
                ConnectionStatus.Connected => ("[green]●[/]", "Connected"),
                ConnectionStatus.Connecting => ("[yellow]◐[/]", "Connecting..."),
                ConnectionStatus.Reconnecting => ("[yellow]↻[/]", "Reconnecting"),
                _ => ("[red]○[/]", "Disconnected")
            };

            var lastMsg = ep.LastMessageTime.HasValue
                ? FormatElapsedTime(now - ep.LastMessageTime.Value)
                : "-";

            rows.Add(new Markup(
                $"    {icon} [bold]{ep.Name.EscapeMarkup()}[/]  [grey]{label} | {ep.MessageCount} events | {lastMsg.EscapeMarkup()}[/]"
            ));
        }

        return new Rows(rows);
    }

    private IRenderable BuildMapStatusSection() {
        var session = state.CurrentSession;

        if (session == null) {
            return new Rows(
                new Markup("  [bold grey]■ Idle[/]"),
                new Markup("  [grey]Waiting for a map to start...[/]")
            );
        }

        var statusLabel = session.IsPaused ? "[bold yellow]⏸ Paused[/]" : "[bold green]▶ Playing[/]";
        var elapsed = FormatDuration(session.TimeElapsed);
        var total = FormatDuration(session.Duration);
        var noFail = session.NoFailEnabled ? " | [yellow]NoFail[/]" : "";
        var bsr = !string.IsNullOrEmpty(session.BSRKey) ? $" | BSR: {session.BSRKey.EscapeMarkup()}" : "";
        var fc = session.FullCombo ? "[green]FC[/]" : $"{session.Misses} miss";

        return new Rows(
            new Markup($"  {statusLabel}"),
            new Markup($"  [bold white]{session.SongName.EscapeMarkup()}[/] — [grey]{session.SongAuthor.EscapeMarkup()}[/]"),
            new Markup($"  [grey]{session.Difficulty.EscapeMarkup()} | {session.MapType.EscapeMarkup()} | BPM {session.BPM} | NJS {session.NJS:F1}{noFail}{bsr}[/]"),
            new Markup($"  {elapsed} / {total}"),
            new Markup($"  Score: [bold]{session.Score:N0}[/] ({session.Rank.EscapeMarkup()}) | Combo: {session.Combo} | {fc} | Acc: {session.Accuracy:F2}%")
        );
    }

    private static string FormatDuration(int seconds) {
        var span = TimeSpan.FromSeconds(seconds);
        return $"{(int) span.TotalMinutes}:{span.Seconds:D2}";
    }

    private static string FormatElapsedTime(TimeSpan elapsed) {
        if (elapsed.TotalSeconds < 1) return "just now";
        if (elapsed.TotalMinutes < 1) return $"{(int) elapsed.TotalSeconds}s ago";
        return $"{(int) elapsed.TotalMinutes}m ago";
    }
}
