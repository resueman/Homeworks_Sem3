using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleFTP
{
    public class Server
    {
        private readonly TcpListener listener;

        public Server(int port = 8888)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task Start()
        {
            try
            {
                listener.Start();
                var socket = await listener.AcceptSocketAsync();
                while (true)
                {
                    await Task.Run(async () =>
                    {
                        using var stream = new NetworkStream(socket);
                        using var streamReader = new StreamReader(stream);
                        var request = await streamReader.ReadToEndAsync();
                        var response = await ProcessRequest(request);
                        await SendResponseAsync(response, stream);
                        socket.Close();
                    });
                }
            }
            finally
            {
                listener.Stop();
            }
        }

        private async Task<string> ProcessRequest(string request)
        {
            var regex = new Regex(@"([12]){1}?s+(.+)");
            var match = regex.Match(request);
            if (!match.Success)
            {
                return "Incorrect request, try again";
            }

            var (command, path) = (int.Parse(match.Groups[1].Value[0].ToString()),
                match.Groups[1].Value[1].ToString());

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

        private async Task SendResponseAsync(string response, NetworkStream stream)
        {
            using var streamWriter = new StreamWriter(stream);
            await streamWriter.WriteLineAsync(response);
            await streamWriter.FlushAsync();
        }

        private (int size, IEnumerable<(string name, bool isDirectory)> list) List(string path)
        {
            if (!Directory.Exists(path))
            {
                return (-1, null);
            }

            var directories = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);            
            var size = directories.Length + files.Length;
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
    }
}
