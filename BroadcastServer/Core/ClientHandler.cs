using System.Net.Sockets;
using System.Text;

namespace BroadcastServer.Core;

public class ClientHandler
{
    private readonly TcpClient _client;
    private readonly Guid _id;
    private readonly BroadcastManager _manager;

    public ClientHandler(TcpClient listener, BroadcastManager manager)
    {
        _client = listener;
        _manager = manager;
        _id = Guid.NewGuid();
    }

    public async Task RunAsync()
    {
        var stream = _client.GetStream();
        var buffer = new byte[1024];

        try
        {
            _manager.Add(_id, this);
            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                await _manager.BroadcastAsync(message, _id);
            }
        }
        catch (Exception)
        {
            // ignored
        }
        finally
        {
            _manager.RemoveClient(_id);
            _client.Close();
        }
    }

    public async Task SendAsync(byte[] data)
    {
        if (_client.Connected) await _client.GetStream().WriteAsync(data, 0, data.Length);
    }
}