using BroadcastServer.Core;
using BroadcastServer.Utils.UI;
using Spectre.Console;

AnsiConsole.Clear();
AnsiConsole.Write(
    new FigletText("Broadcast Server")
        .Centered()
        .Color(Color.Cyan1)
);

AnsiConsole.Write(new Rule("[bold green]Broadcast Server[/]").RuleStyle("green").Centered());

var port = 5000;
var server = new ServerHost(port);

await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .StartAsync("Starting server...", async ctx =>
    {
        await Task.Delay(1000);
        _ = server.StartAsync();
        AnsiConsole.WriteLine($"Server started on port {port}");
    });

Logger.Success("Server started!");
Logger.Info("Press Ctrl+C to exit...");
AnsiConsole.WriteLine();

await Task.Delay(-1);