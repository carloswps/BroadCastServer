using BroadcastServer.Utils.UI;
using Microsoft.AspNetCore.SignalR.Client;

namespace BroadcastServer.Core;

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
            Logger.Error("Error connecting to SignalR: " + e.Message);
        }
    }

    public async Task SendMessageAsync(string user, string message)
    {
        await _connection.InvokeAsync("SendMessage", user, message);
    }
}