using System.Collections.Concurrent;
using System.Text;

namespace BroadcastServer.Core;

public class BroadcastManager
{
    private readonly ConcurrentDictionary<Guid, ClientHandler> _client = new();

    public void AddClient(Guid id, ClientHandler client)
    {
        if (_client.TryAdd(id, client)) Console.WriteLine($"[LOG] Client {id} added to the list.");
    }

    public void RemoveClient(Guid id)
    {
        if (_client.TryRemove(id, out _)) Console.WriteLine($"[LOG] Client {id} removed from the list.");
    }

    public int GetConnectedClients()
    {
        return _client.Count;
    }

    public async Task BroadcastAsync(string message, Guid guidClient)
    {
        var data = Encoding.UTF8.GetBytes(message);
        var task = _client
            .Where(x => x.Key != guidClient)
            .Select(x => x.Value.SendAsync(data));

        await Task.WhenAll(task);
    }

    public async Task BroadcastMessageAsync(string message, Guid? excludeId = null)
    {
        var data = Encoding.UTF8.GetBytes($"[BROADCAST] {message}");
        var tasks = _client
            .Where(x => x.Key != excludeId)
            .Select(x => x.Value.SendAsync(data));

        await Task.WhenAll(tasks);
    }
}