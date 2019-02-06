using LoadRunnerClient.DTOs;
using LoadRunnerClient.MapAndModel;
using LoadRunnerClient.Util;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LoadRunnerClient
{
    public class ServerListViewModel : ObservableViewModelBase
    {
        /// <summary>
        /// ServerListModel which contains logic
        /// </summary>
        private ServerListModel _model;

        /// <summary>
        /// Statemanager
        /// </summary>
        private UiStateModel uiStateModel;

        //Game & Editorsessions
        private ObservableCollection<SessionDTO> _gamesessions = new ObservableCollection<SessionDTO>();

        /// <summary>
        /// ObservableCollection which contains every GameSession available
        /// </summary>
        public ObservableCollection<SessionDTO> gamesession { get => _gamesessions; set => _gamesessions = value; }

        private ObservableCollection<SessionDTO> _editorsessions = new ObservableCollection<SessionDTO>();

        /// <summary>
        /// ObservableCollection which contains every EditorSession available
        /// </summary>
        public ObservableCollection<SessionDTO> editorsession { get => _editorsessions; set => _editorsessions = value; }

		/// <summary>
		/// Factory needed to perform Actions on ViewModel Collections
		/// </summary>
        private TaskFactory taskFactory;

		/// <summary>
		/// 
		/// </summary>
		private bool _showAbout;
               
        private static ServerListViewModel instance;

        /// <summary>
        /// GetInstance Method of the ViewModel
        /// </summary>
        /// <param name="uiStateModel"> Statemanager </param>
        /// <returns>Instance of a ServerListModel</returns>
        public static ServerListViewModel getInstance(UiStateModel uiStateModel)
        {
            if (instance == null)
            {
                instance = new ServerListViewModel(uiStateModel);
            }
            return instance;
        }

        /// <summary>
        /// Standart Constructor,
        /// listener to Model propertys and observable collections will be addet here
        /// </summary>
        /// <param name="uiStateModel"> Statemanager </param>
        private ServerListViewModel(UiStateModel uiStateModel)
        {
            this.uiStateModel = uiStateModel;
            _model = new ServerListModel();           
            //Add listener to model
            _model.PropertyChanged += _model_PropertyChanged;
            //Add collectionchangedlistener to sessionlists to get new items of the list
            _model.Gamesession.CollectionChanged += changed;
            _model.Editorsession.CollectionChanged += changed;
            //listener for own propertychanged to get notified when the VM is shown
            this.PropertyChanged += shown;
            taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// Listener method to get updates from the collections in model
        /// adds the new items to the collection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void changed(object sender, NotifyCollectionChangedEventArgs e)
        {
            taskFactory.StartNew(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (SessionDTO session in e.NewItems)
                        {
                            if (session.editorSession)
                            {
                                editorsession.Add(session);
                            }
                            else
                            {
                                gamesession.Add(session);
                            }
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        editorsession.Clear();
                        gamesession.Clear();
                        break;
                }
            }).Wait();

                                  
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
        /// InitView method
        /// adds the listener in the model to listen to incoming messages and sends a getsessionlistmessage
        /// </summary>      
        private void initView()
        {
            _model.AddListener();
            this._model.ClientChannelHandler.sendGetSessionListMessage();
        }

        /// <summary>
        /// listener to get notified when acceptjoinmessage is received in the model
        /// if joinProperty is set to true the VM gets notified and will changed the State to editor or gamelobby
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "joinProperty")
            {
                this._model.RemoveListener();
				this._model.denyEvent -= denyEvent;
                if (selectedSession.editorSession)
                {
                    EditorViewModel.getInstance(uiStateModel).curSession = _model.chosenSession;
                    uiStateModel.State = "Editor";
                }
                else if (selectedSession.gameSession)
                {
                    GameLobbyViewModel.getInstance(uiStateModel).curSession = _model.chosenSession;
                    GameLobbyViewModel.getInstance(uiStateModel).Owner = false;
                    uiStateModel.State = "GameLobby";
                }
            }
        }

        
        private SessionDTO _selectedSession;

        /// <summary>
        /// Current selectedSession
        /// </summary>
        public SessionDTO selectedSession
        {
            get => _selectedSession;
            set
            {
                if (_selectedSession != value)
                {
                    _selectedSession = value;
                    OnPropertyChanged("selectedSession");
                }
            }
        }

		public void AddDenyEvent(EventHandler<ErrorMessageEventArgs> eventHandler)
		{
			this.denyEvent = eventHandler;
			_model.denyEvent += denyEvent;
		}

        //ICommand to connect to the server , when there is no selected session the button is not clickable
        private ICommand _ConnectToServerCommand;

        /// <summary>
        /// ConnectToServerCommand
        /// </summary>
        public ICommand ConnectToServerCommand
        {
            get
            {
                if (_ConnectToServerCommand == null)
                {
                    _ConnectToServerCommand = new ActionCommand(dummy => this.ConnectToServer(), dummy => selectedSession != null);
                }
                return _ConnectToServerCommand;
            }
        }

        /// <summary>
        /// on button click method, connectes to the selected session, opens game/editor window ( later lobby )
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="args"></param>
		private void ConnectToServer()
        {
            _model.joinSession(selectedSession);
        }

        //ICommand to refresh the serverlist
        private ICommand _refreshServerListCommand;

        /// <summary>
        /// Refreshs the sessionlists on click
        /// </summary>
        public ICommand RefreshServerListCommand
        {
            get
            {
                if (_refreshServerListCommand == null)
                {
                    _refreshServerListCommand = new ActionCommand(dummy => this.RefreshServerList());
                }
                return _refreshServerListCommand;
            }
        }

        /// <summary>
        /// refresh the sessionlists with a getsessionlistmessage
        /// </summary>
        private void RefreshServerList()
        {
            _model.sendGetSessionListMessage();
        }

        //ICommand to create a session
        private ICommand _createSessionCommand;
		private EventHandler<ErrorMessageEventArgs> denyEvent;

        /// <summary>
        /// CreateSessionCommand to switch to the create session view
        /// </summary>
        public ICommand CreateSessionCommand
        {
            get
            {
                if (_createSessionCommand == null)
                {
                    _createSessionCommand = new ActionCommand(dummy => this.CreateSession());
                }
                return _createSessionCommand;
            }
        }

		public bool ShowAbout {
			get => _showAbout;
			set {
				_showAbout = value;
				OnPropertyChanged("ShowAbout");
			}
		}

		/// <summary>
		/// Switches to CreateSession VM and removes the listener from the model
		/// </summary>
		private void CreateSession()
        {
            _model.RemoveListener();
			_model.denyEvent -= denyEvent;
            uiStateModel.State = "CreateSession";
        }
    }
}