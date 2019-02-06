using System.ComponentModel;

namespace LoadRunnerClient
{
    /// <summary>
    /// Observable Model Base which implements the INotifyPropertyChanged
    /// </summary>
    public class ObservableModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}