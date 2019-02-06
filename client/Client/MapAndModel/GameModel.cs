using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using LoadRunnerClient.Network.Messages;
using LoadRunnerClient.Network.Messages.Game;
using LoadRunnerClient.Util;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows.Input;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
	public class GameModel : ObservableModelBase
	{
		/// private properties needed to handle the games logic
		private ClientChannelHandler _clientChannelHandler;

		private bool _digLeftIsPressed = false;

		private bool _digRightIsPressed = false;

		private bool _downIsPressed = false;

		private bool _energyIsPressed = false;

		private GameController _gameController;

		private bool _gameEnd = false;

		private bool _leftIsPressed = false;

		private Map _map;

		private int _numSection;

		private bool _rightIsPressed = false;

		private bool _spaceIsPressed = false;

		private bool _upIsPressed = false;

		/// <summary>
		/// EventHandler with custom Arguments to distribute the sounds to play to the layer above
		/// </summary>
		public event EventHandler<SoundEventArgs> soundEvent;

		public event EventHandler<EventArgs> GameKick;

		/// <summary>
		/// Factory to retrieve sounds to be played thorughout Game
		/// </summary>
		private GameSoundFactory sounds = new GameSoundFactory();

		/// <summary>
		/// Timer used to check Gamepad input if connected
		/// </summary>
		private Timer controlTimer = new Timer()
		{
			Interval = 50
		};

		/// <summary>
		/// Current scores in the game
		/// </summary>
		public Dictionary<string, int> scores;

		/// <summary>
		/// Handler for input of a Gamepad
		/// </summary>
		private GamePadHandler gamePadHandler;

		/// <summary>
		/// Color name of potential Game winner
		/// </summary>
		private string winnerColor;

		/// <summary>
		/// Win / Loss State of Game
		/// </summary>
		private string endState;

		public GameModel()
		{
		}

		/// <summary>
		/// Constructor for the GameModel
		/// </summary>
		/// <param name="numSection">Section-No. to display by client</param>
		public GameModel(int numSection)
		{
			_numSection = numSection;
			this.ClientChannelHandler = ClientChannelHandler.getInstance();
			AddListener();
			ClientChannelHandler.sendPlayerReadyMessage();
		}


		public ClientChannelHandler ClientChannelHandler
		{
			get => _clientChannelHandler;
			set => _clientChannelHandler = value;
		}
		public bool DigLeftIsPressed { get => _digLeftIsPressed; set => _digLeftIsPressed = value; }

		public bool DigRightIsPressed { get => _digRightIsPressed; set => _digRightIsPressed = value; }

		public bool DownIsPressed { get => _downIsPressed; set => _downIsPressed = value; }

		public bool EnergyIsPressed { get => _energyIsPressed; set => _energyIsPressed = value; }

		public GameController GameController { get => _gameController; set => _gameController = value; }

		public bool GameEnd
		{
			get => _gameEnd;
			set
			{
				if (_gameEnd != value)
				{
					_gameEnd = value;
					OnPropertyChanged("GameEnd");
				}
			}
		}

		public bool LeftIsPressed { get => _leftIsPressed; set => _leftIsPressed = value; }

		public Map Map
		{
			get => _map;
			set
			{
				_map = value;
				InitMap(value);
				OnPropertyChanged("Map");
			}
		}
		public bool RightIsPressed { get => _rightIsPressed; set => _rightIsPressed = value; }
		public bool SpaceIsPressed { get => _spaceIsPressed; set => _spaceIsPressed = value; }
		public bool UpIsPressed { get => _upIsPressed; set => _upIsPressed = value; }
		public string WinnerColor { get => winnerColor; set => winnerColor = value; }
		public string EndState { get => endState; set => endState = value; }

		#region Listener
		/// <summary>
		/// Adds a handler to the message input
		/// </summary>
		public void AddListener()
		{
			ClientChannelHandler.sessionChannel.OnMessageReceived += listener;
			addController();
		}

		/// <summary>
		/// Handles messages related to the Game
		/// </summary>
		/// <param name="type">Message Type</param>
		/// <param name="msg">Message Contents</param>
		public void listener(string type, string msg)
		{
			switch (type)
			{
				case KickMessage.TYPE:
					ParseKickMessage(msg);
					return;

				case TickUpdateMessage.TYPE:
					ParseTickUpdateMessage(msg);
					return;

				case GameInitMessage.TYPE:
					ParseGameInitMessage(msg);
					return;

				case PlayerQuitMessage.TYPE:
					ParsePlayerQuitMessage(msg);
					return;
			}
		}

		/// <summary>
		/// Removed Handler added by GameModel from message input
		/// </summary>
		public void RemoveListener()
		{
			ClientChannelHandler.sessionChannel.OnMessageReceived -= listener;
			_map.playSoundEventHandler -= passCharacterSound;
		}
		#endregion Listener

		#region MessageParsing

        private bool EventInMyView(float PosX)
        {
            return (PosX / 60) >= 0 && (PosX / 60) <= Map.SECTION_WIDTH;
        }
		/// <summary>
		/// Parses and Executes a MoveActorMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseActorMoveMessage(String msg)
		{
			MoveActorMessage mAM;
			mAM = Serializer.Deserialize<MoveActorMessage>(msg);
			Map.UpdateCharacterPosition(mAM.uuid, mAM.newPosition, mAM.state);
		}

		/// <summary>
		/// Parses and Executes a CreditPointsUpdateMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseCreditPointsUpdateMessage(String msg)
		{
			CreditPointsUpdateMessage creditPointsUpdateMessage;
			creditPointsUpdateMessage = Serializer.Deserialize<CreditPointsUpdateMessage>(msg);
			Map.UpdateCharacterCreditPoints(creditPointsUpdateMessage.uuid, creditPointsUpdateMessage.creditPoints);

            /// is player on screen? - if yes trigger eventhandler with sound to be played
           
            if (EventInMyView(Map.players[creditPointsUpdateMessage.uuid].PosX))
            {
                soundEvent(this, new SoundEventArgs()
                {
                    Msg = GameSoundFactory.AvailableSounds.CREDIT_POINTS
                });
            }
		}

		/// <summary>
		/// Parses and Executes a GameInitMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseGameInitMessage(String msg)
		{
			GameInitMessage gameInitMessage = Serializer.Deserialize<GameInitMessage>(msg);
			Map = new Map(gameInitMessage);
		}

		/// <summary>
		/// Parses and Executes a ItemPickedUpMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseItemPickedUpMessage(String msg)
		{
			ItemPickedUpMessage itemPickedUpMessage;
			itemPickedUpMessage = Serializer.Deserialize<ItemPickedUpMessage>(msg);

			Item item;
			Enum.TryParse(itemPickedUpMessage.itemDTO.type, out item);

			Map.RemoveItem(itemPickedUpMessage.itemDTO.gridX, itemPickedUpMessage.itemDTO.gridY);
			Map.players[itemPickedUpMessage.uuid].AddItemToInventory(item);

			//if (itemPickedUpMessage.itemDTO.gridX >= _map.Offset && itemPickedUpMessage.itemDTO.gridX < _map.Offset + Map.SECTION_WIDTH)
			//{
			//	soundEvent(this, new SoundEventArgs(){
			// 	Msg = "ItemPickup"
			// });
			//}
		}

		/// <summary>
		/// Parses and Executes a ItemUsedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseItemUsedMessage(String msg)
		{
			ItemUsedMessage itemUsedMessage;
			itemUsedMessage = Serializer.Deserialize<ItemUsedMessage>(msg);

			Item item;
			Enum.TryParse(itemUsedMessage.type, out item);

			Map.players[itemUsedMessage.uuid].RemoveItemFromInventory(item);

			if (EventInMyView(Map.players[itemUsedMessage.uuid].PosX))
			{
				soundEvent(this, new SoundEventArgs(){
					Msg = GameSoundFactory.AvailableSounds.ENERGY_DRINK
				});
			}
		}

		/// <summary>
		/// Parses and Executes a LifePointsUpdateMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseLifePointsUpdateMessage(String msg)
		{
			LifePointsUpdateMessage lifePointsUpdateMessage;
			lifePointsUpdateMessage = Serializer.Deserialize<LifePointsUpdateMessage>(msg);
			bool lostLife = lifePointsUpdateMessage.lifePoints - Map.players[lifePointsUpdateMessage.uuid].LifePoints < 0;
			Map.UpdateCharacterLifePoints(lifePointsUpdateMessage.uuid, lifePointsUpdateMessage.lifePoints);
			if (EventInMyView(Map.players[lifePointsUpdateMessage.uuid].PosX))
			{
				if (lostLife)
				{
					soundEvent(this, new SoundEventArgs(){
						Msg = GameSoundFactory.AvailableSounds.LIFE_LOST
					});
				}
				else
				{
					soundEvent(this, new SoundEventArgs() {
						Msg = GameSoundFactory.AvailableSounds.LIFE_GAINED
					});
				}
			}
		}

		/// <summary>
		/// Parses a TickUpdateMessage and disassembles it into the included Types of Messages which then get passed to Parsing individually
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseTickUpdateMessage(String msg)
		{
			TickUpdateMessage tickUpdateMessage = Serializer.Deserialize<TickUpdateMessage>(msg);

			List<Object> moveActorMessages;
			tickUpdateMessage.changes.TryGetValue("MoveActorMessages", out moveActorMessages);

			moveActorMessages.ForEach(moveActorMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)moveActorMessage;
				ParseActorMoveMessage(messageAsObject.ToString());
			});

			List<Object> lifePointsUpdateMessages;
			tickUpdateMessage.changes.TryGetValue("LifePointsUpdateMessages", out lifePointsUpdateMessages);

			lifePointsUpdateMessages.ForEach(lifePointsUpdateMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)lifePointsUpdateMessage;
				ParseLifePointsUpdateMessage(messageAsObject.ToString());
			});

			List<Object> creditPointsUpdateMessages;
			tickUpdateMessage.changes.TryGetValue("CreditPointsUpdateMessages", out creditPointsUpdateMessages);

			creditPointsUpdateMessages.ForEach(creditPointsUpdateMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)creditPointsUpdateMessage;
				ParseCreditPointsUpdateMessage(messageAsObject.ToString());
			});

			List<Object> tileChangedMessages;
			tickUpdateMessage.changes.TryGetValue("TileChangedMessages", out tileChangedMessages);

			tileChangedMessages.ForEach(tileChangedMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)tileChangedMessage;
				ParseTileChangedMessage(messageAsObject.ToString());
			});

			List<Object> itemPickedUpMessages;
			tickUpdateMessage.changes.TryGetValue("ItemPickedUpMessages", out itemPickedUpMessages);

			itemPickedUpMessages.ForEach(itemPickedUpMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)itemPickedUpMessage;
				ParseItemPickedUpMessage(messageAsObject.ToString());
			});

			List<Object> itemUsedMessages;
			tickUpdateMessage.changes.TryGetValue("ItemUsedMessages", out itemUsedMessages);

			itemUsedMessages.ForEach(itemUsedMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)itemUsedMessage;
				ParseItemUsedMessage(messageAsObject.ToString());
			});

			List<Object> GameEndMessages;
			tickUpdateMessage.changes.TryGetValue("GameEndMessages", out GameEndMessages);
			GameEndMessages.ForEach(gameEndMessage =>
			{
				Newtonsoft.Json.Linq.JObject messageAsObject = (Newtonsoft.Json.Linq.JObject)gameEndMessage;
				ParseGameEndMessage(messageAsObject.ToString());
			});
		}

		/// <summary>
		/// Parses and Executes a TileCHangedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseTileChangedMessage(String msg)
		{
			TileChangedMessage tileChangedMessage;
			tileChangedMessage = Serializer.Deserialize<TileChangedMessage>(msg);

			Tile newMapTile = Tile.DESTROYED_SOLID;

			if (tileChangedMessage.state.Equals(TileChangedMessage.RESTORED_STATE))
			{
				newMapTile = Tile.DESTROYABLE_SOLID;
			}
			else if (tileChangedMessage.state.Equals(TileChangedMessage.DESTROYED_STATE))
			{
                if (EventInMyView(tileChangedMessage.gridX))
				{
					soundEvent(this, new SoundEventArgs(){
						Msg = GameSoundFactory.AvailableSounds.DIGGING
					});
				}
			}
			else
			{
				/// TODO: abschmieren
			}
			Map.UpdateTileStatus(newMapTile, tileChangedMessage.gridX, tileChangedMessage.gridY);
		}

		/// <summary>
		/// Parses and Executes a GameEndMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseGameEndMessage(string msg)
		{
			GameEndMessage gameEndMessage = Serializer.Deserialize<GameEndMessage>(msg);
			scores = gameEndMessage.Scores;
			WinnerColor = gameEndMessage.WinnerColor;
			endState = gameEndMessage.Status;

			GameEnd = true;
		}

		/// <summary>
		/// Parses and Executes a PlayerQuitMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParsePlayerQuitMessage(string msg)
		{
			PlayerQuitMessage playerQuitMessage = Serializer.Deserialize<PlayerQuitMessage>(msg);
			Map.RemoveCharacter(playerQuitMessage.uuid);
		}
		
		private void ParseKickMessage(string msg) {
			if (!_gameEnd) {
				RemoveListener();
				ClientChannelHandler.sessionChannel.Close();
				if (GameKick != null)
					GameKick(this, new EventArgs());
			}
		}

		#endregion MessageParsing

		/// <summary>
		/// Calculates required offset for Map-Model if needed
		/// </summary>
		/// <param name="map">Map Instance</param>
		private void InitMap(Map map)
		{
			if (_numSection >= 0)
			{
				_map.Offset = _numSection;
			}
			_map.playSoundEventHandler += passCharacterSound;
		}

		public void passCharacterSound(object sender, SoundEventArgs e){
			GameCharacter eventCharacter = sender as GameCharacter;
            if (!EventInMyView(eventCharacter.PosX))
                return;

			soundEvent(sender, new SoundEventArgs(){Msg = e.Msg});
		}

		#region InputHandling
		/// <summary>
		/// Removes Gamepad and its Timer
		/// </summary>
		public void GameWindow_Closing()
		{
			ClientChannelHandler.sessionChannel.Close();
			GameController = null;
			controlTimer.Stop();
			controlTimer.Dispose();
		}

		/// <summary>
		/// Register Keypresses on Window
		/// </summary>
		/// <param name="eventArgs">EventArguments containing pressed Keys</param>
		public void OnKeyDownHandler(KeyEventArgs eventArgs)
		{
			if (eventArgs.IsDown)
			{
				if (eventArgs.Key == Key.A && !LeftIsPressed)
				{
					LeftIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.LEFT);
					return;
				}
				else if (eventArgs.Key == Key.D && !RightIsPressed)
				{
					RightIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.RIGHT);
					return;
				}
				else if (eventArgs.Key == Key.W && !UpIsPressed)
				{
					UpIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.UP);
					return;
				}
				else if (eventArgs.Key == Key.S && !DownIsPressed)
				{
					DownIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DOWN);
					return;
				}
				else if (eventArgs.Key == Key.Space && !SpaceIsPressed)
				{
					SpaceIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.JUMP);
				}
				else if (eventArgs.Key == Key.D1 && !EnergyIsPressed)
				{
					EnergyIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.ENERGY);
				}
				else if (eventArgs.Key == Key.Q && !DigLeftIsPressed)
				{
					DigLeftIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DIG_LEFT);
				}
				else if (eventArgs.Key == Key.E && !DigRightIsPressed)
				{
					DigRightIsPressed = true;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DIG_RIGHT);
				}
			}
		}

		/// <summary>
		/// Stop Player Input on Server when button is released
		/// </summary>
		/// <param name="eventArgs">EventArgs containing eleased keys</param>
		public void OnKeyUpHandler(KeyEventArgs eventArgs)
		{
			if (eventArgs.IsUp)
			{
				if (eventArgs.Key == Key.A)
				{
					LeftIsPressed = false;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.LEFT);
					return;
				}
				else if (eventArgs.Key == Key.D)
				{
					RightIsPressed = false;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.RIGHT);
					return;
				}
				else if (eventArgs.Key == Key.W)
				{
					UpIsPressed = false;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.UP);
					return;
				}
				else if (eventArgs.Key == Key.S)
				{
					DownIsPressed = false;
					ClientChannelHandler.sendPlayerCommandMessage(PressedKey.DOWN);
					return;
				}
				else if (eventArgs.Key == Key.Space)
				{
					SpaceIsPressed = false;
				}
				else if (eventArgs.Key == Key.D1)
				{
					EnergyIsPressed = false;
				}
				else if (eventArgs.Key == Key.Q)
				{
					DigLeftIsPressed = false;
				}
				else if (eventArgs.Key == Key.E)
				{
					DigRightIsPressed = false;
				}
			}
		}

		/// <summary>
		/// Checks if there is a Gamepad to add as input and registers appropriate handling
		/// </summary>
		private void addController()
		{
			try
			{
				GameController = new GameController();
				gamePadHandler = new GamePadHandler(this);
				GameController.PropertyChanged += gamePadHandler.handleControllerInput;
				controlTimer.Elapsed += GameController.checkInput;
				controlTimer.Start();
			}
			catch (Exception exc)
			{
				Console.Error.Write(exc.Message);
			}
		}
		#endregion InputHandling
	}


	/// <summary>
	/// EventArgs to incorporate AvailableSounds Enum to ensure only available sounds get played
	/// </summary>
	public class SoundEventArgs : EventArgs
	{
		private GameSoundFactory.AvailableSounds msg;
		private String uuid;

		public SoundEventArgs()
		{

		}

		public GameSoundFactory.AvailableSounds Msg { get => msg; set => msg = value; }
		public string Uuid { get => uuid; set => uuid = value; }
	}
}