using System.ComponentModel;
using System.Windows.Threading;

namespace LoadRunnerClient
{
	/// <summary>
	/// Base class for ViewModels to be able to handle their own visibility
	/// </summary>
    public class ObservableViewModelBase : INotifyPropertyChanged
    {        
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        
        private bool _show = false;

        /// <summary>
        /// Show property to check if the view is shown
        /// </summary>
        public bool show
        {
            get => _show;
            set
            {
                if (_show != value)
                {
                    _show = value;
                    OnPropertyChanged("show");
                }
            }
        }
    }
}