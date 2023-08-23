using System.Net.WebSockets;
using System.Text;

namespace Websocket.Server.Connections
{
    /// <summary>
    /// Keeps track of connected clients.
    /// </summary>
    public class WebsocketManager
    {
        private static readonly Lazy<WebsocketManager> instance = new(() => new WebsocketManager());

        public static WebsocketManager Instance => instance.Value;

        private readonly List<WebSocket> connectedClients = new();

        private WebsocketManager() { }

        public void AddClient(WebSocket client) => connectedClients.Add(client);
        public void RemoveClient(WebSocket client) => connectedClients.Remove(client);

        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var client in connectedClients)
            {
                if (client.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}