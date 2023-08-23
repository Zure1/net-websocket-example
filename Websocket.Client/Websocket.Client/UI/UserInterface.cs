using System;

namespace Websocket.Client.UI
{
    public class UserInterface
    {
        public string GetInputMessage()
        {
            Console.WriteLine("Enter Message to send: ");
            var message = Console.ReadLine();

            return message ?? string.Empty;
        }

        public void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
