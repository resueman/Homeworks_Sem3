using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GuiForSimpleFTP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            var clientViewModel = ClientViewModel.BuildClientViewModelAsync();
            DataContext = clientViewModel;
            InitializeComponent();
        }

        // Download
        private void DownloadCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ServerContent != null) //
            {
                var selected = ServerContent.SelectedItem as ServerItem;
                e.CanExecute = selected != null && !selected.IsDirectory;
            }
        }

        private void DownloadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(ServerContent.SelectedItem is ServerItem file))
            {
                return;
            }
            if (DataContext is ClientViewModel viewModel)
            {
                viewModel.DownloadFile(file.Name);
            }
        }

        // Download All
        private void DownloadAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = DataContext as ClientViewModel;
            e.CanExecute = viewModel.ServerContent.Any(i => !i.IsDirectory);
        }

        private void DownloadAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is ClientViewModel viewModel)
            {
                viewModel.DownloadAll();
            }
        }

        // Connect
        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = DataContext as ClientViewModel;
            e.CanExecute = viewModel.Address.Length != 0 && 0 <= viewModel.Port && viewModel.Port <= 65535;
        }

        private async void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!(DataContext is ClientViewModel viewModel))
            {
                return;
            }
            viewModel = ClientViewModel.BuildClientViewModelAsync(viewModel.Port, viewModel.Address);
            DataContext = viewModel;
            await viewModel.ConnectToServer();
        }

        // GoBack
        private void GoBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (DataContext is ClientViewModel viewModel) && viewModel.CurrentExplorerPath != ClientViewModel.Root;
        }

        private async void GoBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if ((DataContext is ClientViewModel viewModel))
            {
                await viewModel.GoBackToParentFolder();
            }
        }

        // step into directory
        private void StepIntoCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ServerContent != null) //
            {
                var selected = ServerContent.SelectedItem as ServerItem;
                e.CanExecute = selected != null && selected.IsDirectory;
            }
        }

        private async void StepIntoCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (DataContext is ClientViewModel viewModel)
            {
                var selected = ServerContent.SelectedItem as ServerItem;
                await viewModel.StepIntoFolder(selected.Name);
            }
        }

        // change download folder
    }
}
