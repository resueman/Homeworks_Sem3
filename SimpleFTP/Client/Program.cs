﻿using System;
using System.Linq;
using System.Threading.Tasks;

namespace SimpleFTP
{
    /// <summary>
    /// Contains UI and client program's entry point
    /// </summary>
    class Program
    {
        private static void PrintOptions()
        {
            Console.WriteLine("Options");
            Console.WriteLine("1 - List files and directories of current directory");
            Console.WriteLine("2 - Get size of file and its byte content");
            Console.WriteLine("3 - Exit");
        }

        /// <summary>
        /// Clients entry point
        /// </summary>
        static async Task Main()
        {
            PrintOptions();
            using var client = new Client();
            try
            {
                client.Connect();
                while (true)
                {
                    Console.WriteLine("\nPlease, choose option");
                    var option = Console.ReadLine();
                    switch (option)
                    {
                        case "1":
                            Console.Write("Path to directory: ");
                            var pathToDirectory = Console.ReadLine();
                            var directoryContent = await client.List(pathToDirectory);
                            Console.WriteLine($"Number of files and directories in current directory: {directoryContent.Count}");
                            foreach (var (name, isDirectory) in directoryContent)
                            {
                                Console.WriteLine($"Name: {name}, is directory: {isDirectory}");
                            }
                            break;
                        case "2":
                            Console.WriteLine("Path to file: ");
                            var pathToFile = Console.ReadLine();
                            var path = await client.Get(pathToFile, "./Downloads");
                            Console.WriteLine($"File successfully downloaded, path: {path}");
                            break;
                        case "3":
                            return;
                        default:
                            Console.WriteLine("No such option, try again");
                            break;
                    }
                }
            }
            catch (ConnectionToServerException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
