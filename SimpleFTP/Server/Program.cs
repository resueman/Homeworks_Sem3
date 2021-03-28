using System;

namespace SimpleFTP
{
    /// <summary>
    /// Contains UI and server program's entry point
    /// </summary>
    class Program
    {
        /// <summary>
        /// Entry point
        /// </summary>
        static void Main()
        {
            using var server = new Server();
            server.Start();
            var input = Console.ReadKey();
            while (input.Key != ConsoleKey.Escape)
            {
                input = Console.ReadKey();
            }
        }
    }
}
