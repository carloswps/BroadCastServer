using BroadcastServer.Utils.UI;
using Microsoft.AspNetCore.SignalR.Client;

namespace BroadcastServer.Services;

public class SignalRClient
{
    private readonly HubConnection _connection;

    public SignalRClient(string hubUrl, string jwtToken)
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

    public async Task SendMessageAsync(string user, string message)
    {
        try
        {
            await _connection.InvokeAsync("SendMessage", user, message);
        }
        catch (Exception e)
        {
            Logger.Error("Error sending message: ", new Exception(e.Message));
        }
    }
}