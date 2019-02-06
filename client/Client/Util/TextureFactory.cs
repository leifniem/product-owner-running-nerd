using LoadRunnerClient.MapAndModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.Util
{
	/// <summary>
	/// Factory Class to provide Textures to tiles, items and spawnpoints.
	/// </summary>
	public class TextureFactory
	{
		private Dictionary<Tile, Brush> blockTextures = new Dictionary<Tile, Brush>();
		private Dictionary<Item, Brush> itemTextures = new Dictionary<Item, Brush>();

		public TextureFactory()
		{
			/// Adding global textures to dictionary upon initialization
			blockTextures.Add(Tile.DESTROYABLE_SOLID, Application.Current.Resources["BrickTexture"] as Brush);
			blockTextures.Add(Tile.SOLID, Application.Current.Resources["SolidTexture"] as Brush);
			blockTextures.Add(Tile.LADDER, Application.Current.Resources["LadderTexture"] as Brush);
			//blockTextures.Add(Tile.PIPE, Application.Current.Resources["PipeTexture"] as Brush);

			itemTextures.Add(Item.CREDITPOINTS_5, Application.Current.Resources["FiveCPTexture"] as Brush);
			itemTextures.Add(Item.CREDITPOINTS_10, Application.Current.Resources["TenCPTexture"] as Brush);
			itemTextures.Add(Item.CREDITPOINTS_15, Application.Current.Resources["FifteenCPTexture"] as Brush);
			itemTextures.Add(Item.ENERGYDRINK, Application.Current.Resources["EnergyDrinkTexture"] as Brush);
			itemTextures.Add(Item.PIZZA, Application.Current.Resources["PizzaTexture"] as Brush);
		}

		public Brush GetTexture(Tile type)
		{
			return blockTextures[type];
		}

		public Brush GetTexture(Item type)
		{
			return itemTextures[type];
		}

		public Brush GetEnemyTexture()
		{
			return Application.Current.Resources["EnemySpawnTexture"] as Brush;
		}

		public Brush GetSpawnTexture()
		{
			return Application.Current.Resources["PlayerSpawnTexture"] as Brush;
		}
	}
}

