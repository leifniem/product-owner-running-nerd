using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{

	/// <summary>
	///SpawnPoint Model class to see visualize a spawn point in the map editor.
	/// </summary>
	public class MapSpawnPoint : ObservableModelBase
    {
        private int _posX;
        private int _posY;
        internal Brush _texture;

		/// <summary>
		///  X-Position of the spawn point on the maps grid
		/// </summary>
		public int posX
        {
            get => _posX;
            set
            {
                if (_posX != value)
                {
                    _posX = value;
                    OnPropertyChanged("POSX");
                }
            }
        }

		/// <summary>
		///  Y-Position of the spawn point on the maps grid
		/// </summary>
		public int posY
        {
            get => _posY;
            set
            {
                if (_posY != value)
                {
                    _posY = value;
                    OnPropertyChanged("POSY");
                }
            }
        }

		/// <summary>
		/// The texture to be displayed in the map editor.
		/// </summary>
		public Brush Texture
        {
            get => _texture;
        }


    }
}
