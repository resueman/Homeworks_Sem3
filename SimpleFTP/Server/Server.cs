using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace SimpleFTP
{
    /// <summary>
    /// Implments network server that handles request according to file transport protocol
    /// </summary>
    public class Server : IDisposable
    {
        private readonly CancellationTokenSource cts;
        private readonly TcpListener listener;
        private Task mainTask;

        public bool IsStopped { get; private set; }

        /// <summary>
        /// Creates instance of network server
        /// </summary>
        /// <param name="port">Port which will be listened for incoming connections attempts</param>
        public Server(int port = 8888)
        {
            listener = new TcpListener(IPAddress.Any, port);
            cts = new CancellationTokenSource();
        }

        /// <summary>
        /// Stops server
        /// </summary>
        public void Stop()
        {
            listener.Stop();
            cts.Cancel();
            mainTask.Wait();
            Dispose();
        }

        /// <summary>
        /// Starts server for handling requests of clients
        /// </summary>
        public void Start()
        {
            IsStopped = false;
            mainTask = Task.Run(async () =>
            {
                listener.Start();
                while (true)
                {
                    try
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        if (cts.IsCancellationRequested)
                        {
                            return;
                        }
                        HandleClient(client);
                    }
                    catch (Exception e) when (e is InvalidOperationException || e is SocketException)
                    {
                        return;
                    }
                }
            });
        }

        private void HandleClient(TcpClient client)
        {
            Task.Run(async () =>
            {
                try
                {
                    using var stream = client.GetStream();
                    using var streamReader = new StreamReader(stream);
                    using var streamWriter = new StreamWriter(stream) { AutoFlush = true };
                    while (true)
                    {
                        try
                        {
                            var request = await streamReader.ReadLineAsync();
                            await ProcessRequest(request, streamWriter);
                        }
                        catch (Exception e) when (e is IOException || e is InvalidOperationException)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    client.Close();
                    client.Dispose();
                }
            });
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

            switch (command)
            {
                case 1:
                    var (size, list) = List(path);
                    if (size == -1)
                    {
                        await streamWriter.WriteLineAsync(size.ToString());
                        break;
                    }
                    var listResponse = $"{size} ";
                    foreach (var (name, isDirectory) in list)
                    {
                        listResponse += $"{name} " + $"{isDirectory} ";
                    }
                    await streamWriter.WriteLineAsync(listResponse);
                    break;
                case 2:
                    await Get(path, streamWriter);
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

            return (size, directoryContent);
        }

        private async Task Get(string path, StreamWriter streamWriter)
        {
            if (!File.Exists(path))
            {
                await streamWriter.WriteAsync('-');
                await streamWriter.WriteAsync('1');
                return;
            }
            var sizeOfDownloaded = new FileInfo(path).Length;
            await streamWriter.WriteAsync(sizeOfDownloaded + " ");
            using var downloadedStream = new FileStream(path, FileMode.Open);
            await downloadedStream.CopyToAsync(streamWriter.BaseStream);
        }

        /// <summary>
        /// Releases used resourses
        /// </summary>
        public void Dispose()
        {
            listener.Stop();
            cts.Dispose();
            IsStopped = true;
        }
    }
}
