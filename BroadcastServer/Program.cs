using BroadcastServer.Core;
using Spectre.Console;

AnsiConsole.Write(new FigletText("Broadcast Server").Centered().Color(Color.Green));

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

AnsiConsole.MarkupLine($"[green]Server ON.[/] listening at the port [bold]{port}[/]");
AnsiConsole.MarkupLine("Press [bold]CTRL+C[/] to exit.");

await Task.Delay(-1);