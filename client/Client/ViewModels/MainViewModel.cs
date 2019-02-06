using System.Collections.Generic;

namespace LoadRunnerClient
{
    /// <summary>
    /// MainViewModel which controls the current viewmodel shown
    /// </summary>
    public class MainViewModel : ObservableViewModelBase
    {
        /// <summary>
        /// ViewModel dictionary which contains a string (name of the view) and the viewmodel
        /// </summary>
        private Dictionary<string, ObservableViewModelBase> viewmodels = new Dictionary<string, ObservableViewModelBase>();

        /// <summary>
        /// UIStateManager
        /// </summary>
        private UiStateModel uiStateModel = new UiStateModel();

        /// <summary>
        /// Property for the currentViewModel
        /// </summary>
        private ObservableViewModelBase _currentViewModel;

        /// <summary>
        /// Property for the currentViewModel
        /// </summary>
        public ObservableViewModelBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged("CurrentViewModel");
            }
        }

        /// <summary>
        /// Method to change the view of the window
        /// Sets the show value of the current viewmodel to false and the show value of the new vm to true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void changeToView(object sender, UiChangedEventArgs args)
        {
            CurrentViewModel.show = false;
            CurrentViewModel = viewmodels[args.Statename];
            CurrentViewModel.show = true;
        }

        /// <summary>
        /// Constructor which initializes the viewmodels with a uIStateModel
        /// </summary>
        public MainViewModel()
        {
            viewmodels.Add("Login", new LoginViewModel(uiStateModel));
            viewmodels.Add("ServerList", ServerListViewModel.getInstance(uiStateModel));
            viewmodels.Add("GameLobby", GameLobbyViewModel.getInstance(uiStateModel));
            viewmodels.Add("CreateSession", CreateSessionViewModel.getInstance(uiStateModel));
            viewmodels.Add("Game", GameViewModel.getInstance(uiStateModel));
            viewmodels.Add("Editor", EditorViewModel.getInstance(uiStateModel));
            viewmodels.Add("GameEnd", GameEndViewModel.getInstance(uiStateModel));
            uiStateModel.OnUiStateChanged += changeToView;
            CurrentViewModel = viewmodels["Login"];
        }
    }
}