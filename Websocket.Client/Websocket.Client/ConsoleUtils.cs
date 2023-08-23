namespace Websocket.Client
{
	public static class ConsoleUtils
	{
        public static void DisplayFakeProgress(string message, int durationInMilliseconds)
        {
            int steps = 100; // Number of steps in the progress bar
            int delayPerStep = durationInMilliseconds / steps; // Time delay for each step

            for (int i = 0; i <= steps; i++)
            {
                Console.Write($"\r{message}: {i}%");
                Thread.Sleep(delayPerStep);
            }

            Console.WriteLine(); // Move to a new line once the loop is done
        }
    }
}

