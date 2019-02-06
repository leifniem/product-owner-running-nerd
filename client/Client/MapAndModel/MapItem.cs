using LoadRunnerClient.DTOs;

using System;
using System.Windows;
using System.Windows.Media;

namespace LoadRunnerClient.MapAndModel
{
    /// <summary>
    /// Item Model class to see every item we use on our map section, accessible by an ID
    /// </summary>
    public class MapItem:ObservableModelBase
    {
        
        private int _posX;
        private int _posY;
        private Item _item;
		private Brush _texture;

        public MapItem()
        {
        }

        
        public MapItem(Item item, int posX, int posY)
        {
            this.item = item;
            this.posX = posX;
            this.posY = posY;
			assignTexture(item);
        }

		/// <summary>
		/// Assign Texture by Item Type
		/// </summary>
		/// <param name="item"></param>
		public void assignTexture(Item item){
			if (item == Item.CREDITPOINTS_5)
			{
				_texture = Application.Current.Resources["FiveCPTexture"] as Brush;
			}
			else if (item == Item.CREDITPOINTS_10)
			{
				_texture = Application.Current.Resources["TenCPTexture"] as Brush;
			}
			else if (item == Item.CREDITPOINTS_15)
			{
				_texture = Application.Current.Resources["FifteenCPTexture"] as Brush;
			}
			else if (item == Item.PIZZA)
			{
				_texture = Application.Current.Resources["PizzaTexture"] as Brush;
			}
			else if (item == Item.ENERGYDRINK)
			{
				_texture = Application.Current.Resources["EnergyDrinkTexture"] as Brush;
            }
            else
            {
                _texture = null;
			OnPropertyChanged("Texture");
            }
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
            set
            {
                _offsetX = (value % 32);
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

        public MapItem clone()
        {
            MapItem pl = new MapItem(_item,posX,posY);
            return pl;
        }

        public Item item
        {
            get => _item;
            set
            {
                _item = value;
                assignTexture(value);
				OnPropertyChanged("Item");
            }
        }

        public Brush Texture
        {
            get => _texture;
        }

		/// <summary>
		/// Convert Item to DTO
		/// </summary>
		/// <returns>ItemDTO of Item</returns>
        public ItemDTO toItemDTO()
        {
            ItemDTO itemDTO = new ItemDTO(item.ToString(), posX, posY);
            return itemDTO;
        }

        public override string ToString()
        {
            return $"Item({_item},X={posX},Y={posY})";
        }
    }
}
