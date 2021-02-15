using System.Threading.Tasks;

namespace MyNUnit
{
    class Program
    {
        static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            await MyNUnit.Run("nfjjtnt");
        }
    }
}
