using Websocket.Client.Networking;
using Websocket.Client.UI;

namespace Websocket.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var webSocketClientWrapper = new WebSocketClientWrapper();
            var userInterface = new UserInterface();

            await webSocketClientWrapper.ConnectToServerAsync();

            _ = ListenToServerResponses(webSocketClientWrapper);
            _ = HandleUserInputs(webSocketClientWrapper, userInterface);
        }

        static async Task ListenToServerResponses(WebSocketClientWrapper webSocketClientWrapper)
        {
            while (webSocketClientWrapper.IsConnected())
            {
                await webSocketClientWrapper.ReceiveAndPrintServerResponse();
            }
        }

        static async Task HandleUserInputs(WebSocketClientWrapper webSocketClientWrapper, UserInterface userInterface)
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