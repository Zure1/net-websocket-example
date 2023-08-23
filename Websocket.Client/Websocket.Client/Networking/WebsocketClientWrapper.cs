﻿using System.Net.WebSockets;
using System.Text;
using Websocket.Configuration;

namespace Websocket.Client.Networking
{
    /// <summary>
    /// This class is responsible for the websocket client.
    /// It wraps the <see cref="ClientWebSocket"/> and provides methods to connect to the server and send messages.
    /// </summary>
    internal class WebSocketClientWrapper
    {
        private readonly ClientWebSocket WebSocketClient = new();

        /// <summary>
        /// Connects to the websocket server.
        /// </summary>
        /// <returns></returns>
        public async Task ConnectToServerAsync()
        {
            var serverUri = new Uri($"ws://localhost:{WebSocketConfiguration.Port}/sendTextMessage");
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
            var messageBytes = Encoding.UTF8.GetBytes(message);
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