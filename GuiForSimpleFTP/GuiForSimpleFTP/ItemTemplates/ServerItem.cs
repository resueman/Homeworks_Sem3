namespace GuiForSimpleFTP
{
    /// <summary>
    /// Identifies folder or file on server
    /// </summary>
    public class ServerItem
    {
        public ServerItem(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
            IconSource = isDirectory ? "Resources/folder.ico" : "Resources/file.ico";
        }

        /// <summary>
        /// Name of file with extension on name of folder
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Identifies if server item is directory or file
        /// </summary>
        public bool IsDirectory { get; private set; }

        /// <summary>
        /// Path to corresponding icon
        /// </summary>
        public string IconSource { get; private set; }
    }
}
