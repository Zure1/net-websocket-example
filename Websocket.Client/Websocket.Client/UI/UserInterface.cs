using System;

namespace Websocket.Client.UI
{
    /// <summary>
    /// This class is responsible for the user interface.
    /// </summary>
    public class UserInterface
    {
        /// <summary>
        /// Prints a message to the console which asks the user to enter a message and returns the message.
        /// </summary>
        /// <returns>The message entered by the user.</returns>
        public static string GetInputMessage()
        {
            Console.WriteLine("Enter Message to send: ");
            var message = Console.ReadLine();

            return message ?? string.Empty;
        }

        /// <summary>
        /// Prints a message to the console.
        /// </summary>
        /// <param name="message">The message that gets printed to the console.</param>
        public static void PrintMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
