using System;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Class for user interaction
    /// </summary>
    class Program
    {
        public static async Task Main()
        {
            Console.WriteLine("Введите путь: ");
            var path = Console.ReadLine();
            await MyNUnit.Run(path);
        }
    }
}
