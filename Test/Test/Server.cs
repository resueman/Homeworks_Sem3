using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// Allows to receive from and send messages to client
    /// </summary>
    public class Server
    {
        private readonly TcpListener listener;
        private NetworkStream stream;
        private TcpClient client;
        private StreamWriter writer;
        private StreamReader reader;
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Creates instance of Server class
        /// </summary>
        /// <param name="port">port</param>
        public Server(int port)
        {
            if (port > IPEndPoint.MaxPort || port < IPEndPoint.MinPort)
            {
                throw new ArgumentOutOfRangeException($"Port number must be from {IPEndPoint.MinPort} to {IPEndPoint.MaxPort}");
            }
            listener = new TcpListener(IPAddress.Any, port);
        }

        public async Task StartAsync()
        {
            listener.Start();
            var client = await listener.AcceptTcpClientAsync();

            using (stream = client.GetStream())
            {
                ReceiveMessage();
                await SendMessage();
            }
        }

        private void ReceiveMessage()
        {
            Task.Run(async () =>
            {
                using (reader = new StreamReader(stream))
                {
                    var received = await reader.ReadLineAsync();
                    while (received != "exit")
                    {
                        Console.WriteLine(received);
                        received = await reader.ReadLineAsync();
                    }
                    cancellationTokenSource.Cancel();
                    await Shutdown();
                }
            });
        }

        private Task SendMessage()
        {
            return Task.Run(async () =>
            {
                try
                {
                    using (writer = new StreamWriter(stream) { AutoFlush = true }) 
                    {
                        var writingTasks = new List<Task>();
                        var message = Console.ReadLine();
                        while (message != "exit")
                        {
                            writingTasks.Add(writer.WriteLineAsync(message));
                            message = Console.ReadLine();
                        }
                        await Task.WhenAll(writingTasks);
                        await Shutdown();
                    } 
                }
                catch (SocketException e)
                {
                    throw new ConnectionToServerException("Message sending failed", e);
                }
            });
        }

        /// <summary>
        /// Stops server
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
            reader.Dispose();
            writer.Dispose();
            listener.Stop();            

            Environment.Exit(0);
        }
    }
}
