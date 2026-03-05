using BroadcastServer.Models;
using BroadcastServer.Utils.UI;
using Microsoft.AspNetCore.SignalR.Client;
using Spectre.Console;

namespace BroadcastServer.Services;

public class SignalRClient
{
    private readonly HubConnection _connection;
    private readonly int _currentUserId;

    public SignalRClient(string hubUrl, string jwtToken, int currentUserId)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(jwtToken);

                options.HttpMessageHandlerFactory = handler =>
                {
                    if (handler is HttpClientHandler clientHandler)
                        clientHandler.ServerCertificateCustomValidationCallback =
                            (message, cert, chain, errors) => true;
                    return handler;
                };
            })
            .WithAutomaticReconnect()
            .Build();

        _currentUserId = currentUserId;

        _connection.On<PrivateMessageDto>("ReceivePrivateMessage",
            msg =>
            {
                Console.Write("\r" + new string(' ', Console.WindowWidth) + "\r");

                var isMe = msg.SenderId == _currentUserId;
                var displayName = isMe ? "Você" : msg.SenderId.ToString();

                Logger.ChatMessage(displayName, msg.Content, msg.Timestamp, isMe);

                AnsiConsole.Markup("[bold yellow]Message > [/]");
            });

        _connection.On<string, string>("ReceiveNotification", (user, message) => { Logger.Broadcast(message, user); });

        _connection.Reconnecting += error =>
        {
            Logger.Error("Connection lost, reconnecting...", error);
            return Task.CompletedTask;
        };

        _connection.Reconnected += connectionId =>
        {
            Logger.Success("Reconnected to SignalR!");
            return Task.CompletedTask;
        };

        _connection.Closed += error =>
        {
            if (error != null)
            {
                Logger.Error("Connection closed...", error);
                return Task.CompletedTask;
            }

            Logger.Info("Connection closed...");
            return Task.CompletedTask;
        };
    }

    public async Task StartAsync()
    {
        try
        {
            await _connection.StartAsync();
            Logger.Success("SignalR connected!");
        }
        catch (Exception e)
        {
            Logger.Error("Error connecting to SignalR: ", new Exception(e.Message));
            throw;
        }
    }

    public async Task JoinPrivateChatAsync(int targetUserId)
    {
        try
        {
            await _connection.InvokeAsync("JoinPrivateChat", targetUserId);
        }
        catch (Exception e)
        {
            Logger.Error("Error joining private chat: ", new Exception(e.Message));
        }
    }

    public async Task SendMessageAsync(int targetUserId, string message)
    {
        try
        {
            await _connection.InvokeAsync("SendMessage", targetUserId, message);
        }
        catch (Exception e)
        {
            Logger.Error("Error sending message: ", new Exception(e.Message));
        }
    }
}