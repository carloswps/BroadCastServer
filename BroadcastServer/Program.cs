using System.Net.Http.Headers;
using System.Net.Http.Json;
using BroadcastServer.Core;
using BroadcastServer.Utils.UI;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

AnsiConsole.Clear();
AnsiConsole.Write(
    new FigletText("Broadcast CLI")
        .Centered()
        .Color(Color.Cyan1)
);

AnsiConsole.Write(new Rule("[bold green]Broadcast Server[/]").RuleStyle("green").Centered());

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .Build();

var hubUrl = config["SignalR:HubUrl"];
var apiUrl = config["SignalR:ApiUrl"];
if (string.IsNullOrWhiteSpace(apiUrl)) throw new Exception("SignalR API URL is required!");
if (string.IsNullOrWhiteSpace(hubUrl)) throw new Exception("SignalR Hub URL is required!");

var jwtToken = AnsiConsole.Ask<string?>("Send you [bold cyan]Token JWT[/] for you connection");
if (string.IsNullOrWhiteSpace(jwtToken)) throw new Exception("Token JWT is required!");
var signalRUrl = new SignalRClient(hubUrl, jwtToken);

await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .StartAsync("Starting server...", async ctx => { await signalRUrl.StartAsync(); });

Logger.Info("Server connected send messages to clients!");
AnsiConsole.MarkupLine("[bold green]Press CTRL+C to exit[/]");
AnsiConsole.WriteLine();

while (true)
{
    var input = AnsiConsole.Ask<string>("[bold yellow]Message >[/]");
    if (input?.ToLower() == "exit") break;

    if (!string.IsNullOrWhiteSpace(input))
    {
        var targetUserId = '1';
        var url = $"{apiUrl}/send-message/{targetUserId}";

        var dto = new
        {
            Tittle = "CLI Notification",
            Message = input
        };

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwtToken);

        var response = await client.PostAsJsonAsync(url, dto);
        if (response.IsSuccessStatusCode)
            Logger.Success("Message sent!");
        else
            Logger.Error("Error sending message!");
    }
}

AnsiConsole.MarkupLine("[bold red]Disconnected...[/]");