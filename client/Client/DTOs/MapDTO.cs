using LoadRunnerClient.MapAndModel;

namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass a Map
	/// </summary>
	public class MapDTO
    {
        private MapMetaDTO _meta;
        private Tile[][] _tiles;
        private Item[][] _items;
		private EnemySpawnPointDTO[] _enemySpawnPoints;
        private Vector2Int _spawn;

        public MapDTO()
        {
        }

		/// <summary>
		/// Constructor of the MapDTO
		/// </summary>
		/// <param name="meta">MapMetaDTO instance providing information about properties of the Map</param>
		/// <param name="tiles">Collection of Tile Enums describing the Tiles Type</param>
		/// <param name="items">Collection of Item Enums describing the Items Type</param>
		public MapDTO(MapMetaDTO meta, Tile[][] tiles, Item[][] items)
        {
            this._meta = meta;
            this._tiles = tiles;
            this._items = items;
        }

        public MapMetaDTO meta
        {
            get => _meta;
            set => _meta = value;
        }

        public Tile[][] tiles
        {
            get => _tiles;
            set => _tiles = value;
        }


        public Item[][] items
        {
            get => _items;
            set => _items = value;
        }
    
		public EnemySpawnPointDTO[] enemySpawnPoints {
			get => _enemySpawnPoints;
			set => _enemySpawnPoints = value;
		}

        public Vector2Int spawn
        {
            get => _spawn;
            set => _spawn = value;
        }

	}
}
