using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleFTP
{
    public class Client : IDisposable
    {
        private readonly TcpClient client;
        private readonly NetworkStream networkStream;
        private readonly StreamReader reader;
        private readonly StreamWriter writer;

        public Client(string hostname = "127.0.0.1", int port = 8888)
        {
            try
            {
                client = new TcpClient(hostname, port);
                networkStream = client.GetStream();
                reader = new StreamReader(networkStream);
                writer = new StreamWriter(networkStream) { AutoFlush = true };
            }
            catch (SocketException e)
            {
                throw new ConnectionToServerException("Connection to server failed", e);
            }
        } 

        public async Task<(int size, IEnumerable<(string name, bool isDirectory)> directoryContent)> List(string pathToDirectory)
        {
            try
            {
                await writer.WriteLineAsync($"1 {pathToDirectory}");

                await WaitForResponse();
                var response = await reader.ReadLineAsync();
                var splitted = response.Replace("  ", " ").Trim().Split(' ');
                var size = int.Parse(splitted[0]);
                var directoryContent = new List<(string name, bool isDirectory)>();
                for (var i = 2; i < splitted.Length; ++i)
                {
                    directoryContent.Add((splitted[i - 1], bool.Parse(splitted[i])));
                    ++i;
                }

                return (size, directoryContent.Count > 0 ? directoryContent : null);
            }
            catch (IOException e)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }

        public async Task<(long size, byte[] content)> Get(string pathToFile)
        {
            try
            {
                await writer.WriteLineAsync($"2 {pathToFile}");

                var responseBytes = await ReceiveBytes();

                var responseString = Encoding.UTF8.GetString(responseBytes);
                var size = Regex.Match(responseString, @"-?\d+\s?").Value;
                if (size.Trim() == "-1")
                {
                    return (-1, null);
                }

                var sizeInBytes = Encoding.UTF8.GetBytes(size);
                var content = new byte[responseBytes.Length - sizeInBytes.Length];
                Array.Copy(responseBytes, sizeInBytes.Length, content, 0, content.Length);

                return (long.Parse(size), content);
            }
            catch (Exception e) when (e is IOException || e is SocketException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }

        private async Task WaitForResponse()
        {
            var timeout = 10;
            var maxTimeout = 10000;
            while (timeout < maxTimeout)
            {
                if (networkStream.DataAvailable)
                {
                    return;
                }
                await Task.Delay(timeout);
                timeout *= 2;
            }
            throw new ConnectionToServerException("Server response timed out");
        }

        private async Task<byte[]> ReceiveBytes()
        {
            await WaitForResponse();

            var response = new byte[0];
            var receiveBuffer = new byte[client.ReceiveBufferSize];
            while (networkStream.DataAvailable)
            {
                var actuallyReceived = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
                Array.Resize(ref response, response.Length + actuallyReceived);
                Array.Copy(receiveBuffer, response, actuallyReceived);
            }

            return response;
        }

        public void Dispose()
        {
            client.Close();
            reader.Dispose();
            writer.Dispose();
        }
    }
}
