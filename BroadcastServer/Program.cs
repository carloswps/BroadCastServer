using BroadcastServer.Services;
using BroadcastServer.Utils;
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
            if (jwtToken == null)
            {
                AnsiConsole.MarkupLine("[red]❌ Login failed. Please check your credentials.[/]");
                await Task.Delay(2000);
            }
        });
}

var currentUserId = JwtParser.GetUserIdFromToken(jwtToken!);
var signalR = new SignalRClient(hubUrl, jwtToken, currentUserId);

await AnsiConsole.Status()
    .Spinner(Spinner.Known.Dots)
    .StartAsync("Starting server...", async ctx => { await signalR.StartAsync(); });

AnsiConsole.Write(new Rule("[bold green]Chat[/]").RuleStyle("green").Centered());

AnsiConsole.MarkupLine($"[bold grey]✅ Connected to Chat as User {currentUserId}![/]");

int? targetUserId = null;

while (true)
{
    if (targetUserId == null)
    {
        targetUserId = AnsiConsole.Prompt(
            new TextPrompt<int>("👤 [bold yellow]Enter Target User ID to chat (or 0 to exit):[/]")
                .ValidationErrorMessage("[red]Please enter a valid numeric ID![/]")
        );

        if (targetUserId == 0) break;

        await signalR.JoinPrivateChatAsync(targetUserId.Value);
        AnsiConsole.MarkupLine($"[bold grey]✅ Switched chat to User {targetUserId}![/]");
        AnsiConsole.MarkupLine(
            "[grey]Type your message and press Enter. Type '/exit' to quit or '/target' to change user.[/]\n");
    }

    var input = AnsiConsole.Prompt(
        new TextPrompt<string>($"[bold yellow]Message (to {targetUserId}) >[/]")
            .AllowEmpty());

    if (string.IsNullOrWhiteSpace(input)) continue;

    var cmd = input.ToLower().Trim();
    if (cmd == "/exit" || cmd == "exit") break;
    if (cmd == "/target")
    {
        targetUserId = null;
        continue;
    }

    try
    {
        await signalR.SendMessageAsync(targetUserId.Value, input);

        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth) + "\r");
    }
    catch (Exception ex)
    {
        Logger.Error("Failed to send message", ex);
    }
}

AnsiConsole.MarkupLine("[bold red]Disconnected...[/]");