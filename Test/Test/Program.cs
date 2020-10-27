using System;

namespace Test
{
    class Program
    {
        static async void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 2)
            {
                return;
            }

            var success = int.TryParse(args[0], out int port);
            if (!success)
            {
                return;
            }

            if (args.Length == 2)
            {
                var client = new Client(port, args[1]);
                await client.StartAsync();
            }
            else
            {
                var server = new Server(port);
                await server.StartAsync();
            }
        }
    }
}
