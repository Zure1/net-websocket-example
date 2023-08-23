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
            await ConnectToServerAsync();
            await HandleWebsocketCommunicationAsync();

            Console.ReadLine();
        }

        static async Task ConnectToServerAsync()
        {
            var serverUri = new Uri($"ws://localhost:{WebSocketConfiguration.Port}/send");
            var isConnected = false;

            Console.WriteLine($"Connecting to {serverUri}... ");

            while (!isConnected)
            {
                try
                {
                    await WebSocketClient.ConnectAsync(serverUri, CancellationToken.None);
                    isConnected = true;
                }
                catch (Exception)
                {
                    Console.WriteLine($"Connection failed! Retrying... ");
                    Thread.Sleep(200);
                }
            }

            Console.WriteLine($"Websocket is now connected to {serverUri}!\n");
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