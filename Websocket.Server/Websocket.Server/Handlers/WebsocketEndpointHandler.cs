using System.Net;
using System.Net.WebSockets;
using System.Text;
using Websocket.Configuration;
using Websocket.Server.Connections;

namespace Websocket.Server.Handlers
{
    internal class WebSocketEndpointHandler
    {
        public static void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                switch (context.Request.Path)
                {
                    case EndpointConstants.SendTextMessage:
                    case EndpointConstants.CreateGroupChat:
                        await HandleDataAsync(context.WebSockets);
                        break;
                    default:
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                }

                if (next != null)
                {
                    await next(context);
                }
            });
        }

        private static async Task HandleDataAsync(WebSocketManager webSocketManager)
        {
            if (!webSocketManager.IsWebSocketRequest)
            {
                Console.WriteLine("Not a websocket request");
                return;
            }

            var webSocket = await webSocketManager.AcceptWebSocketAsync();
            WebsocketManager.Instance.AddClient(webSocket);

            // Receive data
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketConfiguration.Buffer), CancellationToken.None);

            // Handle data
            if (result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    string clientMessage = ReceiveMessage(webSocket, result);
                    Console.WriteLine($"Client says: {clientMessage}");

                    string serverResponse = $"Server says: Received message \"{clientMessage}\"\n";
                    await SendMessageAsync(webSocket, serverResponse, result);

                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketConfiguration.Buffer), CancellationToken.None);
                }
            }

            // Close connection
            await webSocket.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, CancellationToken.None);
            WebsocketManager.Instance.RemoveClient(webSocket);
        }

        private static string ReceiveMessage(WebSocket webSocket, WebSocketReceiveResult result)
        {
            var messageBytes = new ArraySegment<byte>(WebSocketConfiguration.Buffer, 0, result.Count);
            return Encoding.UTF8.GetString(messageBytes);
        }

        private static async Task SendMessageAsync(WebSocket webSocket, string message, WebSocketReceiveResult result)
        {
            var messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(messageBytes, result.MessageType, result.EndOfMessage, CancellationToken.None);
        }
    }
}