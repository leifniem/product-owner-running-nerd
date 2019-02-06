using System;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel {

	/// <summary>
	/// EnemySpawnPoint Model class to see visualize an enemy spawn point in the map editor.
	/// </summary>
	public class MapEnemySpawnPoint : MapSpawnPoint {

		public MapEnemySpawnPoint() {
			_texture = Application.Current.Resources["EnemySpawnTexture"] as Brush;
		}

	}
}