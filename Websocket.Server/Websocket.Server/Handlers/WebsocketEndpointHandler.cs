using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Websocket.Configuration;
using Websocket.Server.Connections;
using Websocket.Server.Models;

namespace Websocket.Server.Handlers
{
    internal class WebSocketEndpointHandler
    {
        public static void Configure(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path != $"/{WebSocketSettings.Name}")
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
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketSettings.Buffer), CancellationToken.None);

            // Handle data
            if (result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    var clientMessage = ReceiveMessage(result);
                    if (clientMessage == null)
                    {
                        Console.WriteLine("Received websocket message from Client is null!");
                    }
                    else
                    {
                        switch (clientMessage.Type)
                        {
                            case "TextMessage":
                                string serverResponse = $"Server says: Received text message \"{clientMessage.Message}\" from User {clientMessage.Sender} to User {clientMessage.Receiver}\n";
                                await SendMessageAsync(webSocket, serverResponse, result);
                                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketSettings.Buffer), CancellationToken.None);
                                break;
                            default:
                                Console.WriteLine($"{nameof(WebSocketMessage)}.{nameof(WebSocketMessage.Type)} '{clientMessage.Type}' is not supported!");
                                break;
                        }
                    }
                }
            }

            // Close connection
            await webSocket.CloseAsync(result.CloseStatus!.Value, result.CloseStatusDescription, CancellationToken.None);
            WebsocketManager.Instance.RemoveClient(webSocket);
        }

        private static WebSocketMessage ReceiveMessage(WebSocketReceiveResult result)
        {
            var messageBytes = new ArraySegment<byte>(WebSocketSettings.Buffer, 0, result.Count);
            var messageString = Encoding.UTF8.GetString(messageBytes.Array, messageBytes.Offset, messageBytes.Count);

            return JsonSerializer.Deserialize<WebSocketMessage>(messageString);
        }

        private static async Task SendMessageAsync(WebSocket webSocket, string message, WebSocketReceiveResult result)
        {
            var messageBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
            await webSocket.SendAsync(messageBytes, result.MessageType, result.EndOfMessage, CancellationToken.None);
        }
    }
}