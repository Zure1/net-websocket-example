using Websocket.Client.Networking;
using Websocket.Client.UI;

namespace Websocket.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var webSocketClientWrapper = new WebSocketClientWrapper();

            await webSocketClientWrapper.ConnectToServerAsync();

            _ = ListenToServerResponses(webSocketClientWrapper);
            _ = HandleUserInputs(webSocketClientWrapper);
        }

        static async Task ListenToServerResponses(WebSocketClientWrapper webSocketClientWrapper)
        {
            while (webSocketClientWrapper.IsConnected())
            {
                await webSocketClientWrapper.ReceiveAndPrintServerResponse();
            }
        }

        static async Task HandleUserInputs(WebSocketClientWrapper webSocketClientWrapper)
        {
            while (webSocketClientWrapper.IsConnected())
            {
                var message = UserInterface.GetInputMessage();
                if (!string.IsNullOrWhiteSpace(message))
                {
                    await webSocketClientWrapper.SendWebsocketMessage(message);
                }

                // Wait short so that the "enter message" is printed after the server response
                Thread.Sleep(50);
            }
        }
    }
}