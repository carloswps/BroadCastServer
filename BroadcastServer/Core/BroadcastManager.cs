using System.Collections.Concurrent;
using System.Text;

namespace BroadcastServer.Core;

public class BroadcastManager
{
    private readonly ConcurrentDictionary<Guid, ClientHandler> _client = new();

    public void Add(Guid id, ClientHandler client)
    {
        if (_client.ContainsKey(id)) return;
        _client.TryAdd(id, client);
    }

    public void RemoveClient(Guid id)
    {
        if (!_client.ContainsKey(id)) return;
        _client.TryRemove(id, out _);
    }

    public async Task BroadcastAsync(string message, Guid guidClient)
    {
        var data = Encoding.UTF8.GetBytes(message);
        var task = _client
            .Where(x => x.Key != guidClient)
            .Select(x => x.Value.SendAsync(data));

        await Task.WhenAll(task);
    }
}