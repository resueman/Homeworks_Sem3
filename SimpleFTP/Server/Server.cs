using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

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
                        await ProcessRequest(request, streamWriter);
                    }
                });
            }
        }

        private async Task ProcessRequest(string request, StreamWriter streamWriter)
        {
            var regex = new Regex(@"([12]){1}?\s+(.+)");
            var match = regex.Match(request);
            if (!match.Success)
            {
                await streamWriter.WriteAsync("Incorrect request, try again");
            }

            var (command, path) = (int.Parse(match.Groups[1].Value),
                match.Groups[2].Value);

            path = Path.GetFullPath(path);

            switch (command)
            {
                case 1:
                    var (size, list) = List(path);
                    var listResponse = $"{size} ";
                    foreach (var (name, isDirectory) in list)
                    {
                        listResponse += $"{name} " + $"{isDirectory} ";
                    }
                    await streamWriter.WriteLineAsync(listResponse);
                    break;
                case 2:
                    var (contentSize, content) = await Get(path);
                    var sizeInString = $"{contentSize} ";
                    var sizeInBytes = Encoding.UTF8.GetBytes(sizeInString);
                    var response = new byte[sizeInString.Length + contentSize];
                    sizeInBytes.CopyTo(response, 0);
                    content.CopyTo(response, sizeInBytes.Length);
                    await streamWriter.BaseStream.WriteAsync(response);
                    break;
            }
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

            var size = new FileInfo(path).Length;
            var content = new byte[size];
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                await fileStream.ReadAsync(content);
            }

            return (content.Length, content);
        }

        public void Dispose()
        {
            listener.Stop();
        }
    }
}
