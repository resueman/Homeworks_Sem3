using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GuiForSimpleFTP
{
    /// <summary>
    /// Identifies downloading file
    /// </summary>
    public class Download : INotifyPropertyChanged
    {
        private int progress;

        public Download(string name, int progressValue)
        {
            Name = name;
            ProgressValue = progressValue;
        }

        /// <summary>
        /// Name of downloaded file
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Shows the percentage of downloaded file
        /// </summary>
        public int ProgressValue
        {
            get => progress;
            set
            {
                progress = value;
                OnPropertyChanged(nameof(ProgressValue));
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
