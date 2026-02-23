using Spectre.Console;

namespace BroadcastServer.Utils.UI;

public static class Logger
{
    public static void Success(string message)
    {
        AnsiConsole.MarkupLine($"[bold green][[[/][bold green]LOG[/][bold green]]][/] {message}");
    }

    public static void Info(string message)
    {
        AnsiConsole.MarkupLine($"[bold blue][[[/][bold blue]INFO[/][bold blue]]][/] {message}");
    }

    public static void Error(string message)
    {
        AnsiConsole.MarkupLine($"[bold red][[[/][bold red]ERRO[/][bold red]]][/] {message}");
    }

    public static void Broadcast(string message, string user)
    {
        var escapedMessage = Markup.Escape(message);
        AnsiConsole.MarkupLine(
            $"[bold yellow][[[/][bold yellow]MSG[/][bold yellow]]][/] [underline]{user}[/]: {escapedMessage}");
    }
}