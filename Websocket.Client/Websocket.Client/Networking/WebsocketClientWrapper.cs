using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Websocket.Client.Models;
using Websocket.Configuration;

namespace Websocket.Client.Networking
{
    /// <summary>
    /// This class is responsible for the websocket client.
    /// It wraps the <see cref="ClientWebSocket"/> and provides methods to connect to the server and send messages.
    /// </summary>
    internal class WebSocketClientWrapper
    {
        private ClientWebSocket WebSocketClient = new();

        /// <summary>
        /// Connects to the websocket server.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectToServerAsync()
        {
            var serverUri = new Uri($"ws://localhost:{WebSocketSettings.Port}/{WebSocketSettings.Name}");
            var isConnected = false;

            Console.WriteLine($"Connecting to {serverUri}... ");

            while (!isConnected)
            {
                try
                {
                    if (WebSocketClient == null || WebSocketClient.State == WebSocketState.Closed || WebSocketClient.State == WebSocketState.Aborted)
                    {
                        WebSocketClient = new ClientWebSocket();
                    }

                    await WebSocketClient.ConnectAsync(serverUri, CancellationToken.None);
                    isConnected = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Connection failed! Exception message:\n{e.Message}\nRetrying... ");
                    Thread.Sleep(500);
                }
            }

            Console.WriteLine($"Websocket is now connected to {serverUri}!\n");
        }

        /// <summary>
        /// Sends a message to the websocket server.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendWebsocketMessage(string message)
        {
            var messageObject = new WebSocketMessage
            {
                Type = "TextMessage",
                Message = message,
                Sender = "100298",
                Receiver = "186293"
            };

            var serializedMessage = JsonSerializer.Serialize(messageObject);
            var messageBytes = Encoding.UTF8.GetBytes(serializedMessage);
            var bytesToSend = new ArraySegment<byte>(messageBytes);
            await WebSocketClient.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        /// <summary>
        /// Receives a message from the websocket server and prints it to the console.
        /// </summary>
        /// <returns></returns>
        public async Task ReceiveAndPrintServerResponse()
        {
            byte[] responseBuffer = new byte[1024];
            int offset = 0;
            int packet = 1024;

            while (true)
            {
                var bytesReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                var response = await WebSocketClient.ReceiveAsync(bytesReceived, CancellationToken.None);
                var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                Console.WriteLine(responseMessage);
                if (response.EndOfMessage)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Checks if the websocket client is connected to the server.
        /// </summary>
        /// <returns></returns>
        public bool IsConnected()
        {
            return WebSocketClient.State == WebSocketState.Open;
        }
    }
}