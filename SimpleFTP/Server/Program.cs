using System;
using System.Threading.Tasks;

namespace SimpleFTP
{
    class Program
    {
        static async Task Main()
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
