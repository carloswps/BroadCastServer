using System.Net.Http.Headers;
using System.Net.Http.Json;
using BroadcastServer.Models;
using BroadcastServer.Services;
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
if (string.IsNullOrWhiteSpace(apiUrl) || string.IsNullOrWhiteSpace(hubUrl))
{
    Logger.Error("SignalR API URL is not configured in appsettings.json", null);
    return;
}

var authService = new AuthService(apiUrl);
string? jwtToken = null;

while (string.IsNullOrWhiteSpace(jwtToken))
{
    AnsiConsole.Clear();
    AnsiConsole.Write(new FigletText("Broadcast CLI").Color(Color.Cyan1));
    AnsiConsole.Write(new Rule("[bold green]Authentication is required[/]").RuleStyle("red").Centered());
    AnsiConsole.WriteLine();

    var email = AnsiConsole.Prompt(new TextPrompt<string>("[white]Email[/] [grey]>[/]").PromptStyle("red")
        .ValidationErrorMessage("Email is required").Validate(email => email.Contains('@')));
    var password =
        AnsiConsole.Prompt(new TextPrompt<string>("🔑 [white]Senha:[/]").PromptStyle("red").Secret());

    await AnsiConsole.Status()
        .StartAsync("Authenticating...", async ctx =>
        {
            jwtToken = await authService.LoginAsync(email, password);
            if (jwtToken == null) AnsiConsole.MarkupLine("[red]❌ Error.[/]");
        });
}

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

        var dto = new SendMessageDto
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
            Logger.Error("Error sending message!", null);
    }
}

AnsiConsole.MarkupLine("[bold red]Disconnected...[/]");