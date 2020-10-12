using System.Threading.Tasks;

namespace SimpleFTP
{
    class Program
    {
        static async Task Main()
        {
            using var server = new Server();
            await server.Start();
        }
    }
}
