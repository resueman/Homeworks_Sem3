using System.Windows.Input;

namespace GuiForSimpleFTP
{
    /// <summary>
    /// Commands available to execute
    /// </summary>
    public static class Commands
    {
        /// <summary>
        /// Downloads selected file
        /// </summary>
        public static RoutedUICommand Download { get; } = new RoutedUICommand("Download", "Download", typeof(Commands));

        /// <summary>
        /// Downloads all files in explored folder
        /// </summary>
        public static RoutedUICommand DownloadAll { get; } = new RoutedUICommand("Download all", "DownloadAll", typeof(Commands));

        /// <summary>
        /// Returns back from current folder to parent folder, but not above root folder of server
        /// </summary>
        public static RoutedUICommand GoBack { get; } = new RoutedUICommand("Go back", "GoBack", typeof(Commands));

        /// <summary>
        /// Connects client to port by early specified port and hostname
        /// </summary>
        public static RoutedUICommand Connect { get; } = new RoutedUICommand("Connect", "Connect", typeof(Commands));

        /// <summary>
        /// Steps into selected folder
        /// </summary>
        public static RoutedUICommand StepInto { get; } = new RoutedUICommand("Step into", "StepInto", typeof(Commands));

        /// <summary>
        /// Changes download folder
        /// </summary>
        public static RoutedUICommand ChangeDownloadFolder { get; } = new RoutedUICommand("Change download folder", "ChangeDownloadFolder", typeof(Commands));
    }
}
