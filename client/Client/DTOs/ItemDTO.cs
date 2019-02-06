using LoadRunnerClient.MapAndModel;

namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about an Item on the Map
	/// </summary>
	public class ItemDTO
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

		public ItemDTO()
		{
		}

		/// <summary>
		/// Constructor of the ItemDTO
		/// </summary>
		/// <param name="type">Type of Item (later converted to Item Enum)</param>
		/// <param name="gridX">X-Position on Map</param>
		/// <param name="gridY">Y-Position on Map</param>
		public ItemDTO(string type, int gridX, int gridY)
		{
			this.type = type;
			this.gridX = gridX;
			this.gridY = gridY;
		}

		/// <summary>
		/// Method for converting the DTO into a MapItem instance
		/// </summary>
		/// <returns>MapItem Instance of DTO</returns>
		public MapItem toPlaceable()
		{
			//TODO:toPlaceable id,tile
			Item item = Item.CREDITPOINTS_5;
			switch (_type)
			{
				case ("CREDITPOINTS_5"):
					item = Item.CREDITPOINTS_5;
					break;
				case ("CREDITPOINTS_10"):
					item = Item.CREDITPOINTS_10;
					break;
				case ("CREDITPOINTS_15"):
					item = Item.CREDITPOINTS_15;
					break;
				case ("ENERGYDRINK"):
					item = Item.ENERGYDRINK;
					break;

				case ("PIZZA"):
					item = Item.PIZZA;
					break;
			}
			MapItem placeable = new MapItem(item, gridX, gridY);

			return placeable;
		}
	}
}
