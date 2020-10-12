﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleFTP
{
    public class Client
    {
        private readonly int port;

        public Client(int port = 8888)
        {
            this.port = port;
        }

        internal async Task<(int sizeOfDirectory, IEnumerable<(string name, bool isDirectory)> directoryContent)> List(string pathToDirectory)
        {
            await SendRequest($"1 {pathToDirectory}");
            var response = await ReceiveResponse();
            if (response == "-1")
            {
                return (-1, null);
            }
            var splitted = response.Replace("  ", " ").Trim().Split(' ');
            var size = int.Parse(splitted[0]);
            var directoryContent = new List<(string name, bool isDirectory)>();
            for (var i = 2; i < splitted.Length; ++i)
            {
                directoryContent.Add((splitted[i - 1], bool.Parse(splitted[i]));
            }

            return (size, directoryContent);
        }

        internal async Task<(long size, byte[] content)> Get(string pathToFile)
        {
            await SendRequest($"2 {pathToFile}");
            var response = await ReceiveResponse();
            var regex = new Regex(@"(\d+)\s*(.+)?");
            var match = regex.Match(response);
            var size = long.Parse(match.Groups[1].Value[0].ToString());
            var content = Encoding.Unicode.GetBytes(match.Groups[1].Value[1].ToString());

            return (size, content);
        }

        private async Task SendRequest(string request)
        {
            using var client = new TcpClient("localhost", port);
            var stream = client.GetStream();
            var writer = new StreamWriter(stream);
            await writer.WriteLineAsync();
        }

        private async Task<string> ReceiveResponse()
        {
            using var client = new TcpClient("localhost", port);
            var stream = client.GetStream();
            var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
