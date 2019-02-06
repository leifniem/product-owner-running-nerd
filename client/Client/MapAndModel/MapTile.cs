using LoadRunnerClient.DTOs;

using System;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
    /// <summary>
    /// TileModel class to see every tile we use on our map section, accessible by an ID
    /// </summary>
    public class MapTile : ObservableModelBase
    {
        private int _posX;
        private int _posY;
        private Tile _tile;
        private Brush _texture;

        public MapTile()
        {

        }

        public MapTile(Tile tile, int posX, int posY)
        {
            this.tile = tile;
            this.posX = posX;
            this.posY = posY;
        }

        public MapTile(Tile tile)
        {
            this._tile = tile;
            this._texture = GetBrushByTile(tile);
		}

        public int posX
        {
            get => _posX;
            set
            {
                if (_posX != value)
                {
                    _posX = value;
                    offsetX = value;
                    OnPropertyChanged("POSX");
                }
            }
        }

        private int _offsetX;
        public int offsetX
        {
            get => _offsetX;
            set { _offsetX = (value % 32);
                OnPropertyChanged("offset");
            }
        }

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
		/// Duplicates the MapTile
		/// </summary>
		/// <returns></returns>
        public MapTile clone()
        {
            MapTile pl = new MapTile(tile);
            pl.posX = posX;
            pl.posY = posY;
            return pl;
        }

        public Tile tile
        {
            get => _tile;
            set
            {
                _tile = value;
                _texture = GetBrushByTile(value);

                OnPropertyChanged("Tile");
                OnPropertyChanged("Texture");
            }
        }

        public Brush Texture
        {
            get => _texture;
        }

		/// <summary>
		/// Gets the texture for the TileType
		/// </summary>
		/// <param name="tile"></param>
		/// <returns>ImageBrush of matching texture</returns>
        private Brush GetBrushByTile(Tile tile)
        {
            if (tile == Tile.SOLID)
                return Application.Current.Resources["SolidTexture"] as Brush;

            if (tile == Tile.LADDER)
                return Application.Current.Resources["LadderTexture"] as Brush;
            if (tile == Tile.DESTROYABLE_SOLID)
                return Application.Current.Resources["BrickTexture"] as Brush;
            if (tile == Tile.DESTROYED_SOLID)
                return Application.Current.Resources["HoleTexture"] as Brush;

            return null;
        }

		/// <summary>
		/// Converts the MapTile into a DTO
		/// </summary>
		/// <returns>TileDTO of Tile</returns>
        public TileDTO totileDTO()
        {
            TileDTO tileDTO = new TileDTO(tile.ToString(), posX, posY);
            return tileDTO;
        }

        public override string ToString()
        {
            return $"Tile({_tile},X={posX},Y={posY})";
        }
    }
}