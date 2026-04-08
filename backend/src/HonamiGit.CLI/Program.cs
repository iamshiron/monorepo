using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(c => {
    c.SetApplicationName("honamigit");
    c.SetApplicationVersion("0.0.0");
});

await app.RunAsync(args);
