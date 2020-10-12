using System;
using System.Threading.Tasks;

namespace SimpleFTP
{
    class Program
    {
        static async Task Main()
        {
            var server = new Server();
            await server.Start();
        }
    }
}
