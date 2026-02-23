using System.Net;
using System.Net.Sockets;
using BroadcastServer.Utils.UI;

namespace BroadcastServer.Core;

public class ServerHost
{
    private readonly TcpListener _listener;
    private readonly BroadcastManager _manager;
    private bool _isRunning;

    public ServerHost(int listener)
    {
        _listener = new TcpListener(IPAddress.Any, listener);
        _manager = new BroadcastManager();
    }

    public async Task StartAsync()
    {
        _listener.Start();
        _isRunning = true;
        Logger.Success("Server started!");

        try
        {
            while (_isRunning)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var clientId = Guid.NewGuid();
                Logger.Info($"Client {clientId} connected.");

                var handler = new ClientHandler(client, _manager, clientId);
                _manager.AddClient(clientId, handler);
                _ = handler.RunAsync();
            }
        }
        catch (Exception e)
        {
            Logger.Error(e.Message);
        }
    }
}