using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Class for user interaction
    /// </summary>
    class Program
    {
        public static async Task Main(string[] args)
        {
            foreach (var path in args)
            {
                await MyNUnit.Run(path);
            }
        }
    }
}
