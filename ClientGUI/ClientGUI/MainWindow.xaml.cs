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

namespace ClientGUI
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
            e.CanExecute = true;
        }

        private void DownloadCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Download All
        private void DownloadAllCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void DownloadAllCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        // Connect
        private void ConnectCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var viewModel = DataContext as ClientViewModel;
            e.CanExecute = viewModel.Address.Length != 0 && 0 <= viewModel.Port && viewModel.Port <= 65535;
        }

        private async void ConnectCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var viewModel = DataContext as ClientViewModel;
            var clientViewModel = await ClientViewModel.BuildClientViewModelAsync(viewModel.Port, viewModel.Address);
            DataContext = clientViewModel;
            clientViewModel.ConnectToServer();
        }

        // GoBack
        private void GoBackCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void GoBackCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
