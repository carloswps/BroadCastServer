using System.Net.Sockets;
using System.Text;
using BroadcastServer.Utils.UI;

namespace BroadcastServer.Core;

public class ClientHandler(TcpClient listener, BroadcastManager manager, Guid id)
{
    public async Task RunAsync()
    {
        try
        {
            manager.AddClient(id, this);

            await manager.BroadcastMessageAsync($"[NEW CLIENT] {id} connected.", id);

            var stream = listener.GetStream();
            var buffer = new byte[1024];

            manager.AddClient(id, this);

            while (true)
            {
                var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;
                var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                await manager.BroadcastMessageAsync($"Message from {id.ToString()[..5]}: {message}", id);
            }
        }
        catch (Exception e)
        {
            Logger.Error($"An error occurred while handling a client: {e.Message}");
        }
        finally
        {
            manager.RemoveClient(id);
            await manager.BroadcastMessageAsync($"[CLIENT DISCONNECTED] {id.ToString()[..5]}", id);
            listener.Close();
        }
    }

    public async Task SendAsync(byte[] data)
    {
        if (listener.Connected) await listener.GetStream().WriteAsync(data, 0, data.Length);
    }
}