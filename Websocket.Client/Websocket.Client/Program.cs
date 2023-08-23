using System.Net.WebSockets;
using System.Text;
using Websocket.Configuration;

namespace Websocket.Client
{
    class Program
    {
        private static ClientWebSocket WebSocketClient = new();

        static async Task Main(string[] args)
        {
            await EstablishWebsocketConnectionAsync();
            await HandleWebsocketCommunicationAsync();

            Console.ReadLine();
        }

        static async Task EstablishWebsocketConnectionAsync()
        {
            var serverUri = new Uri($"ws://localhost:{WebSocketConfiguration.Port}/send");
            ConsoleUtils.DisplayFakeProgress("Connecting to server", 500);
            await WebSocketClient.ConnectAsync(serverUri, CancellationToken.None);
            Console.WriteLine("Websocket connection established!\n");
        }

        static async Task HandleWebsocketCommunicationAsync()
        {
            while (WebSocketClient.State == WebSocketState.Open)
            {
                Console.WriteLine("Enter Message to send: ");
                var message = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(message))
                {
                    await SendWebsocketMessage(message);
                    await ReceiveAndPrintWebsocketResponse();
                }
            }
        }

        static async Task SendWebsocketMessage(string message)
        {
            var messageBytes = Encoding.UTF8.GetBytes(message);
            var bytesToSend = new ArraySegment<byte>(messageBytes);
            await WebSocketClient.SendAsync(bytesToSend, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        static async Task ReceiveAndPrintWebsocketResponse()
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
    }
}