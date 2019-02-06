using LoadRunnerClient.DTOs;
using LoadRunnerClient.MapAndModel.ViewModel;
using LoadRunnerClient.Util;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static LoadRunnerClient.MapAndModel.ViewModel.CreateSessionModel;

namespace LoadRunnerClient
{
    /// <summary>
    /// CreateSessionViewModel
    /// </summary>
    public class CreateSessionViewModel : ObservableViewModelBase
    {
        private UiStateModel uiStateModel;
        private CreateSessionModel _model;
        private TaskFactory taskFactory;
        private static CreateSessionViewModel instance;
		public EventHandler<ErrorMessageEventArgs> denyEvent;

        /// <summary>
        /// getInstance method to get an instance of the ViewModel
        /// </summary>
        /// <param name="uiStateModel"> Statemanager </param>
        /// <returns></returns>
        public static CreateSessionViewModel getInstance(UiStateModel uiStateModel)
        {
            if (instance == null)
            {
                instance = new CreateSessionViewModel(uiStateModel);
            }
            return instance;
        }

        /// <summary>
        /// Standart constructor which sets the uiStateModel and listens to PropertyChanged
        /// Initializes the Taskfactory
        /// </summary>
        /// <param name="uiStateModel"></param>
        private CreateSessionViewModel(UiStateModel uiStateModel)
        {
            this.uiStateModel = uiStateModel;
            PropertyChanged += shown;
            taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

        //current session which will be created
        private SessionDTO _curSession;

        /// <summary>
        /// Current Session Property
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
        /// Initializes the CreateSessionModel and adds listener to the propertychanged of the model
        /// Sends a MapListMessage to receive the maps from the server
        /// </summary>
        private void initView()
        {
            _model = new CreateSessionModel();
            _model.AddListener();
            _model.PropertyChanged += _model_PropertyChanged;
            _model.Items.CollectionChanged += CollectionChanged;
            _model.sendMapListMessage();
			GameSessionProperty = true;
			EditorSessionProperty = false;
			NewMapProperty = false;
        }

        /// <summary>
        /// If the collection is changed, the new objects will be addet to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            taskFactory.StartNew(() =>
            {
                foreach (MapMetaDTO map in e.NewItems)
                {
                    if (!Contains(Items, map))
                    {
                        Items.Add(map);
                    }
                }
            }).Wait();
            
        }

		public void AddDenyEvent(EventHandler<ErrorMessageEventArgs> eventHandler)
		{
			denyEvent = eventHandler;
			_model.denyEvent += denyEvent;
		}

		/// <summary>
		/// Contains method to check if the map is already in the ObservableCollection
		/// </summary>
		/// <param name="list"></param>
		/// <param name="map"></param>
		/// <returns></returns>

