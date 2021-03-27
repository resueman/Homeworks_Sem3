using SimpleFTP;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace ClientGUI
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private int port = 8888;
        private string address = "127.0.0.1";

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

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        private string downloadPath;
        private string currentExplorerPath;
        const string root = "./";
        private Client client;

        private ClientViewModel(int port, string hostname)
        {
            Port = port;
            Address = hostname;
            Downloads = new ObservableCollection<Download>();
            ServerContent = new ObservableCollection<ServerItem>();
            client = new Client(Address, Port);
        }

        public static ClientViewModel BuildClientViewModelAsync()
        {
            return new ClientViewModel(8888, "127.0.0.1");
        }

        public static async Task<ClientViewModel> BuildClientViewModelAsync(int port, string hostname)
        {
            var clientViewModel = new ClientViewModel(port, hostname);
            await clientViewModel.ListServerContent(root);
            
            return clientViewModel;
        }

        public ObservableCollection<Download> Downloads { get; set; }

        public ObservableCollection<ServerItem> ServerContent { get; set; }

        private async Task ListServerContent(string path)
        {
            try
            {
                var (_, content) = await client.List(path);
                var serverContent = content.AsParallel()
                    .Select(item => new ServerItem(item.name, item.isDirectory))
                    .OrderBy(item => item.IsDirectory);

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
            currentExplorerPath = currentExplorerPath != root
                ? currentExplorerPath.Substring(0, currentExplorerPath.LastIndexOf('/') - 1)
                : root;

            await ListServerContent(currentExplorerPath);
        }

        // double click in explorer
        public async Task StepIntoFolder(string itemName)
        {
            if (ServerContent.First(item => item.Name == itemName).IsDirectory) 
            {
                currentExplorerPath = Path.Combine(currentExplorerPath, itemName);

                await ListServerContent(currentExplorerPath);
            }
        }

        // 'connect' button
        public void ConnectToServer()
        {
            try
            {
                client = new Client(Address, Port);
            }
            catch (Exception e)
            {

            }
        }

        // 'change' button - opens explorer

        //using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
        //{
        //    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
        //    var selectedDirectory = dialog.SelectedPath;
        //}
        public void ChangeDownloadFolder(string newDownloadPath)
        {
            downloadPath = newDownloadPath;
        }

        // 'download button'
        public void DownloadFile(string fileName)
        {
            var download = new Download(fileName, 0);
            Downloads.Add(download);
            Task.Run(async () =>
            {
                try
                {
                    var client = new Client(Address, Port);
                    await client.Get(fileName, downloadPath);
                    // dispose
                }
                catch (Exception e)
                {

                }
            });
        }

        // 'download all' button
        public async Task DownloadAllFilesInCurrentDirectory()
        {
            foreach (var file in ServerContent.Where(i => !i.IsDirectory))
            {
                DownloadFile(file.Name);
            }
        }
    }
}
