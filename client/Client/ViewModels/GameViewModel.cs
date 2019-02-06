using LoadRunnerClient.MapAndModel;
using LoadRunnerClient.Util;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace LoadRunnerClient
{
	/// <summary>
	/// GameViewModel
	/// </summary>
	public class GameViewModel : ObservableViewModelBase
	{
		private UiStateModel uiStateModel;
		private static GameViewModel instance;
		public GameModel _model;
		private TaskFactory taskFactory;
		private ObservableCollection<MapTile> _relevantSolids = new ObservableCollection<MapTile>();

		/// <summary>
		/// Collecton of RelevantSolids
		/// </summary>
		public ObservableCollection<MapTile> RelevantSolids
		{
			get => _relevantSolids;
			set
			{
				_relevantSolids = value;
				OnPropertyChanged("RelevantSolids");
			}
		}

		private ObservableCollection<MapItem> _relevantItems = new ObservableCollection<MapItem>();

		/// <summary>
		/// Collection of RelevantSolids
		/// </summary>
		public ObservableCollection<MapItem> RelevantItems
		{
			get => _relevantItems;
			set
			{
				_relevantItems = value;
				OnPropertyChanged("RelevantItems");
			}
		}

		private ObservableCollection<GameCharacter> _players = new ObservableCollection<GameCharacter>();

		/// <summary>
		/// Collection of Players
		/// </summary>
		public ObservableCollection<GameCharacter> Players
		{
			get => _players;
			set
			{
				_players = value;
				OnPropertyChanged("Players");
			}
		}

		/// <summary>
		/// GetInstance Method
		/// </summary>
		/// <param name="uiStateModel"></param>
		/// <returns>An instance of the GameViewModel</returns>
		public static GameViewModel getInstance(UiStateModel uiStateModel)
		{
			if (instance == null)
			{
				instance = new GameViewModel(uiStateModel);
			}
			return instance;
		}

		/// <summary>
		/// Standart constructor
		/// </summary>
		/// <param name="uiStateModel"></param>
		private GameViewModel(UiStateModel uiStateModel)
		{
			this.uiStateModel = uiStateModel;
			PropertyChanged += shown;
			taskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());
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

		private int _section;
		public int Section { get => _section; set => _section = value; }

		/// <summary>
		/// InitView method which inits the GameModel
		/// </summary>
		private void initView()
		{
			this._model = new GameModel(Section);
			this._model.PropertyChanged += MapChanged;
			this._model.soundEvent += playEventSounds;
			this._model.GameKick += OnKick;
		}


		private void OnKick(object sender, EventArgs e) {
			uiStateModel.State = "ServerList";
		}

		/// <summary>
		/// CollectionChanged listener
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void PlayersChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (GameCharacter item in e.NewItems) { Players.Add(item); }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (GameCharacter item in e.OldItems) { Players.Remove(item); }
						break;
					case NotifyCollectionChangedAction.Reset:
						Players.Clear();
						break;
					default:
						throw new ArgumentException("Unexpected Items-Change " + e.Action.ToString());
				}
			}).Wait();
		}

		/// <summary>
		/// Mapchanged method whenever the map is changed in the model -> the relevant items solids and players will be updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void MapChanged(object sender, PropertyChangedEventArgs e)
		{

            if (e.PropertyName == "Map")
            {
                if (Section >= 0)
                {
                    taskFactory.StartNew(() =>
                    {
						Players.Clear();
                        RelevantItems.Clear();
                        RelevantSolids.Clear();
                        foreach (GameCharacter player in _model.Map.players.Values)
                        {
                            Players.Add(player);
                        }
                        foreach (MapItem item in _model.Map.RelevantItems)
                        {
                            RelevantItems.Add(item);
                        }
                        foreach (MapTile item in _model.Map.RelevantSolids)
                        {
                            RelevantSolids.Add(item);
                        }
                        _model.Map.RelevantSolids.CollectionChanged += SolidsChanged;
                        _model.Map.RelevantItems.CollectionChanged += ItemsChanged;
                        _model.Map.players.CollectionChanged += PlayersChanged;
                    }).Wait();
                }
            }else if(e.PropertyName == "GameEnd")
            {
                GameEndViewModel.getInstance(uiStateModel).State = _model.EndState;
                GameEndViewModel.getInstance(uiStateModel).Scores = _model.scores;
                GameEndViewModel.getInstance(uiStateModel).WinnerColor = _model.WinnerColor;
				_model.RemoveListener();
                uiStateModel.State = "GameEnd";
            }
        }

		/// <summary>
		/// CollectionChanged method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (MapItem item in e.NewItems) { RelevantItems.Add(item); }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (MapItem item in e.OldItems) { RelevantItems.Remove(item); }
						break;
					case NotifyCollectionChangedAction.Reset:
						RelevantItems.Clear();
						break;
					default:
						throw new ArgumentException("Unexpected Items-Change " + e.Action.ToString());
				}
			}).Wait();
		}

		/// <summary>
		/// CollectionChanged method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SolidsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (MapTile item in e.NewItems) { RelevantSolids.Add(item); }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (MapTile item in e.OldItems) { RelevantSolids.Remove(item); }
						break;
					case NotifyCollectionChangedAction.Reset:
						RelevantSolids.Clear();
						break;
					default:
						throw new ArgumentException("Unexpected Solid-Change " + e.Action.ToString());
				}
			}).Wait();
		}

		private ICommand _unloadedCommand;

		/// <summary>
		/// UnloadedCommand which is responsible to close everything relevant
		/// </summary>
		public ICommand UnloadedCommand
		{
			get
			{
				if (_unloadedCommand == null)
				{
					_unloadedCommand = new ActionCommand(e => UnLoaded());
				}
				return _unloadedCommand;
			}
		}

		/// <summary>
		/// Unloaded method which calls the GameWindow_Closing method of the model switches state to ServerList
		/// </summary>
		public void UnLoaded()
		{
			_model.GameWindow_Closing();
			uiStateModel.State = "ServerList";
		}

		/// <summary>
		/// HandleKeyDown method which gives the input to the model
		/// if the EscapeKey is pressed the unload method is called 
		/// </summary>
		/// <param name="e"> Key which is pressed </param>
		public void HandleKeyDown(KeyEventArgs e)
		{
			if (e.Key != Key.Escape)
			{
				_model.OnKeyDownHandler(e);
			}
			//else
			//{
			//    UnLoaded();
			//}
		}

		/// <summary>
		/// HandleKeyUp method which gives the input to the model
		/// </summary>
		/// <param name="e"> Key which is pressed </param>
		public void HandleKeyUp(KeyEventArgs e)
		{
			_model.OnKeyUpHandler(e);
		}

		#region Sounds 
		/// <summary>
		/// Factory to retrieve sounds to be played thorughout Game and play asynchronously
		/// </summary>
		private GameSoundFactory sounds = new GameSoundFactory();
		TaskFactory playTaskFactory = new TaskFactory(TaskScheduler.FromCurrentSynchronizationContext());

		public void playEventSounds(object sender, SoundEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				sounds.getSound(e.Msg).Play();
			});
		}
		#endregion
	}
}