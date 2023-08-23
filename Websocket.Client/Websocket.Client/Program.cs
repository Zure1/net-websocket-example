using System.Net.WebSockets;
using System.Text;
using Websocket.Configuration;

namespace Websocket.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Press 'Enter' to continue...");
            Console.ReadLine();

            using (var websocketClient = new ClientWebSocket())
            {
                var serviceUri = new Uri($"ws://localhost:{WebSocketConfiguration.Port}/send");
                var cancellationTokenSource = new CancellationTokenSource();
                cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(120));

                try
                {
                    await EstablishWebsocketConnectionAsync(websocketClient, serviceUri, cancellationTokenSource.Token);
                    await HandleWebsocketCommunicationAsync(websocketClient, cancellationTokenSource.Token);
                }
                catch (WebSocketException e)
                {
                    Console.WriteLine(e);
                }
            }

            Console.ReadLine();
        }

        static async Task EstablishWebsocketConnectionAsync(ClientWebSocket websocketClient, Uri serviceUri, CancellationToken cancellationToken)
        {
            await websocketClient.ConnectAsync(serviceUri, cancellationToken);
            Console.WriteLine("Websoket connection established!");
        }

        static async Task HandleWebsocketCommunicationAsync(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            while (clientWebSocket.State == WebSocketState.Open)
            {
                Console.WriteLine("Enter Message to send: ");
                var message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    await SendWebsocketMessage(clientWebSocket, message, cancellationToken);
                    await ReceiveAndPrintWebsocketResponse(clientWebSocket, cancellationToken);
                }
            }
        }

        static async Task SendWebsocketMessage(ClientWebSocket clientWebSocket, string message, CancellationToken cancellationToken)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var bytesToSend = new ArraySegment<byte>(messageBytes);
            await clientWebSocket.SendAsync(bytesToSend, WebSocketMessageType.Text, true, cancellationToken);
        }

        static async Task ReceiveAndPrintWebsocketResponse(ClientWebSocket clientWebSocket, CancellationToken cancellationToken)
        {
            byte[] responseBuffer = new byte[1024];
            int offset = 0;
            int packet = 1024;

            while (true)
            {
                var bytesReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
                var response = await clientWebSocket.ReceiveAsync(bytesReceived, cancellationToken);
                var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                Console.WriteLine(responseMessage);
                if (response.EndOfMessage)
                {
                    break;
                }
            }
        }
    }
}