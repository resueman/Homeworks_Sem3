using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleFTP
{
    public class Server : IDisposable
    {
        private readonly TcpListener listener;

        public Server(int port = 8888)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task Start()
        {
            listener.Start();
            while (true)
            {
                using var client = await listener.AcceptTcpClientAsync();
                await Task.Run(async () =>
                {
                    using var stream = client.GetStream();
                    using var streamReader = new StreamReader(stream);
                    using var streamWriter = new StreamWriter(stream) { AutoFlush = true };
                    while (true)
                    {
                        var request = await streamReader.ReadLineAsync();
                        var response = await ProcessRequest(request);
                        await streamWriter.WriteLineAsync(response);
                    }
                });
            }
        }

        private async Task<string> ProcessRequest(string request)
        {
            var regex = new Regex(@"([12]){1}?\s+(.+)");
            var match = regex.Match(request);
            if (!match.Success)
            {
                return "Incorrect request, try again";
            }

            var (command, path) = (int.Parse(match.Groups[1].Value),
                match.Groups[2].Value);

            var response = "";
            switch (command)
            {
                case 1:
                    var (size, list) = List(path);
                    response = $"{size} ";
                    foreach (var (name, isDirectory) in list)
                    {
                        response += $"{name} " + $"{isDirectory} ";
                    }
                    break;
                case 2:
                    var (contentSize, content) = await Get(path);
                    response = $"{contentSize} " + $"{content}";
                    break;
            }

            return response;
        }

        private (int size, IEnumerable<(string name, bool isDirectory)> list) List(string path)
        {
            if (!Directory.Exists(path))
            {
                return (-1, null);
            }

            var files = Directory.GetFiles(path);
            var directories = Directory.GetDirectories(path);
            var size = files.Length + directories.Length;
            var directoryContent = new List<(string, bool)>();
            directoryContent.AddRange(files.Select(file => (file, false)));
            directoryContent.AddRange(directories.Select(directory => (directory, true)));

            return (size , directoryContent);
        }

        private async Task<(long size, byte[] content)> Get(string path)
        {
            if (!File.Exists(path))
            {
                return (-1, null);
            }

            var content = await File.ReadAllBytesAsync(path);

            return (content.Length, content);
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
