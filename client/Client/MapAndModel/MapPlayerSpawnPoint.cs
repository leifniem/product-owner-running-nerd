using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{

	/// <summary>
	/// PlayerSpawnPoint Model class to see visualize the player's spawn point in the map editor.
	/// </summary>
	public class MapPlayerSpawnPoint : MapSpawnPoint
    {

        public MapPlayerSpawnPoint()
        {
            _texture = Application.Current.Resources["PlayerSpawnTexture"] as Brush;
        }

    }
}