		//contains method to check if a map is already in the observable list
		private bool Contains(ObservableCollection<MapMetaDTO> list, MapMetaDTO map)
        {
            foreach (MapMetaDTO cur in list)
            {
                if (cur.name.Equals(map.name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// If the startProperty of the model is changed the joinSession method is called
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "startProperty")
            {
                joinSession();
            }
        }

        /// <summary>
        /// Method to join the session which was just created
        /// </summary>
        private void joinSession()
        {
            _model.createSessionChannel();
            _model.RemoveListener();
            if (_model.sessionDTO != null)
            {
                if (_model.sessionDTO.gameSession)
                {
                    GameLobbyViewModel.getInstance(uiStateModel).curSession = _model.sessionDTO;
                    GameLobbyViewModel.getInstance(uiStateModel).Owner = true;
                    uiStateModel.State = "GameLobby";
                }
                if (_model.sessionDTO.editorSession)
                {
                    EditorViewModel.getInstance(uiStateModel).curSession = _model.sessionDTO;
                    uiStateModel.State = "Editor";
                }
            }
        }

        private ObservableCollection<MapMetaDTO> _items = new ObservableCollection<MapMetaDTO>();
        /// <summary>
        /// ObservableCollection which contains the maps
        /// </summary>
        public ObservableCollection<MapMetaDTO> Items { get => _items; set => _items = value; }

        private bool _gameSessionProperty;

        /// <summary>
        /// GameSessionProperty to show if the session is a gamesession
        /// </summary>
        public bool GameSessionProperty
        {
            get => _gameSessionProperty;
            set
            {
                if (_gameSessionProperty != value)
                {
                    _gameSessionProperty = value;
                    _model.sessionDTO.gameSession = value;
                    OnPropertyChanged("GameSessionProperty");
                }
            }
        }

        private bool _editorSessionProperty;

        /// <summary>
        /// EditorSessionProperty to show if the session is a editorsession
        /// </summary>
        public bool EditorSessionProperty
        {
            get => _editorSessionProperty;
            set
            {
                if (_editorSessionProperty != value)
                {
                    _editorSessionProperty = value;
                    _model.sessionDTO.editorSession = value;
                    OnPropertyChanged("EditorSessionProperty");
                }
            }
        }

		private bool _newMapProperty;

        /// <summary>
        /// NewMapProperty
        /// </summary>
        public bool NewMapProperty
        {
            get => _newMapProperty;
            set
            {
                _newMapProperty = value;
                _model.isNewMap = value;
                OnPropertyChanged("NewMapProperty");
            }
        }

        private string _sessionNameProperty = "SessionName";

        /// <summary>
        /// SessionNameProperty which contains the sessionname
        /// </summary>
        public string SessionNameProperty
        {
            get => _sessionNameProperty;
            set
            {
                if (_sessionNameProperty != value)
                {
                    _sessionNameProperty = value;
                    OnPropertyChanged("SessionNameProperty");
                }
            }
        }

        private string _mapNameProperty = "MapName";

        /// <summary>
        /// MapNameProperty
        /// </summary>
        public string MapNameProperty
        {
            get => _mapNameProperty;
            set
            {
                if (_mapNameProperty != value)
                {
                    _mapNameProperty = value;
                    OnPropertyChanged("MapNameProperty");
                }
            }
        }

        private ICommand _backCommand;

        /// <summary>
        /// BackCommand which is responsible to go back to the ServerList
        /// </summary>
        public ICommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new ActionCommand(e => {
						_model.denyEvent -= denyEvent;
						_model.RemoveListener();
						uiStateModel.State = "ServerList";
					});
                }
                return _backCommand;
            }
        }

        private ICommand _createSessionCommand;

        /// <summary>
        /// CreateSessionCommand which calls the CreateSession method
        /// </summary>
        public ICommand CreateSessionCommand
        {
            get
            {
                if (_createSessionCommand == null)
                {
                    _createSessionCommand = new ActionCommand(e => CreateSession());
                }
                return _createSessionCommand;
            }
        }

        private MapMetaDTO _map;

        public MapMetaDTO Map
        {
            get => _map;
            set
            {
                if (_map != value)
                {
                    _map = value;
                    OnPropertyChanged("Map");
                }
            }
        }

        /// <summary>
        /// Creates a new Session and sends the session to the Server
        /// </summary>
        public void CreateSession()
        {
            _model.sessionDTO.name = SessionNameProperty;
            _model.sessionDTO.users = 0;
            _model.sessionDTO.gameSession = GameSessionProperty;
            _model.sessionDTO.editorSession = EditorSessionProperty;
            if (_model.sessionDTO.editorSession && NewMapProperty)
            {
                _model.sessionDTO.mapMetaDTO = new MapMetaDTO();
                _model.sessionDTO.mapMetaDTO.name = MapNameProperty;
            }
            else
            {
                foreach (MapMetaDTO map in _model.Items)
                {
                    MapMetaDTO mapMetaDTO = Map;
                    if (map.name == mapMetaDTO.name)
                    {
                        _model.sessionDTO.mapMetaDTO = map;
                    }
                    else
                    {
                        if (_model.sessionDTO.editorSession)
                        {
                            _model.sessionDTO.mapMetaDTO = Map;
                        }
                    }
                }
            }

            _model.sendCreateSessionMessage();
        }
    }
}