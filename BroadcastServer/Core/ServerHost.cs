using System.Net;
using System.Net.Sockets;

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
        Console.WriteLine("Server started and listening...");

        try
        {
            while (_isRunning)
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"[LOG] A new client connected: {client.Client.RemoteEndPoint}");

                var handler = new ClientHandler(client, _manager);
                var clientId = Guid.NewGuid();
                _manager.Add(clientId, handler);
                _ = handler.RunAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"[ERROR] Error: {e.Message}");
        }
    }
}