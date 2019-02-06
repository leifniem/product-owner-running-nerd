using ExtensionsMethods;
using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
	/// <summary>
	/// MapSection Class to load a map and create all models we need to build the map
	/// /// Author Florian & Leif
	/// </summary>
	public class Map : ObservableModelBase
	{
		/// Constant variable for calculating Section offsets
		public const int SECTION_WIDTH = 32;
		/// Constant variable for calculating Section offsets
		public const int SECTION_HEIGHT = 16;

		/// <summary>
		/// Offset of the section shown by the client in relation to the Map
		/// </summary>
		private int _offset;

		/// <summary>
		/// Number of sections in Map
		/// </summary>
		private int _sections;

		/// <summary>
		/// Dictionary of GameCharacters on complete Map
		/// </summary>
		private ObservableDictionary<string, GameCharacter> _players = new ObservableDictionary<string, GameCharacter>();


		/// <summary>
		/// Collection of Items on complete Map
		/// </summary>
		private MapObservableCollection<MapItem> _items = new MapObservableCollection<MapItem>();

		/// <summary>
		/// Collection of Tiles on the whole map
		/// </summary>
		private MapObservableCollection<MapTile> _solids = new MapObservableCollection<MapTile>();

        /// <summary>
        /// All Spawnpoints for AI characters on the map
        /// </summary>
        private MapObservableCollection<MapSpawnPoint> _spawnPoints = new MapObservableCollection<MapSpawnPoint>();

		/// <summary>
		/// Calculated Tiles for Screen Section
		/// </summary>
		private ObservableCollection<MapTile> _relevantSolids = new ObservableCollection<MapTile>();

		/// <summary>
		/// Calculated Items for Screen Section
		/// </summary>
		private ObservableCollection<MapItem> _relevantItems = new ObservableCollection<MapItem>();

        /// <summary>
        /// Spawn Point for Players
        /// </summary>
        private MapPlayerSpawnPoint playerSpawn;

        
		/// <summary>
		/// Map-Constructor
		/// </summary>
		/// <param name="initMsg">GameInitialization Message with map Info</param>
		public Map(GameInitMessage initMsg)
		{
			foreach (GameCharacterDTO player in initMsg.playerDTOs)
			{
				CreateCharacter(player.uuid, player.position.X, player.position.Y, player.color, player.enemy);
			}
			foreach (GameCharacter player in _players.Values)
			{
				player.playerSound += playCharacterSounds;
			}
			populateMap(initMsg.mapDTO);
		}

		public Map()
		{
		}

		/// <summary>
		/// Constructor for Map in Editor
		/// </summary>
		/// <param name="eim">EditorInit Message</param>
		public Map(EditorInitMessage eim)
		{
			populateMap(eim.mapDTO);
		}

		/// <summary>
		/// Populate map with its given tiles, items and enemies spawn points
		/// </summary>
		/// <param name="mdto">MapDTO Object to populate from</param>
		public void populateMap(MapDTO mdto)
		{
			items.Clear();
			solids.Clear();
            spawnPoints.Clear();
			Sections = mdto.meta.numberOfSections;
			var tileList = new List<MapTile>();
			var itemList = new List<MapItem>();
			for (int x = 0; x < mdto.meta.numberOfSections * SECTION_WIDTH; x++)
			{
				for (int y = 0; y < SECTION_HEIGHT; y++)
				{
					tileList.Add(new MapTile(mdto.tiles[x][y], x, y));
					itemList.Add(new MapItem(mdto.items[x][y], x, y));
                    spawnPoints.Add(new EmptyMapSpawnPoint(x, y));
				}
			}
			solids.AddAll(tileList);
			items.AddAll(itemList);
			RelevantItems = calculateRelevantItems(items);
			RelevantSolids = calculateRelevantSolids(solids);

            var spawnVector = mdto.spawn;
            playerSpawn = new MapPlayerSpawnPoint();
            playerSpawn.posX = spawnVector.x;
            playerSpawn.posY = spawnVector.y;
            spawnPoints.Add(playerSpawn);

			for (int i = 0; i < mdto.enemySpawnPoints.Length; i++)
			{
				EnemySpawnPointDTO dto = mdto.enemySpawnPoints[i];
				MapEnemySpawnPoint enemySpawnPoint = new MapEnemySpawnPoint();
				enemySpawnPoint.posX = dto.gridX;
				enemySpawnPoint.posY = dto.gridY;
				spawnPoints.Add(enemySpawnPoint);
				OnPropertyChanged("enemySpawnPoints");
			}
		}


		/// <summary>
		/// Solids placed on map
		/// </summary>
		public MapObservableCollection<MapTile> solids
		{
			get => _solids;
			set
			{
				_solids = value;
				OnPropertyChanged("solids");
			}
		}

		public ObservableDictionary<string, GameCharacter> players { get => _players; set => _players = value; }

		public MapObservableCollection<MapItem> items
		{
			get => _items;
			set
			{
				_items = value;
				OnPropertyChanged("items");
			}
		}

		public MapObservableCollection<MapSpawnPoint> spawnPoints
		{
			get => _spawnPoints;
			set
			{
				_spawnPoints = value;
				OnPropertyChanged("enemySpawnPoints");
			}
		}

		public event EventHandler<SoundEventArgs> playSoundEventHandler;

		/// <summary>
		/// Adds a MapTile to the Map
		/// </summary>
		/// <param name="placeable">Tile to Add</param>
		public void AddSolid(MapTile placeable)
		{
			solids.RemoveAll(e => e.posX == placeable.posX && e.posY == placeable.posY);
			solids.Add(placeable);
		}

		/// <summary>
		/// Adds a MapItem to the Map
		/// </summary>
		/// <param name="placeable">Item to Add</param>
		public void AddItem(MapItem placeable)
		{
			items.RemoveAll(e => e.posX == placeable.posX && e.posY == placeable.posY);
			items.Add(placeable);
		}

		/// <summary>
		/// Removes the MapTile at given position
		/// </summary>
		/// <param name="posX">Column</param>
		/// <param name="posY">Row</param>
		public void RemoveTile(int posX, int posY)
		{
			solids.RemoveAll(element => element.posX == posX && element.posY == posY);
			solids.Add(new MapTile(Tile.EMPTY, posX, posY));
		}

		/// <summary>
		/// Removes the MapItem at given position
		/// </summary>
		/// <param name="posX">Column</param>
		/// <param name="posY">Row</param>
		public void RemoveItem(int posX, int posY)
		{
			MapItem changeMapItem = items.First(solid => solid.posX == posX && solid.posY == posY);
			changeMapItem.item = Item.EMPTY;
			if (RelevantItems.Remove(changeMapItem)) { RelevantItems.Add(changeMapItem); }
		}

		/// <summary>
		/// Add additional Spawn point for AI
		/// </summary>
		/// <param name="enemy"></param>
		public void AddEnemySpawnPoint(MapEnemySpawnPoint enemy)
		{
			spawnPoints.Add(enemy);
		}

		/// <summary>
		/// Remove AI Spawn previously placed
		/// </summary>
		/// <param name="posX"></param>
		/// <param name="posY"></param>
		public void RemoveEnemySpawnPoint(int posX, int posY)
		{
			spawnPoints.RemoveAll(element => element.posX == posX && element.posY == posY);
            spawnPoints.Add(new EmptyMapSpawnPoint(posX, posY));
            if (playerSpawn.posX == posX && playerSpawn.posY == posY)
            {
                spawnPoints.Add(playerSpawn);
            }            
		}

        /// <summary>
        /// Changes The PlayerSpawnPoint
        /// </summary>
        /// <param name="spawnPoint"></param>
        public void ReplaceSpawnPoint(Vector2Int vector)
        {
            this.playerSpawn.posX = vector.x;
            this.playerSpawn.posY= vector.y;
        }

		/// <summary>
		/// Calculate Tiles relevant to displayed section
		/// </summary>
		/// <param name="input">Tiles of complete map</param>
		/// <returns>Filtered List of Tiles limited to X-Range</returns>
		private ObservableCollection<MapTile> calculateRelevantSolids(MapObservableCollection<MapTile> input)
		{
			// Filter Elements by X-Positon and turn into List
			ObservableCollection<MapTile> rel = new ObservableCollection<MapTile>(input.Where(element => element.posX >= Offset && element.posX < (Offset + SECTION_WIDTH)));

			return rel;
		}

		/// <summary>
		/// Calculate Items relevant to displayed section
		/// </summary>
		/// <param name="input">Tiles of complete map</param>
		/// <returns>Filtered List of Items limited to X-Range</returns>
		private ObservableCollection<MapItem> calculateRelevantItems(MapObservableCollection<MapItem> input)
		{
			ObservableCollection<MapItem> rel = new ObservableCollection<MapItem>(input.Where(element => element.posX >= Offset && element.posX < (Offset + SECTION_WIDTH)));

			return rel;
		}

		public int Offset
		{
			get => _offset;
			set
			{
				_offset = value * SECTION_WIDTH;
				RelevantSolids = calculateRelevantSolids(_solids);
				RelevantItems = calculateRelevantItems(_items);
			}
		}

		public ObservableCollection<MapTile> RelevantSolids
		{
			get => _relevantSolids;
			set
			{
				_relevantSolids = value;
				OnPropertyChanged("RelevantSolids");
			}
		}

		public ObservableCollection<MapItem> RelevantItems
		{
			get => _relevantItems;
			set
			{
				_relevantItems = value;
				OnPropertyChanged("RelevantItems");
			}
		}

		/// <summary>
		/// Add a new Player to the Map
		/// </summary>
		/// <param name="id">Integer to identify Player</param>
		/// <param name="x">Initial X Coordinate</param>
		/// <param name="y">Initial Y-Coordinate</param>
		/// <param name="color">Color assigned to the Character</param>
		/// <param name="isEnemy">Determines if Character is an Enemy</param>
		public void CreateCharacter(string id, float x, float y, string color, bool isEnemy)
		{
			GameCharacter newChar = new GameCharacter(id, color, isEnemy);
			newChar.PosX = x;
			newChar.PosY = y;
			this.players.Add(id, newChar);
			this.UpdateCharacterPosition(id, new Vector2(x, y), GameCharacterState.STANDING);
		}

		/// <summary>
		/// temporary GameCharacter Variable to assign Event related Player to
		/// </summary>
		private GameCharacter audioRelatedPlayer;

		/// <summary>
		/// Play Sound related to Character emitting event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void playCharacterSounds(object sender, EventArgs e)
		{
			GameCharacter audioRelatedPlayer = sender as GameCharacter;
			GameSoundFactory.AvailableSounds soundName;

			/// check if GameCharacterState (RUNNING, etc.) has an accordingly named sound available
			try
			{
				soundName = (GameSoundFactory.AvailableSounds)System.Enum.Parse(
								typeof(GameSoundFactory.AvailableSounds),
								audioRelatedPlayer.State.ToString());
			}
			catch (ArgumentException err)
			{
				/// do nothing and abort
				return;
			}

			if (audioRelatedPlayer.PosX / 60 >= Offset && audioRelatedPlayer.PosX / 60 < Offset + SECTION_WIDTH)
				playSoundEventHandler(sender, new SoundEventArgs()
				{
					Msg = soundName,
					Uuid = audioRelatedPlayer.Id
				});
		}

		/// <summary>
		/// Update Tile at given Position
		/// </summary>
		/// <param name="newTile">New TileType</param>
		/// <param name="posX">Column</param>
		/// <param name="posY">Row</param>
		public void UpdateTileStatus(Tile newTile, int posX, int posY)
		{
			MapTile changeMapTile = solids.First(solid => solid.posX == posX && solid.posY == posY);
			changeMapTile.tile = newTile;
			if (RelevantSolids.Remove(changeMapTile)) { RelevantSolids.Add(changeMapTile); }
		}

		/// <summary>
		/// Update Characters Position
		/// </summary>
		/// <param name="name">Identifier of Character</param>
		/// <param name="posX">new Position X-Coordinate</param>
		/// <param name="posY">new Position Y-Coordinate</param>
		/// <param name="state">State to change Character into</param>
		public void UpdateCharacterPosition(string id, Vector2 xy, GameCharacterState state)
		{
			this.players[id].PosX = (xy.X - _offset) * 60;
			this.players[id].PosY = xy.Y * 60;
			this.players[id].State = state;
		}

		/// <summary>
		/// Update a Characters HP
		/// </summary>
		/// <param name="id">ID of Character</param>
		/// <param name="lifePoints">New Amount of HP</param>
		public void UpdateCharacterLifePoints(string id, int lifePoints)
		{
			this.players[id].LifePoints = lifePoints;
		}


		/// <summary>
		/// Update a Characters CP
		/// </summary>
		/// <param name="id">ID of Character</param>
		/// <param name="lifePoints">New Amount of CP</param>
		public void UpdateCharacterCreditPoints(string id, int creditPoints)
		{
			this.players[id].CreditPoints = creditPoints;
		}

		/// <summary>
		/// Add a section to the Map
		/// </summary>
		/// <param name="sections"></param>
		public void AddSection(int sections)
		{
			this.Sections = sections;
			var tileList = new List<MapTile>();
			var itemList = new List<MapItem>();
            var spawnList = new List<MapSpawnPoint>();
			for (int x = (sections - 1) * SECTION_WIDTH; x < sections * SECTION_WIDTH; x++)
			{
				for (int y = 0; y < SECTION_HEIGHT; y++)
				{
					//AddSolid(new MapTile(Tile.EMPTY,x,y));
					tileList.Add(new MapTile(Tile.EMPTY, x, y));
					itemList.Add(new MapItem(Item.EMPTY, x, y));
                    spawnList.Add(new EmptyMapSpawnPoint(x, y));
				}
			}
			solids.AddAll(tileList);
			items.AddAll(itemList);
            spawnPoints.AddAll(spawnList);
		}

		/// <summary>
		/// Remove a section from the Map
		/// </summary>
		/// <param name="sections"></param>
		public void RemoveSection(int sections)
		{
			this.Sections = sections;
			var tileList = new List<MapTile>();
			var itemList = new List<MapItem>();
			var spawnList = new List<MapSpawnPoint>();
			for (int x = sections * SECTION_WIDTH; x < (sections + 1) * SECTION_WIDTH; x++)
			{
				for (int y = 0; y < SECTION_HEIGHT; y++)
				{
					tileList.Add(solids.First(element => element.posX == x && element.posY == y));
					itemList.Add(items.First(element => element.posX == x && element.posY == y));
                    spawnList.Add(spawnPoints.First(element => element.posX == x && element.posY == y));
				}
			}
			solids.RemoveAll(tileList);
			items.RemoveAll(itemList);
            spawnPoints.RemoveAll(spawnList);
            if (!spawnPoints.Contains(playerSpawn))
                spawnPoints.Add(playerSpawn);
		}

		/// <summary>
		/// Remove a Player from the Map
		/// </summary>
		/// <param name="id">UUID of Player to remove</param>
		public void RemoveCharacter(string id)
		{
			if (this.players.ContainsKey(id))
			{
				this.players.Remove(id);
			}
		}

		public GameCharacter Players
		{
			get => default(GameCharacter);
			set
			{
			}
		}

		public int Sections
		{
			set
			{
				_sections = value;
				OnPropertyChanged("Sections");
			}
			get => _sections;
		}
	}
}