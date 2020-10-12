using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleFTP
{
    public class Client : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream stream;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public Client()
        {
            client = new TcpClient();
            stream = client.GetStream();
            reader = new StreamReader(stream);
            writer = new StreamWriter(stream) { AutoFlush = true };
        }

        public async Task<bool> ConnectToSever(string hostname = "127.0.0.1", int port = 8888)
        {
            var timeout = 10;
            var maxTimeout = 1000000;
            while (timeout < maxTimeout)
            {
                try
                {
                    await client.ConnectAsync(hostname, port);
                    return true;
                }
                catch (SocketException)
                {
                    Thread.Sleep(timeout);
                    timeout *= 2;
                }
            }
            return false;
        }

        public async Task<(int size, IEnumerable<(string name, bool isDirectory)> directoryContent)> List(string pathToDirectory)
        {
            await writer.WriteLineAsync($"1 {pathToDirectory}");

            var response = await reader.ReadLineAsync();
            var splitted = response.Replace("  ", " ").Trim().Split(' ');
            var size = int.Parse(splitted[0]);
            var directoryContent = new List<(string name, bool isDirectory)>();
            for (var i = 2; i < splitted.Length; ++i)
            {
                directoryContent.Add((splitted[i - 1], bool.Parse(splitted[i])));
            }

            return (size, directoryContent.Count > 0 ? directoryContent : null);
        }

        public async Task<(long size, byte[] content)> Get(string pathToFile)
        {
            await writer.WriteLineAsync($"2 {pathToFile}");

            var response = await reader.ReadLineAsync();
            var regex = new Regex(@"(\d+)\s*(.+)?");
            var match = regex.Match(response);
            var size = long.Parse(match.Groups[1].Value[0].ToString());
            var content = Encoding.Unicode.GetBytes(match.Groups[1].Value[1].ToString());

            return (size, content);
        }

        public void Dispose()
        {
            Console.WriteLine("Disposed");
            client.Close();
            stream.Dispose();
            reader.Dispose();
            writer.Dispose();
        }
    }
}
