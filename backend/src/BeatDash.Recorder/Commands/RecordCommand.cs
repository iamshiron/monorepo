using Spectre.Console;
using Spectre.Console.Cli;

namespace Shiron.BeatDash.Recorder.Commands;

public sealed class RecordCommand : AsyncCommand<RecordCommand.Settings> {
    public sealed class Settings : CommandSettings {
        [CommandArgument(0, "[output]")] public string Output { get; set; } = "./.recordings";
        [CommandOption("-h|--host")] public string Host { get; set; } = "ws://127.0.0.1:2946";
    }

    protected override Task<int> ExecuteAsync(CommandContext context, Settings settings, CancellationToken cancellationToken) {
        if (!Directory.Exists(settings.Output)) Directory.CreateDirectory(settings.Output);
        var outFile = Path.Join(settings.Output, $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.json");

        AnsiConsole.MarkupLine($"[green]Recording saved to [white]{outFile}[/][/]");
        return Task.FromResult(0);
    }
}
