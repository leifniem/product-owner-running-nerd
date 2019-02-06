using System;

namespace LoadRunnerClient
{
    public class UiChangedEventArgs : EventArgs
    {
        public readonly string Statename;

        public UiChangedEventArgs(string Statename)
        {
            this.Statename = Statename;
        }
    }

    /// <summary>
    /// Utility Class responsible for handling ViewChanges in the MainWindow
    /// </summary>
    public class UiStateModel
    {
        public event EventHandler<UiChangedEventArgs> OnUiStateChanged;

        private string _CurrentStateName;

        public string State
        {
            get
            {
                return _CurrentStateName;
            }
            set
            {
                if (_CurrentStateName != value)
                {
                    Console.WriteLine("uiStateModel: switch from " + _CurrentStateName + " to " + value);
                    _CurrentStateName = value;
                    OnUiStateChanged(this, new UiChangedEventArgs(_CurrentStateName));
                }
            }
        }
    }
}