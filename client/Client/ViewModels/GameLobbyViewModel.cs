using LoadRunnerClient.DTOs;
using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LoadRunnerClient
{
    /// <summary>
    /// GameLobbyViewModel
    /// </summary>
    public class GameLobbyViewModel : ObservableViewModelBase
    {
		private TaskFactory taskFactory;

		private UiStateModel uiStateModel;

        private GameLobbyModel _model;

        private static GameLobbyViewModel instance;


        /// <summary>
        /// GetInstance Method
        /// </summary>
        /// <param name="uiStateModel"></param>
        /// <returns>An instance of GameLobbyViewModel </returns>
        public static GameLobbyViewModel getInstance(UiStateModel uiStateModel)
        {
            if (instance == null)
            {
                instance = new GameLobbyViewModel(uiStateModel);
            }
            return instance;
        }

        //Sections that get displayed as buttons
        private readonly ObservableCollection<SectionModel> _sections = new ObservableCollection<SectionModel>();

        /// <summary>
        /// Sections which get displayed on the screen
        /// </summary>
        public ObservableCollection<SectionModel> Sections { get { return _sections; } }

        /// <summary>
        /// Standart constructor
        /// </summary>
        /// <param name="uiStateModel"></param>
        private GameLobbyViewModel(UiStateModel uiStateModel)
        {
			taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
			this.uiStateModel = uiStateModel;
            this.PropertyChanged += shown;
        }

        //if client is owner of the session -> the the play button is displayed
        private bool _owner;

        /// <summary>
        /// Owner Property 
        /// </summary>
        public bool Owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                {
                    _owner = value;
                    OnPropertyChanged("owner");
                }
            }
        }

        //current session
        private SessionDTO _curSession;

        /// <summary>
        /// CurrentSession
        /// </summary>
        public SessionDTO curSession
        {
            get => _curSession;
            set
            {
                if (_curSession != value)
                {
                    _curSession = value;
                    OnPropertyChanged("curSession");
                }
            }
        }

        /// <summary>
        /// Method which listens to the PropertyChanged of the class and checks if the View is shown, if so
        /// the initView is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void shown(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "show")
            {
                if (show)
                {
                    initView();
                }
            }
        }

        /// <summary>
        /// InitView Method which adds a number of SectionModels to the ObservableCollection Sections
        /// </summary>
        private void initView()
        {
            //model is initialized here because the sectionmodels will be created when initialized
            this._model = new GameLobbyModel(curSession);
			taskFactory.StartNew(() =>
			{
				Sections.Clear();
				foreach (SectionModel section in _model.Sections)
				{
					Sections.Add(section);
				}
			}).Wait();			
            _model.AddListener();
            _model.PropertyChanged += _model_PropertyChanged;
			_model.GameLobbyKick += KickListener;
        }

		/// <summary>
		/// Listens to a KickEvent and returns to the ServerList
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void KickListener(object sender, EventArgs e)
		{
			_model.ExitLobby();
			_model.ClientChannelHandler.sessionChannel.Close();
			uiStateModel.State = "ServerList";
		}

		/// <summary>
		/// PropertyChanged Listener which checks if the game will be started,
		/// if the game is started the uiStateModel State will be set to Game
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"> Property which was changed</param>
		private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "startProperty")
            {
                //the GameVM section is important to tell the client which part of the map he has to visualize
                GameViewModel.getInstance(uiStateModel).Section = _model.selectedSection - 1;
				_model.ExitLobby();				
                uiStateModel.State = "Game";
            }
        }

        private ICommand _playCommand;

        /// <summary>
        /// PlayCommand to start the game, available to SessionOwner
        /// </summary>
        public ICommand PlayCommand
        {
            get
            {
                if (_playCommand == null)
                {
                    _playCommand = new ActionCommand(e => StartGame());
                }
                return _playCommand;
            }
        }

        /// <summary>
        /// Sends a StartGameMessage
        /// </summary>
        private void StartGame()
        {
            _model.ClientChannelHandler.sendStartGameMessage();
        }

		private ICommand _backCommand;
		public ICommand BackCommand{
			get {
				if(_backCommand == null){
					_backCommand = new ActionCommand(e => BackToLobby());
				}
				return _backCommand;
			}
		}

		public void BackToLobby(){
			_model.sendDeselectSectionMessage(_model.selectedSection);
			_model.ClientChannelHandler.sendPlayerQuitMessage();			
		}
    }
}