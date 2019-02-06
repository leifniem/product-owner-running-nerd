using LoadRunnerClient.DTOs;
using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LoadRunnerClient
{
    /// <summary>
    /// EditorViewModel
    /// </summary>
	public class EditorViewModel : ObservableViewModelBase
	{
		private UiStateModel uiStateModel;
		private static EditorViewModel instance;
		private EditorModel _model;

        private double _sliderValue;
        private ObservableCollection<MapTile> viewModelMapTiles = new ObservableCollection<MapTile>();
        public ObservableCollection<MapTile> MapTiles { get => viewModelMapTiles; set => viewModelMapTiles = value; }
        private ObservableCollection<MapItem> viewModelMapItems = new ObservableCollection<MapItem>();
        public ObservableCollection<MapItem> MapItems { get => viewModelMapItems; set => viewModelMapItems = value; }
        private ObservableCollection<BlockCursorView> _mapCursors = new ObservableCollection<BlockCursorView>(); 
        public ObservableCollection<BlockCursorView> MapCursors { set => _mapCursors = value;  get => _mapCursors; }
        private Dictionary<BlockCursor, BlockCursorView> cursorMapping = new Dictionary<BlockCursor, BlockCursorView>();

		/// <summary>
		/// Used to Synchronize the Collections from Model and View
		/// </summary>
        private TaskFactory taskFactory;


		/// <summary>
		/// Gets  fired on Collection changed of <paramref name="sender"/>
		/// </summary>
		/// <param name="sender">Collection that calls the CollectionChanged event. In this Case: <see cref="MapTiles"/></param>
		/// <param name="e"><see cref="NotifyCollectionChangedEventArgs"/> that is given by <paramref name="sender"/></param>
		private void MapTiles_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (MapTile item in e.NewItems) { viewModelMapTiles.Add(item); }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (MapTile item in e.OldItems) { viewModelMapTiles.Remove(item); }
						break;
					case NotifyCollectionChangedAction.Reset:
						viewModelMapTiles.Clear();
						break;
					default:
						throw new ArgumentException("Unbehandelter Collection-Change " + e.Action.ToString());
				}
			}).Wait();
		}

        private ObservableCollection<MapSpawnPoint> _enemySpawns = new ObservableCollection<MapSpawnPoint>();
        public ObservableCollection<MapSpawnPoint> EnemySpawns { get => _enemySpawns; set => _enemySpawns = value; }

        private void EnemySpawns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            taskFactory.StartNew(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (MapSpawnPoint item in e.NewItems) { _enemySpawns.Add(item); }
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        foreach (MapSpawnPoint item in e.OldItems) { _enemySpawns.Remove(item); }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        _enemySpawns.Clear();
                        break;
                    default:
                        throw new ArgumentException("Unbehandelter Collection-Change " + e.Action.ToString());
                }
            }).Wait();
        }

		/// <summary>
		/// Gets  fired on Collection changed of <paramref name="sender"/>
		/// </summary>
		/// <param name="sender">Collection that calls the CollectionChanged event. In this Case: <see cref="MapItems"/></param>
		/// <param name="e"><see cref="NotifyCollectionChangedEventArgs"/> that is given by <paramref name="sender"/></param>
		private void MapItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (MapItem item in e.NewItems) { viewModelMapItems.Add(item); }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (MapItem item in e.OldItems) { viewModelMapItems.Remove(item); }
						break;
					case NotifyCollectionChangedAction.Reset:
						viewModelMapItems.Clear();
						break;
					default:
						throw new ArgumentException("Unbehandelter Collection-Change " + e.Action.ToString());
				}
			}).Wait();
		}


		/// <summary>
		/// Gets  fired on Collection changed of <paramref name="sender"/>
		/// </summary>
		/// <param name="sender">Collection that calls the CollectionChanged event. In this Case: <see cref="Cursors"/></param>
		/// <param name="e"><see cref="NotifyCollectionChangedEventArgs"/> that is given by <paramref name="sender"/></param>
		private void CursorsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			taskFactory.StartNew(() =>
			{
				switch (e.Action)
				{
					case NotifyCollectionChangedAction.Add:
						foreach (BlockCursor item in e.NewItems) {
                            var bcv = new BlockCursorView(item, this);
                            cursorMapping.Add(item, bcv);
                            MapCursors.Add(bcv);
                        }
						break;
					case NotifyCollectionChangedAction.Remove:
						foreach (BlockCursor item in e.OldItems) {
                            var bcv = cursorMapping[item];
                            MapCursors.Remove(bcv);
                            cursorMapping.Remove(item);
                        }
						break;
					case NotifyCollectionChangedAction.Reset:
						MapCursors.Clear();
						break;
					default:
						throw new ArgumentException("Unbehandelter Collection-Change " + e.Action.ToString());
				}
			}).Wait();
		}


        public double SliderValue
		{
			get => _sliderValue;
			set
			{
				if (_sliderValue != value)
				{
					_sliderValue = value;
					offset = (int)SliderValue / -60;
					OnPropertyChanged("SliderValue");
				}
			}
		}

        public SessionDTO curSession
		{
			get => _model.curSession;
			set
			{
				if (_model == null)
				{
					_model = new EditorModel(value);
				}
				else
				{
					_model.curSession = value;
				}
			}
		}

		public static EditorViewModel getInstance(UiStateModel uiStateModel)
		{
			if (instance == null)
			{
				instance = new EditorViewModel(uiStateModel);
			}
			return instance;
		}

		/// <summary>
		/// Calls a method in <see cref="_model"/> that the Cursor has been moved
		/// </summary>
		/// <param name="pos">cursor-position in Window</param>
		public void HighlightGrid(Point pos)
		{
			int c = (int)Math.Floor(pos.X / 60);
			int r = (int)Math.Floor(pos.Y / 60);
			_model.CursorMove(c + offset, r);
		}

		/// <summary>
		/// sets the current Tile/Item in <see cref="_model"/>
		/// </summary>
		/// <param name="sender">UIElement, that calls the method</param>
		/// <param name="e">RoutedEventArgs</param>
		public void SetActive(object sender, RoutedEventArgs e)
		{
			var emitter = (System.Windows.Controls.Button)sender;
			string type = emitter.Tag.ToString();
			_model.SetActiveCursor(type);
		}

		private EditorViewModel(UiStateModel uiStateModel)
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

		/// <summary>
		/// Width of the Map for View.
		/// View uses to dynamically change the size of the Grid
		/// </summary>
		public int Width
		{
			get => _model.Width;
			set => _model.Width = value;
		}

		/// <summary>
		/// sets in <see cref="_model"/> if the cursor is dragged
		/// </summary>
		public bool Drag { get => _model.Drag; set => _model.Drag = value; }

		public Map Map
		{
			get => _model.Map;
			set
			{
				_model.Map = value;
				OnPropertyChanged("Map");
			}
		}

        private int _offset;

		/// <summary>
		/// Offset of current user to display the correct Position of <see cref="Cursors"/>
		/// Also used to Place Items/Tiles on right position
		/// </summary>
		public int offset
		{
			get => _offset;
			set
			{
                _offset = value;
                OnPropertyChanged("offset");
			}
		}

		public int PosX { get => _model.PosX; set => _model.PosX = value; }

		public int PosY { get => _model.PosY; set => _model.PosY = value; }

		/// <summary>
		/// initializes the view and alle Elements, the view depends on:
		/// <see cref="OnEditorInit(object, EventArgs)"/>
		/// <see cref="OnEditorInfoReceived(object, EventArgs)"/>
		/// <see cref="OnKicked(object, EventArgs)"/>
		/// <see cref="MapTiles_CollectionChanged(object, NotifyCollectionChangedEventArgs)"/>
		/// <see cref="MapItems_CollectionChanged(object, NotifyCollectionChangedEventArgs)"/>
		/// <see cref="CursorsChanged(object, NotifyCollectionChangedEventArgs)"/>
		/// 
		/// sends UserJoinMessage 
		/// </summary>
		private void initView()
		{
			_model = new EditorModel(curSession);
			_model.Drag = false;
			_model.EditorInit += OnEditorInit;
			_model.EditorInfo += OnEditorInfoReceived;
			_model.EditorKick += OnKicked;
			_model.EditorEnemyDialog += PlacingEnemySpawn;
            _model.Map.spawnPoints.CollectionChanged += EnemySpawns_CollectionChanged;
            _model.Map.solids.CollectionChanged += MapTiles_CollectionChanged;
            _model.Map.items.CollectionChanged += MapItems_CollectionChanged;
			_model.Cursors.CollectionChanged += CursorsChanged;
            _model.SendUserJoinMessage();
        }

		/// <summary>
		/// Shows the EnemySpawnPoint Dialog Window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void PlacingEnemySpawn(object sender, EditorModel.EnemySpawnArgs e) {
			taskFactory.StartNew(() => {
				var enemySpawnWindow = new EnemyScriptDialog(e.name, e.code, e.locked, e.x, e.y);
				enemySpawnWindow.ShowDialog();
			});
		}

		/// <summary>
		/// Handle Right Clicks to make editing AIs possible
		/// </summary>
		/// <param name="pos">Position of Cursor</param>
		public void MouseDownRight(Point pos)
		{
			int c = (int)Math.Floor(pos.X / 60);
			int r = (int)Math.Floor(pos.Y / 60);
			_model.MouseDownRight(c + offset, r);
		}

		/// <summary>
		/// Handle left Clicks
		/// </summary>
		/// <param name="pos">Position of Cursor</param>
		public void MouseDownLeft(Point pos)
		{			
			int c = (int)Math.Floor(pos.X / 60);
			int r = (int)Math.Floor(pos.Y / 60);
			_model.MouseDownLeft(c + offset, r);
		}

		/// <summary>
		/// Stops any dragging movements
		/// </summary>
		public void MouseUp()
		{
			_model.Drag = false;
		}

		/// <summary>
		/// Stops any dragging movements
		/// </summary>
		public void MouseLeave()
		{
			_model.Drag = false;
		}

		/// <summary>
		/// Respond to changes of the minimap slider
		/// </summary>
		/// <param name="value">Position in Minimap</param>
		public void handleSliderChange(double value)
		{
			offset = (int)value / -60;
		}

		public void OnEditorInfoReceived(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Initialization of MiniMap
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnEditorInit(object sender, EventArgs e)
		{
			double currentPosition = SliderValue;
			SliderMinimum = (_model.Map.Sections - 1) * -1920;
			if (currentPosition < SliderMinimum)
				SliderValue = SliderMinimum;
        }

		public void OnKicked(object sender, EventArgs e)
		{
			_model.RemoveListener();
            _model.ClientChannelHandler.sessionChannel.Close();
			uiStateModel.State = "ServerList";
		}


		private ICommand _addSectionCommand;
		public ICommand AddSectionCommand
		{
			get
			{
				if (_addSectionCommand == null)
				{
					_addSectionCommand = new ActionCommand(e => _model.AddSection());
				}
				return _addSectionCommand;
			}
		}


		private ICommand _removeSectionCommand;
		public ICommand RemoveSectionCommand
		{
			get
			{
				if (_removeSectionCommand == null)
				{
					_removeSectionCommand = new ActionCommand(e => _model.RemoveSection());
				}
				return _removeSectionCommand;
			}
		}
        


		private ICommand _quitCommand;
		public ICommand QuitEditorCommand
		{
			get
			{
				if (_quitCommand == null)
				{
					_quitCommand = new ActionCommand(e => _model.QuitEditor());
				}
				return _quitCommand;
			}
		}

		private int _sliderMinimum;

		public int SliderMinimum
		{
			get => _sliderMinimum;
			set
			{
				_sliderMinimum = value;
				OnPropertyChanged("SliderMinimum");
			}
		}

	}
}