using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// Allows to receive from and send messages to server
    /// </summary>
    class Client
    {
        private TcpClient client;
        private NetworkStream stream;
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Creates instance of client
        /// </summary>
        /// <param name="port">port</param>
        /// <param name="hostname">hostname</param>
        public Client(int port, string hostname)
        {
            if (port > IPEndPoint.MaxPort || port < IPEndPoint.MinPort)
            {
                throw new ArgumentOutOfRangeException($"Port number must be from {IPEndPoint.MinPort} to {IPEndPoint.MaxPort}");
            }

            try
            {
                client = new TcpClient(hostname, port);
                cancellationTokenSource = new CancellationTokenSource();
            }
            catch (SocketException e)
            {
                throw new ConnectionToServerException("Connection failed", e);
            }
        }

        public async Task StartAsync()
        {
            using (stream = client.GetStream()) 
            {
                ReceiveMessage();
                await SendMessage();
            }
        }

        public void ReceiveMessage()
        {
            Task.Run(async () =>
            {
                using var reader = new StreamReader(stream);
                var received = await reader.ReadLineAsync();
                while (received != "exit")
                {
                    Console.WriteLine(received);
                    received = await reader.ReadLineAsync();
                }
                cancellationTokenSource.Cancel();
                await Shutdown();
            });
        }

        public Task SendMessage()
        {
            return Task.Run(async() =>
            {
                try
                {
                    using var writer = new StreamWriter(stream) { AutoFlush = true };
                    var writingTasks = new List<Task>();
                    var message = Console.ReadLine();
                    while (message != "exit")
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            break;
                        }
                        writingTasks.Add(writer.WriteLineAsync(message));
                        message = Console.ReadLine();
                    }
                    await Task.WhenAll(writingTasks);
                    await Shutdown();
                }
                catch (SocketException e)
                {
                    throw new ConnectionToServerException("Message sending failed", e);
                }
            });
        }

        /// <summary>
        /// Stops client
        /// </summary>
        public async Task Shutdown()
        {
            using var writer = new StreamWriter(stream) { AutoFlush = true };
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                await writer.WriteLineAsync("exit");
            }
            cancellationTokenSource.Dispose();
            stream.Dispose();
            client.Close();
        }
    }
}
