using System;
using System.IO;
using System.Threading.Tasks;

namespace SimpleFTP
{
    class Program
    {
        private static void PrintOptions()
        {
            Console.WriteLine("Options");
            Console.WriteLine("1 - List files and directories of current directory");
            Console.WriteLine("2 - Get size of file and its byte content");
            Console.WriteLine("3 - Exit");
        }

        static async Task Main()
        {
            PrintOptions();
            var client = new Client();
            while (true)
            {
                Console.WriteLine("\nPlease, choose option");
                var option = Console.ReadLine();
                switch (option)
                {
                    case "1":
                        Console.WriteLine("Enter path to directory");
                        var pathToDirectory = Console.ReadLine();
                        var (sizeOfDirectory, directoryContent) = await client.List(pathToDirectory);
                        Console.WriteLine($"size: {sizeOfDirectory}");
                        foreach (var (name, isDirectory) in directoryContent)
                        {
                            Console.WriteLine($"Name: {name}, is directory: {isDirectory}");
                        }
                        break;
                    case "2":
                        Console.WriteLine("Enter path to file");
                        var pathToFile = Console.ReadLine();
                        var (sizeOfFile, fileContent) = await client.Get(pathToFile);
                        Console.WriteLine($"size: {sizeOfFile}");
                        Console.WriteLine("File successfully downloaded");
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("No such option, try again");
                        break;
                }
            }
        }
    }
}
