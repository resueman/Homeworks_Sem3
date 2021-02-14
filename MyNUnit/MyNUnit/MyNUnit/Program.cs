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
            var testRunner = new TestRunner();
            await testRunner.Run("nfjjtnt");
        }
    }
}
