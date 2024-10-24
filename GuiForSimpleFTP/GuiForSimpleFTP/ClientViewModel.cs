﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using SimpleFTP;

namespace GuiForSimpleFTP
{
    /// <summary>
    /// Identifies view model of client
    /// </summary>
    public class ClientViewModel : INotifyPropertyChanged
    {
        private int port = 8888;
        private string address = "127.0.0.1";
        private Client client;
        private string downloadFolder;
        private string errorMessage;

        /// <summary>
        /// Path to root folder on server, above which it's impossible to view content
        /// </summary>
        public static string Root { get; } = "./";

        /// <summary>
        /// Shows if client connected to server
        /// </summary>
        public bool IsConnected { get; private set; } = false;

        /// <summary>
        /// Folder on the client where files from server will be downloaded
        /// </summary>
        public string DownloadFolder 
        {
            get => downloadFolder; 
            set 
            { 
                downloadFolder = value; 
                OnPropertyChanged(nameof(DownloadFolder));
            }
        }

        /// <summary>
        /// The path to the folder the content of which we are currently viewing
        /// </summary>
        public string CurrentExplorerPath { get; private set; } = Root;

        /// <summary>
        /// Port to connect
        /// </summary>
        public int Port
        {
            get => port;
            private set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        /// <summary>
        /// Hostname
        /// </summary>
        public string Address
        {
            get => address;
            private set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage
        {
            get => errorMessage;
            private set
            {
                errorMessage = value;
                OnErrorOccured();
            }
        }

        /// <summary>
        /// Error occured event
        /// </summary>
        public event PropertyChangedEventHandler ErrorOccured;

        /// <summary>
        /// Called when new error occurs
        /// </summary>
        public void OnErrorOccured()
        {
            ErrorOccured?.Invoke(this, new PropertyChangedEventArgs(ErrorMessage));
        }

        /// <summary>
        /// Downloading files
        /// </summary>
        public ObservableCollection<Download> Downloads { get; private set; }

        private ObservableCollection<ServerItem> serverContent;

        /// <summary>
        /// Content of currenr explored folder
        /// </summary>
        public ObservableCollection<ServerItem> ServerContent
        {
            get => serverContent;
            private set
            {
                serverContent = value;
                OnPropertyChanged(nameof(ServerContent));
            }
        }

        private ClientViewModel(int port, string hostname)
        {
            Port = port;
            Address = hostname;
            Downloads = new ObservableCollection<Download>();
            ServerContent = new ObservableCollection<ServerItem>();
            client = new Client(Address, Port);
            if (!Directory.Exists("./Downloads/"))
            {
                Directory.CreateDirectory("./Downloads/");
            }
            DownloadFolder = "./Downloads/";
        }

        /// <summary>
        /// Creates instance of ClientViewModel class with default values
        /// </summary>
        /// <returns>Instance of ClientViewModel class</returns>
        public static ClientViewModel BuildClientViewModelAsync() => new ClientViewModel(8888, "127.0.0.1");

        /// <summary>
        /// Creates instance of ClientViewModel class
        /// </summary>
        /// <param name="port">Port</param>
        /// <param name="hostname">Hostname</param>
        /// <returns>Instance of ClientViewModel class</returns>
        public static ClientViewModel BuildClientViewModelAsync(int port, string hostname)
        {
            return new ClientViewModel(port, hostname);
        }

        /// <summary>
        /// Connects client to port by early specified port and hostname
        /// </summary>
        public async Task ConnectToServer()
        {
            try
            {
                await client.ConnectAsync();
                IsConnected = true;
                await ListServerContent(Root);
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// Lists content of current folder on server
        /// </summary>
        /// <param name="path">Path of folder to list</param>
        private async Task ListServerContent(string path)
        {
            try
            {
                var content = await client.List(path);
                var serverContent = content == null
                    ? new List<ServerItem>()
                    : content.AsParallel()
                          .Select(item => new ServerItem(Path.GetFileName(item.name), item.isDirectory))
                          .OrderBy(item => item.IsDirectory).ToList();

                ServerContent.Clear();
                foreach (var item in serverContent)
                {
                    ServerContent.Add(item);
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// Returns back from current folder to parent folder, but not above root folder of server
        /// </summary>
        public async Task GoBackToParentFolder()
        {
            try
            {
                CurrentExplorerPath = CurrentExplorerPath != Root
                    ? CurrentExplorerPath.Substring(0, CurrentExplorerPath.LastIndexOf("\\"))
                    : Root;
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }

            await ListServerContent(CurrentExplorerPath);
        }

        /// <summary>
        /// Steps into selected folder
        /// </summary>
        /// <param name="folderName">Name of folder to step in</param>
        public async Task StepIntoFolder(string folderName)
        {
            try
            {
                if (ServerContent.First(item => item.Name == folderName).IsDirectory)
                {
                    CurrentExplorerPath = CurrentExplorerPath += $"\\{folderName}";

                    await ListServerContent(CurrentExplorerPath);
                }
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
        }

        /// <summary>
        /// Changes Download folder
        /// </summary>
        /// <param name="newDownloadPath">Path to new download folder</param>
        public void ChangeDownloadFolder(string newDownloadPath)
        {
            DownloadFolder = newDownloadPath;
        }

        /// <summary>
        /// Downloads selected file from server
        /// </summary>
        /// <param name="fileName">Name of file to download</param>
        public void DownloadFile(string fileName)
        {
            var download = new Download(fileName, 0);
            Downloads.Add(download);
            Task.Run(async () =>
            {
                try
                {
                    using var client = new Client(Address, Port);
                    client.Connect();
                    var pathToFile = Path.Combine(CurrentExplorerPath, fileName);
                    download.ProgressValue = 0;
                    await client.Get(pathToFile, DownloadFolder);
                    download.ProgressValue = 100;
                }
                catch(Exception e)
                {
                    ErrorMessage = e.Message;
                }
            });
        }

        /// <summary>
        /// Downloads all files from current explored folder on server
        /// </summary>
        public void DownloadAll()
        {
            foreach (var file in ServerContent.Where(i => !i.IsDirectory))
            {
                DownloadFile(file.Name);
            }
        }

        /// <summary>
        /// Property change event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Called when a property has changed, notifies subscribers to the property, that it has changed 
        /// </summary>
        /// <param name="PropertyName">Name of changed property</param>
        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
