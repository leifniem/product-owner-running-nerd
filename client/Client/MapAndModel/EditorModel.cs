using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using LoadRunnerClient.Network.Messages;
using LoadRunnerClient.Util;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
	/// <summary>
	/// Model for handling an EditorSession
	/// </summary>
	public class EditorModel : ObservableModelBase
	{
		/// <summary>
		/// Instance of the ClientChannelHandler used for messaging
		/// </summary>
		private ClientChannelHandler _clientChannelHandler;

		/// <summary>
		/// DTO of the ongoing Session
		/// </summary>
		private SessionDTO _curSession;

		/// <summary>
		/// Dictionary of all currently active Cursors in the Session
		/// </summary>
		private ObservableDictionary<string, BlockCursor> _cursors = new ObservableDictionary<string, BlockCursor>();

		/// <summary>
		/// Instance of the Sessions Map
		/// </summary>
		private Map _map;

		/// <summary>
		/// Current offset to the origin of the Map needed for calculating positions
		/// </summary>
		//private int _offset;


		/// <summary>
		/// Own cursors X-Position to calculate need for Move / new Tile Placement
		/// </summary>
		private int _posX = 0;
		/// <summary>
		/// Own cursors Y-Position to calculate need for Move / new Tile Placement
		/// </summary>
		private int _posY = 0;

		/// <summary>
		/// Preview Texture of the selected Item or Tile
		/// </summary>
		private Brush _texture;

		/// <summary>
		/// Uuid of own Cursors
		/// </summary>
		private string _uuid;

		/// <summary>
		/// Width of the Map in Pixels
		/// </summary>
		private int _width = 1920;

		/// <summary>
		/// Dictionary needed to translate the current Selection into an Item
		/// </summary>
		private Dictionary<BlockCursorType, Item> BlockCursorToItem = new Dictionary<BlockCursorType, Item>()
		{
			{ BlockCursorType.ITEM_CREDITPOINTS_5, Item.CREDITPOINTS_5 },
			{ BlockCursorType.ITEM_CREDITPOINTS_10, Item.CREDITPOINTS_10 },
			{ BlockCursorType.ITEM_CREDITPOINTS_15, Item.CREDITPOINTS_15 },
			{ BlockCursorType.ITEM_ENERGYDRINK, Item.ENERGYDRINK },
			{ BlockCursorType.ITEM_PIZZA, Item.PIZZA }
		};

		/// <summary>
		/// Dictionary needed to translate the current Selection into a Tile
		/// </summary>
		private Dictionary<BlockCursorType, Tile> BlockCursorToTile = new Dictionary<BlockCursorType, Tile>()
		{
			{ BlockCursorType.TILE_DESTROYABLESOLID, Tile.DESTROYABLE_SOLID },
			{ BlockCursorType.TILE_SOLID, Tile.SOLID },
			{ BlockCursorType.TILE_LADDER, Tile.LADDER }
		};

		/// <summary>
		/// ClearBrush used to display the selected "Eraser"
		/// </summary>
		private Brush cleanBrush = Application.Current.Resources["ClearBrush"] as Brush;

		/// <summary>
		/// boolean determining if a click & drag movement is ongoing
		/// </summary>
		private bool drag = false;

		/// <summary>
		/// Initial position of a click & drag movement
		/// </summary>
		private Vector2 dragStart = new Vector2();

		/// <summary>
		/// Type of selected Object to Place
		/// </summary>
		private BlockCursorType selectedBlock = BlockCursorType.TILE_EMPTY;

		/// <summary>
		/// Texture Factory providing Textures of placed objects
		/// </summary>
		private TextureFactory texFac = new TextureFactory();

		/// <summary>
		/// Constructor for the EditorModel
		/// </summary>
		/// <param name="sess">Session to load</param>
		public EditorModel(SessionDTO sess)
		{
			_clientChannelHandler = ClientChannelHandler.getInstance();
			_map = new Map();
			curSession = sess;
			AddListener();
			Width = 1920;
			_texture = cleanBrush;
		}

		/// <summary>
		/// EventHandler enqueued when receiving Info
		/// </summary>
		public event EventHandler<EditorInfoArgs> EditorInfo;
		/// <summary>
		/// EventHandler enqueued when a Session is initiated in the Editor
		/// </summary>
		public event EventHandler<EventArgs> EditorInit;
		/// <summary>
		/// EventHandler enqueued when getting kicked out of a Session
		/// </summary>
		public event EventHandler<EventArgs> EditorKick;

		public event EventHandler<EnemySpawnArgs> EditorEnemyDialog;

		/// <summary>
		/// Possible Types of objects to select for placement
		/// </summary>
		private enum BlockCursorType
		{
			TILE_EMPTY,
			TILE_SOLID,
			TILE_DESTROYABLESOLID,
			TILE_LADDER,
			TILE_PIPE,
			ITEM_CREDITPOINTS_5,
			ITEM_CREDITPOINTS_10,
			ITEM_CREDITPOINTS_15,
			ITEM_ENERGYDRINK,
			ITEM_PIZZA,
			SPAWN_ENEMY,
			SPAWN_USER
		}

		public ClientChannelHandler ClientChannelHandler
		{
			get => _clientChannelHandler;
			set => _clientChannelHandler = value;
		}

		public SessionDTO curSession
		{
			get => _curSession;
			set => _curSession = value;
		}

		public ObservableDictionary<string, BlockCursor> Cursors { get => _cursors; set => _cursors = value; }

		public bool Drag { get => drag; set => drag = value; }

		public MapObservableCollection<MapItem> items
		{
			get => _map.items;
			set => _map.items = value;
		}

		public Map Map
		{
			get => _map;
			set
			{
				_map = value;
				OnPropertyChanged("Map");
			}
		}

		//public int offset
		//{
		//    get => _offset;
		//    set
		//    {
		//        _offset = value;
		//        OnPropertyChanged("offset");
		//    }
		//}

		public int PosX { get => _posX; set => _posX = value; }

		public int PosY { get => _posY; set => _posY = value; }



		public MapObservableCollection<MapTile> solids
		{
			get => _map.solids;
			set => _map.solids = value;
		}

		public int Width
		{
			get => _width;
			set
			{
				if (_width != value)
				{
					_width = value;
				}
			}
		}

		/// <summary>
		/// Method for adding an Item to the Map
		/// </summary>
		/// <param name="item">Type of Item</param>
		/// <param name="c">X-Position on Map</param>
		/// <param name="r">Y-Position on Map</param>
		public void addItemToMap(Item item, int c, int r)
		{
			MapItem newItem = new MapItem(item, c, r);
			ClientChannelHandler.sendItemPlacedMessage(newItem.toItemDTO());
		}

		/// <summary>
		/// Method for adding a Listener to the message input, including its handler
		/// </summary>
		public void AddListener()
		{
			ClientChannelHandler.sessionChannel.OnMessageReceived += handleMessage;
		}

		/// <summary>
		/// Method for sending the request of adding an additional section to the Map
		/// </summary>
		public void AddSection()
		{
			ClientChannelHandler.sendAddSectionMessage();
		}

		/// <summary>
		/// Method for adding a Tile to the Map
		/// </summary>
		/// <param name="tile">Type of Tile</param>
		/// <param name="c">X-Position on Map</param>
		/// <param name="r">Y-Position on Map</param>
		public void addTileToMap(Tile tile, int c, int r)
		{
			MapTile mapTile = new MapTile(tile);
			mapTile.posX = c;
			mapTile.posY = r;
			ClientChannelHandler.sendTilePlacedMessage(mapTile.totileDTO());
		}

		/// <summary>
		/// Method for handling cursor movements
		/// </summary>
		/// <param name="c">Column cursor is in</param>
		/// <param name="r">Row Cursor is in</param>
		public void CursorMove(int c, int r)
		{
			if (c >= 0 && r >= 0)
			{
				if (PosX != c || PosY != r)
				{
					PosX = c;
					PosY = r;

					sendCursorMoveMessage(c, r);

					if (drag)
					{
						addElementToMap(c, r);
					}
				}
			}
		}

		/// <summary>
		/// MessageHandler for the EditorModel
		/// </summary>
		/// <param name="type">Type of message</param>
		/// <param name="msg">Message contents</param>
		public void handleMessage(string type, string msg)
		{
			switch (type)
			{
				case EditorInitMessage.TYPE:
					ParseEditorInitMessage(msg);
					return;

				case TilePlacedMessage.TYPE:
					ParseTilePlacedMessage(msg);
					return;

				case TileRemovedMessage.TYPE:
					ParseTileRemovedMessage(msg);
					return;

				case ItemPlacedMessage.TYPE:
					ParseItemPlacedMessage(msg);
					return;

				case ItemRemovedMessage.TYPE:
					ParseItemRemovedMessage(msg);
					return;

				case EditorSessionJoinedMessage.TYPE:
					ParseUserJoinedSessionMessage(msg);
					return;

				case CursorMovedMessage.TYPE:
					ParseCursorMovedMessage(msg);
					return;

				case CursorIdMessage.TYPE:
					ParseCursorIdMessage(msg);
					return;

				case PlayerQuitMessage.TYPE:
					ParsePlayerQuitMessage(msg);
					return;

				case KickMessage.TYPE:
					ParseKickMessage(msg);
					return;

				case EnemySpawnPointPlacedMessage.TYPE:
					ParseEnemySpawnPointPlacedMessage(msg);
					return;
				case EnemySpawnPointRemovedMessage.TYPE:
					ParseEnemySpawnPointRemovedMessage(msg);
					return;
				case EnemySpawnPointLockAcceptMessage.TYPE:
					ParseEnemySpawnPointLockAcceptMessage(msg);
					return;

				case SpawnPointReplaceMessage.TYPE:
					ParseSpawnPointReplaceMessage(msg);
					return;
				case AddSectionMessage.TYPE:
					ParseAddSection(msg);
					return;

				case RemoveSectionMessage.TYPE:
					ParseRemoveSection(msg);
					return;
			}
		}

		/// <summary>
		/// Place initial Tile and start potential Drag-Placement
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void MouseDownLeft(int c, int r)
		{
			if (selectedBlock != BlockCursorType.SPAWN_ENEMY)
			{
				Drag = true;
				dragStart.X = c;
				dragStart.Y = r;
			}
			addElementToMap(c, r);
		}

		public void MouseDownRight(int c, int r)
		{
			ClientChannelHandler.SendEnemySpawnPointRequestLockMessage(c, r);
		}

		public void OnAttach()
		{
		}

		/// <summary>
		/// Sends Request for closing the Editor for local User
		/// </summary>
		public void QuitEditor()
		{
			ClientChannelHandler.sendQuitEditorMessage();
		}

		/// <summary>
		/// Sends request to remove an Item from the Map
		/// </summary>
		/// <param name="c">X-Position of Item to remove</param>
		/// <param name="r">Y-Position of Item to remove</param>
		public void removeItem(int c, int r)
		{
			ClientChannelHandler.sendItemRemovedMessage(c, r);
		}

		/// <summary>
		/// Removes the MessageHandler from the Input Channel
		/// </summary>
		public void RemoveListener()
		{
			ClientChannelHandler.sessionChannel.OnMessageReceived -= handleMessage;
		}

		/// <summary>
		/// Requests removal of rightmost section
		/// </summary>
		public void RemoveSection()
		{
			ClientChannelHandler.sendRemoveSectionMessage();
		}

		/// <summary>
		/// Sends request to remove a Tile from the Map
		/// </summary>
		/// <param name="c">X-Position of Tile to remove</param>
		/// <param name="r">Y-Position of Tile to remove</param>
		public void removeTile(int c, int r)
		{
			ClientChannelHandler.sendTileRemovedMessage(c, r);
		}

		/// <summary>
		/// Sends updated position of Cursor to the Server
		/// </summary>
		/// <param name="c"></param>
		/// <param name="r"></param>
		public void sendCursorMoveMessage(int c, int r)
		{
			this.ClientChannelHandler.sendCursorMoveMessage(c, r);
		}

		/// <summary>
		/// Requests Access to EditorSession
		/// </summary>
		public void SendUserJoinMessage()
		{
			this.ClientChannelHandler.sendSessionJoinMessage(curSession, SessionRole.SESSION_GUEST);
		}

		/// <summary>
		/// Changes selected Object Type
		/// </summary>
		/// <param name="type">Type of selected Element</param>
		public void SetActiveCursor(string type)
		{
			switch (type)
			{
				case "EMPTY":
					_texture = cleanBrush;
					selectedBlock = BlockCursorType.TILE_EMPTY;
					break;

				case "BRICK":
					_texture = texFac.GetTexture(Tile.DESTROYABLE_SOLID);
					selectedBlock = BlockCursorType.TILE_DESTROYABLESOLID;
					break;

				case "SOLID":
					_texture = texFac.GetTexture(Tile.SOLID);
					selectedBlock = BlockCursorType.TILE_SOLID;
					break;

				case "LADDER":
					_texture = texFac.GetTexture(Tile.LADDER);
					selectedBlock = BlockCursorType.TILE_LADDER;
					break;

				//case "PIPE":
				//	_texture = texFac.GetTexture(Tile.LADDER);
				//	selectedBlock = BlockCursorType.TILE_LADDER;
				//	break;

				case "CREDITPOINTS_5":
					_texture = texFac.GetTexture(Item.CREDITPOINTS_5);
					selectedBlock = BlockCursorType.ITEM_CREDITPOINTS_5;
					break;

				case "CREDITPOINTS_10":
					_texture = texFac.GetTexture(Item.CREDITPOINTS_10);
					selectedBlock = BlockCursorType.ITEM_CREDITPOINTS_10;
					break;

				case "CREDITPOINTS_15":
					_texture = texFac.GetTexture(Item.CREDITPOINTS_15);
					selectedBlock = BlockCursorType.ITEM_CREDITPOINTS_15;
					break;

				case "ENERGYDRINK":
					_texture = texFac.GetTexture(Item.ENERGYDRINK);
					selectedBlock = BlockCursorType.ITEM_ENERGYDRINK;
					break;

				case "PIZZA":
					_texture = texFac.GetTexture(Item.PIZZA);
					selectedBlock = BlockCursorType.ITEM_PIZZA;
					break;

				case "SPAWN_ENEMY":
					_texture = texFac.GetEnemyTexture();
					selectedBlock = BlockCursorType.SPAWN_ENEMY;
					break;

				case "SPAWN_USER":
					_texture = texFac.GetSpawnTexture();
					selectedBlock = BlockCursorType.SPAWN_USER;
					break;

				default:
					_texture = cleanBrush;
					selectedBlock = BlockCursorType.TILE_EMPTY;
					break;
			}
			setTexture();
		}

		/// <summary>
		/// Handles placement related Actions
		/// </summary>
		/// <param name="c">Column to add element to</param>
		/// <param name="r">Row to add element to</param>
		private void addElementToMap(int c, int r)
		{
			if (selectedBlock != BlockCursorType.TILE_EMPTY)
			{
				if (selectedBlock.ToString().Contains("TILE"))
				{
					addTileToMap(BlockCursorToTile[selectedBlock], c, r);
				}
				else if (selectedBlock.ToString().Contains("ITEM"))
				{
					addItemToMap(BlockCursorToItem[selectedBlock], c, r);
				}
				else if (selectedBlock == BlockCursorType.SPAWN_ENEMY)
				{
					var args = new EnemySpawnArgs() { x = c, y = r, name = "", code = "", locked = false };
					if (EditorEnemyDialog != null)
						EditorEnemyDialog(this, args);
				}
				else if (selectedBlock == BlockCursorType.SPAWN_USER)
				{
					replaceSpawnPoint(c, r);
				}
				else
				{
					//addItemToMap(BlockCursorToItem[selectedBlock], c, r);
					return;
				}
			}
			else
			{
				removeTile(c, r);
				removeItem(c, r);
			}
		}

        /// <summary>
        /// Sends a message that the SpawnPoint position changed
        /// </summary>
        /// <param name="c">new column of SpawnPoint</param>
        /// <param name="r">new row of SpawnPoint</param>
        private void replaceSpawnPoint(int c, int r)
        {
            var x = c;
            var y = r;
            var message = new SpawnPointReplaceMessage(x, y);
            this.ClientChannelHandler.sendReplaceSpawnPointMessage(message);
        }
 
		/// <summary>
		/// Calls the EditorInit Handler to trigger an Event
		/// </summary>
		private void HandleEditorInit()
		{
			EventHandler<EventArgs> handler = EditorInit;
			if (handler != null)
			{
				handler(this, new EventArgs());
			}
			else
			{
				Console.Error.WriteLine("EditorInit Handler not found");
			}
		}

		#region ParseMessages
		/// <summary>
		/// Parses and Executes a TileRemovedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseTileRemovedMessage(string msg)
		{
			TileRemovedMessage trm = Serializer.Deserialize<TileRemovedMessage>(msg);
			Map.RemoveTile(trm.posX, trm.posY);
		}

		/// <summary>
		/// Parses and Executes a TilePlacedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		public void ParseTilePlacedMessage(String msg)
		{
			TilePlacedMessage TilePlacedMessage = Serializer.Deserialize<TilePlacedMessage>(msg);
			TileDTO tileDTO = TilePlacedMessage.tileDTO;
			MapTile placeable = tileDTO.toPlaceable();
			Map.AddSolid(placeable);
		}


		/// <summary>
		/// Parses and Executes a AddSectionMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseAddSection(string msg)
		{
			AddSectionMessage message = Serializer.Deserialize<AddSectionMessage>(msg);
			Map.AddSection(message.sections);
			HandleEditorInit();
		}

		/// <summary>
		/// Parses and Executes a CursorIdMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseCursorIdMessage(string msg)
		{
			CursorIdMessage message = Serializer.Deserialize<CursorIdMessage>(msg);
			_uuid = message.uuid;
			setTexture();
		}

		/// <summary>
		/// Parses and Executes a CursorMovedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseCursorMovedMessage(string msg)
		{
			CursorMovedMessage cursorMovedMessage = Serializer.Deserialize<CursorMovedMessage>(msg);
			string uuid = cursorMovedMessage.cursor.uuid;
			if (Cursors.ContainsKey(uuid))
			{
				Cursors[uuid].X = cursorMovedMessage.cursor.gridX;
				Cursors[uuid].Y = cursorMovedMessage.cursor.gridY;
			}
		}


		private void ParseSpawnPointReplaceMessage(string msg)
		{
			SpawnPointReplaceMessage message = Serializer.Deserialize<SpawnPointReplaceMessage>(msg);
			var vector = message.spawn;
			Map.ReplaceSpawnPoint(vector);
		}


		private void ParseEditorCommandMessage(string msg)
		{
			throw new NotImplementedException();
		}

		public class EditorInfoArgs : EventArgs
		{
			public String msg;

			public EditorInfoArgs()
			{
			}
		}

		public class EnemySpawnArgs : EventArgs
		{
			public String code;
			public String name;
			public int x, y;
			public bool locked;

			public EnemySpawnArgs()
			{
			}
		}


		/// <summary>
		/// Parses and Executes a EditorInitMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseEditorInitMessage(String msg)
		{
			EditorInitMessage eim = Serializer.Deserialize<EditorInitMessage>(msg);
			Map.populateMap(eim.mapDTO);
			HandleEditorInit();

		}

		/// <summary>
		/// Parses and Executes a ItemPlacedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseItemPlacedMessage(string msg)
		{
			//Console.WriteLine(msg);
			ItemPlacedMessage itemPlacedMessage = Serializer.Deserialize<ItemPlacedMessage>(msg);
			ItemDTO idto = itemPlacedMessage.itemDTO;
			MapItem toPlace = idto.toPlaceable();
			Map.AddItem(toPlace);
		}

		/// <summary>
		/// Parses and Executes a ItemRemovedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseItemRemovedMessage(string msg)
		{
			ItemRemovedMessage irm = Serializer.Deserialize<ItemRemovedMessage>(msg);
			Map.RemoveItem(irm.posX, irm.posY);
		}

		/// <summary>
		/// Parses and Executes a KickedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseKickMessage(string msg)
		{
			if (EditorKick != null)
			{
				EditorKick(this, new EventArgs());
			}
			else
			{
				Console.Error.WriteLine("EditorKick Handler not found");
			}
		}

		/// <summary>
		/// Parses and Executes a PlayerQuitMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParsePlayerQuitMessage(string msg)
		{
			PlayerQuitMessage playerQuitMessage = Serializer.Deserialize<PlayerQuitMessage>(msg);
			if (Cursors.ContainsKey(playerQuitMessage.uuid))
			{
				Cursors.Remove(playerQuitMessage.uuid);
			}
		}

		/// <summary>
		/// Parses and Executes a RemoveSectionMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseRemoveSection(string msg)
		{
			RemoveSectionMessage message = Serializer.Deserialize<RemoveSectionMessage>(msg);
			Map.RemoveSection(message.sections);
			HandleEditorInit();
		}

		/// <summary>
		/// Parses and Executes a EditorSessionJoinedMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseUserJoinedSessionMessage(string msg)
		{
			EditorSessionJoinedMessage message = Serializer.Deserialize<EditorSessionJoinedMessage>(msg);
			Cursors.Clear();
			foreach (CursorDTO c in message.cursor)
			{
				_cursors.Add(c.uuid, new BlockCursor(c.color, c.gridX, c.gridY));
			}
			setTexture();
		}

		/// <summary>
		/// Sets the Texture for own Cursor
		/// </summary>
		private void setTexture()
		{
			try
			{
				_cursors[_uuid].Texture = _texture;
			}
			catch (ArgumentNullException e)
			{
				Console.Error.WriteLine("uuid Not set yet");
			}
			catch (KeyNotFoundException e)
			{
				Console.Error.WriteLine("uuid Not set yet");
			}
		}

		private void ParseEnemySpawnPointPlacedMessage(string msg)
		{
			EnemySpawnPointPlacedMessage message = Serializer.Deserialize<EnemySpawnPointPlacedMessage>(msg);
			EnemySpawnPointDTO dto = message.enemySpawnDTO;
			MapEnemySpawnPoint sp = new MapEnemySpawnPoint();
			sp.posX = dto.gridX;
			sp.posY = dto.gridY;
			Map.AddEnemySpawnPoint(sp);
		}

		private void ParseEnemySpawnPointRemovedMessage(string msg)
		{
			EnemySpawnPointRemovedMessage message = Serializer.Deserialize<EnemySpawnPointRemovedMessage>(msg);
			Map.RemoveEnemySpawnPoint(message.enemySpawnDTO.gridX, message.enemySpawnDTO.gridY);
		}

		private void ParseEnemySpawnPointLockAcceptMessage(string msg)
		{
			EnemySpawnPointLockAcceptMessage message = Serializer.Deserialize<EnemySpawnPointLockAcceptMessage>(msg);
			EnemySpawnPointDTO dto = message.enemySpawnDTO;
			var args = new EnemySpawnArgs() { x = dto.gridX, y = dto.gridY, name = dto.name, code = dto.code, locked = true };
			if (EditorEnemyDialog != null)
				EditorEnemyDialog(this, args);
		}



		#endregion ParseMessages
	}
}