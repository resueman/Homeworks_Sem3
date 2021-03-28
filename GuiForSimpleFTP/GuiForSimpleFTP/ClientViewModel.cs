using System;
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
    public class ClientViewModel : INotifyPropertyChanged
    {
        private int port = 8888;
        private string address = "127.0.0.1";
        private Client client;

        public bool IsConnected { get; set; } = false;

        private string downloadFolder;
        public string DownloadFolder 
        {
            get => downloadFolder; 
            set 
            { 
                downloadFolder = value; 
                OnPropertyChanged(nameof(DownloadFolder));
            }
        }

        public string CurrentExplorerPath { get; private set; } = Root;

        public static string Root { get; } = "./";

        public int Port
        {
            get => port;
            set
            {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }

        public string Address
        {
            get => address;
            set
            {
                address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public ObservableCollection<Download> Downloads { get; set; }

        private ObservableCollection<ServerItem> serverContent;

        public ObservableCollection<ServerItem> ServerContent
        {
            get => serverContent;
            set
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

        public static ClientViewModel BuildClientViewModelAsync()
        {
            return new ClientViewModel(8888, "127.0.0.1");
        }

        public static ClientViewModel BuildClientViewModelAsync(int port, string hostname)
        {
            return new ClientViewModel(port, hostname);
        }

        public async Task ConnectToServer()
        {
            try
            {
                client.Connect();
                IsConnected = true;
                await ListServerContent(Root);
            }
            catch (Exception e)
            {

            }
        }

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

            }
        }

        // click on '<-' button
        public async Task GoBackToParentFolder()
        {
            CurrentExplorerPath = CurrentExplorerPath != Root
                ? CurrentExplorerPath.Substring(0, CurrentExplorerPath.LastIndexOf("\\"))
                : Root;

            await ListServerContent(CurrentExplorerPath);
        }

        // double click in explorer
        public async Task StepIntoFolder(string folderName)
        {
            if (ServerContent.First(item => item.Name == folderName).IsDirectory)
            {
                CurrentExplorerPath = CurrentExplorerPath += $"\\{folderName}";

                await ListServerContent(CurrentExplorerPath);
            }
        }

        // 'change' button - opens explorer
        public void ChangeDownloadFolder(string newDownloadPath)
        {
            DownloadFolder = newDownloadPath;
        }

        public void DownloadFile(string fileName)
        {
            var download = new Download(fileName, 0);
            Downloads.Add(download);
            Task.Run(async () =>
            {
                using var client = new Client(Address, Port);
                client.Connect();
                var pathToFile = Path.Combine(CurrentExplorerPath, fileName);
                await client.Get(pathToFile, DownloadFolder);
            });
        }

        public void DownloadAll()
        {
            foreach (var file in ServerContent.Where(i => !i.IsDirectory))
            {
                DownloadFile(file.Name);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
