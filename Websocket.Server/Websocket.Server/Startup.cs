using System.Net;
using System.Net.WebSockets;
using System.Text;
using Websocket.Configuration;

namespace Websocket.Server
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            var wsOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(WebSocketConfiguration.KeepAliveIntervalSeconds)
            };

            app.UseWebSockets(wsOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/send")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        using (WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync())
                        {
                            await SendAsync(webSocket);
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }

                // Call the next delegate to continue the pipeline (optional)
                if (next != null)
                {
                    await next(context);
                }
            });
        }

        private static async Task SendAsync(WebSocket webSocket)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketConfiguration.Buffer), CancellationToken.None);

            if (result != null)
            {
                while (!result.CloseStatus.HasValue)
                {
                    var message = Encoding.UTF8.GetString(new ArraySegment<byte>(WebSocketConfiguration.Buffer, 0, result.Count));
                    Console.WriteLine($"Client says: {message}");
                    await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Server says: {DateTime.UtcNow:f}\n")), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    result = await webSocket.ReceiveAsync(new ArraySegment<byte>(WebSocketConfiguration.Buffer), CancellationToken.None);
                }
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
