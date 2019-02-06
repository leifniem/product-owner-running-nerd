using LoadRunnerClient.MapAndModel;

namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about a single MapTile
	/// </summary>
	public class TileDTO
    {
        private string _type;
        private int _gridX;
        private int _gridY;

        public string type
        {
            get => _type;
            set => _type = value;
        }

        public int gridX
        {
            get => _gridX;
            set => _gridX = value;
        }

        public int gridY
        {
            get => _gridY;
            set => _gridY = value;
        }

        public TileDTO()
        {
        }

		/// <summary>
		/// Constructor for a TileDTO
		/// </summary>
		/// <param name="type">Type of Tile (Later converted to Tile-Enum)</param>
		/// <param name="gridX">X-Position of Tile</param>
		/// <param name="gridY">Y-Position of Tile</param>
        public TileDTO(string type, int gridX, int gridY)
        {
            this.type = type;
            this.gridX = gridX;
            this.gridY = gridY;
        }

		/// <summary>
		/// Method converting the TileDTO into a MapTile instance
		/// </summary>
		/// <returns>MapTile instance</returns>
        public MapTile toPlaceable()
        {
            //TODO:toPlaceable id,tile
            Tile tile = Tile.SOLID;
            switch (_type)
            {
				case ("DESTROYABLE_SOLID"):
                    tile = Tile.DESTROYABLE_SOLID;
                    break;

                case ("SOLID"):
                    tile = Tile.SOLID;
                    break; 
					
                case ("LADDER"):
                    tile = Tile.LADDER;
                    break;

                case ("ENEMY"):
                    tile = Tile.ENEMY;
                    break;
            }
            MapTile placeable = new MapTile(tile);
            placeable.posX = gridX;
            placeable.posY = gridY;
            return placeable;
        }
    }
}