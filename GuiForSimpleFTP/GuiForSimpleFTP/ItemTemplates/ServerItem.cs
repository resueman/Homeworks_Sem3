namespace GuiForSimpleFTP
{
    public class ServerItem
    {
        public ServerItem(string name, bool isDirectory)
        {
            Name = name;
            IsDirectory = isDirectory;
            IconSource = isDirectory ? "Resources/folder.ico" : "Resources/file.ico";
        }

        public string Name { get; private set; }

        public bool IsDirectory { get; private set; }

        public string IconSource { get; private set; }
    }
}
