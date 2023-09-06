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
                if (context.Request.Path == $"/{WebSocketConfiguration.Name}")
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
                else
                {
                    await HandleDataAsync(context.WebSockets);
                }

                // Call the next middleware in the pipeline
                if (next != null)
                {
                    await next(context);
                }
            });
        }

        /// <summary>
        /// Handles incoming data from a client.
        /// </summary>
        /// <param name="webSocketManager">The websocket manager of the request.</param>
        /// <returns>An awaitable Task that handles the data from the request.</returns>
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
                    string clientMessage = ReceiveMessage(result);
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

        private static string ReceiveMessage(WebSocketReceiveResult result)
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