using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SimpleFTP
{
    /// <summary>
    /// Implements network client that follows file transport protocol 
    /// </summary>
    public class Client : IDisposable
    {
        private readonly string hostname;
        private readonly int port;
        private TcpClient client;
        private NetworkStream networkStream;
        private StreamReader reader;
        private StreamWriter writer;

        /// <summary>
        /// Creates instance of network client
        /// </summary>
        /// <param name="hostname">Hostname</param>
        /// <param name="port">Port</param>
        public Client(string hostname = "127.0.0.1", int port = 8888)
        {
            this.hostname = hostname;
            this.port = port;
        }

        /// <summary>
        /// Connects to server
        /// </summary>
        public void Connect()
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

        public async Task ConnectAsync()
        {
            var timeout = 10;
            var maxTimeout = 100000;
            while (timeout < maxTimeout && client == null)
            {
                try
                {
                    client = new TcpClient(hostname, port);
                    networkStream = client.GetStream();
                    reader = new StreamReader(networkStream);
                    writer = new StreamWriter(networkStream) { AutoFlush = true };
                }
                catch (SocketException)
                {
                    await Task.Delay(timeout);
                    timeout *= 2;
                }
            }
            if (client == null)
            {
                throw new ConnectionToServerException("Server response timed out");
            }
        }

        /// <summary>
        /// Closes client
        /// </summary>
        public void Close()
        {
            client.Close();
            client.Dispose();
        }

        /// <summary>
        /// Makes a listing of files and directories in a specified directory on the server
        /// </summary>
        /// <param name="pathToDirectory">Path to directory, which content should be listed</param>
        /// <returns>Content of specified directory according to file transport protocol</returns>
        public async Task<List<(string name, bool isDirectory)>> List(string pathToDirectory)
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

                return directoryContent.Count > 0 ? directoryContent : null;
            }
            catch (Exception e) when (e is IOException || e is SocketException || e is ObjectDisposedException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
        }

        /// <summary>
        /// Returns file content
        /// </summary>
        /// <param name="sourcePath">Path to file on server that will be download</param>
        /// <param name="destinationFolder">Folder where file would be downloaded</param>
        /// <returns>Size and file content according to file transport protocol</returns>
        public async Task<string> Get(string sourcePath, string destinationFolder)
        {
            var path = $"{destinationFolder}\\{Path.GetFileName(sourcePath)}";
            try
            {
                await writer.WriteLineAsync($"2 {sourcePath}");

                var size = await ReadSize();
                if (size == -1)
                {
                    throw new FileNotFoundException($"{sourcePath} doesn't exist");
                }    
                
                await Download(size, path);
            }
            catch (Exception e) when (e is IOException || e is SocketException || e is ObjectDisposedException)
            {
                throw new ConnectionToServerException(e.Message, e);
            }
            return path;
        }

        private async Task<long> ReadSize()
        {
            await WaitForResponse();

            var buffer = new byte[long.MaxValue.ToString().Length + 1];
            networkStream.Read(buffer, 0, 2);

            if (Convert.ToChar(buffer[0]) == '-' && Convert.ToChar(buffer[1]) == '1')
            {
                return -1;
            }

            var currentPosition = 1;
            while (buffer[currentPosition] != ' ')
            {
                ++currentPosition;
                await networkStream.ReadAsync(buffer, currentPosition, 1);
            }
            var sizeAsByteArray = new byte[currentPosition + 1];
            Array.Copy(buffer, sizeAsByteArray, currentPosition + 1);
            var size = "";
            foreach (var b in sizeAsByteArray)
            {
                size += Convert.ToChar(b);
            }
            return long.Parse(size);
        }

        private async Task Download(long size, string pathToDownload)
        {
            using var fileStream = File.Create(pathToDownload);

            var bufferSize = 1024;
            var buffer = new byte[bufferSize];
            for (var i = 0; i < size / bufferSize; ++i)
            {
                await Copy(buffer, reader.BaseStream, fileStream);
            }

            buffer = new byte[size % bufferSize];
            await Copy(buffer, reader.BaseStream, fileStream);
        }

        private async Task Copy(byte[] buffer, Stream source, Stream destination)
        {
            if (buffer.Length == 0)
            {
                return;
            }
            await source.ReadAsync(buffer, 0, buffer.Length);
            await destination.WriteAsync(buffer, 0, buffer.Length);
        }

        private async Task WaitForResponse()
        {
            var timeout = 10;
            var maxTimeout = 100000;
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

        /// <summary>
        /// Releases used resources
        /// </summary>
        public void Dispose()
        {
            client.Close();
            reader.Dispose();
            writer.Dispose();
        }
    }
}
