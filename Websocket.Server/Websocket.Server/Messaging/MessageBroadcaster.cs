using Websocket.Server.Connections;

namespace Websocket.Server.Messaging
{
    /// <summary>
    /// Background service to send random messages to all connected clients.
    /// </summary>
    public class MessageBroadcaster
    {
        public void StartSending(int timeBetweenMessagesInSeconds)
        {
            Timer timer = new(SendRandomMessage!, null, TimeSpan.Zero, TimeSpan.FromSeconds(timeBetweenMessagesInSeconds));
        }

        private void SendRandomMessage(object state)
        {
            string message = $"Server broadcasts message to all clients at {DateTime.Now}";
            WebsocketManager.Instance.BroadcastMessageAsync(message).Wait();
        }
    }
}
