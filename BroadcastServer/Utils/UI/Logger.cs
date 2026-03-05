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

    public static void Error(string message, Exception? error)
    {
        if (error == null) return;
        var panel = new Panel(
                $"{Markup.Escape(error.Message)}{Environment.NewLine}[grey]({error.GetType().Name})[/]")
            .Header($"[bold red][[[/][bold red]ERRO[/][bold red]]][/] {message}")
            .BorderColor(Color.Red)
            .Expand();

        AnsiConsole.Write(panel);
    }

    public static void Broadcast(string message, string user)
    {
        var escapedMessage = Markup.Escape(message);
        AnsiConsole.MarkupLine(
            $"[bold yellow][[[/][bold yellow]MSG[/][bold yellow]]][/] [underline]{user}[/]: {escapedMessage}");
    }

    public static void ChatMessage(string user, string message, int timestamp, bool isMe = false)
    {
        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(message)) return;

        var color = isMe ? Color.Cyan1 : Color.Green;

        // Timestamp formating
        var timeStr = timestamp > 24
            ? DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().ToString("HH:mm")
            : $"{timestamp:D2}:00";

        var panel = new Panel(Markup.Escape(message))
        {
            Header = new PanelHeader($"[bold {color.ToMarkup()}] {user} [/] [grey]{timeStr}[/]"),
            Border = BoxBorder.Rounded,
            BorderStyle = color,
            Padding = new Padding(1, 0, 1, 0)
        };

        var alignment = isMe ? HorizontalAlignment.Right : HorizontalAlignment.Left;

        var renderable = new Padder(
            new Align(panel, alignment),
            new Padding(isMe ? 10 : 0, 0, isMe ? 0 : 10, 0)
        );

        AnsiConsole.Write(renderable);
    }
}