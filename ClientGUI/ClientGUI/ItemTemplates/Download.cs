using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ClientGUI
{
    public class Download : INotifyPropertyChanged
    {
        private int progress;

        public Download(string name, int progressValue)
        {
            Name = name;
            ProgressValue = progressValue;
        }

        // downloading path

        public string Name { get; private set; }

        public int ProgressValue
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged(nameof(ProgressValue));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}
