using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LoadRunnerClient.MapAndModel
{
    /// <summary>
    /// Model for a character in Game Mode
    /// </summary>
    public class GameCharacter : ObservableViewModelBase
    {
        private float _posX;
        private float _posY;
        private string _id;
        private SolidColorBrush _playerColor;
        private bool _isPlayer;
        private BitmapImage _sprite;
        private GameCharacterState _state = GameCharacterState.STANDING;
        private int direction = 1;
        private int _lifePoints = 3;
        private int _creditPoints = 0;
        private int _energyDrink = 0;
        private double _opacity = 1.0;
        public Dictionary<Item, int> _inventory = new Dictionary<Item, int>();
        public event EventHandler<EventArgs> playerSound;

        /// <summary>
        /// Dictionary for sprite calculation.
        /// Sets relation between GameCharacterState and Sprite Row
        /// </summary>
        private Dictionary<GameCharacterState, int> stateToInt = new Dictionary<GameCharacterState, int>()
            {
                { GameCharacterState.RUNNING, 0 },
                { GameCharacterState.FALLING, 1 },
                { GameCharacterState.JUMPING, 1 },
                { GameCharacterState.STANDING, 0 },
                { GameCharacterState.CLIMBING, 2 },
            };

        /// <summary>
        /// Constant of Sprite width for Sprite Calculation
        /// </summary>
        private const int SPRITEWIDTH = 42;
        /// <summary>
        /// Constant of Sprite height for Sprite Calculation
        /// </summary>
        private const int SPRITEHEIGHT = 60;

        /// <summary>
        /// Number of Frames in each direction (without neutral state)
        /// </summary>
        private const int NUMFRAMES = 3;

        /// <summary>
        /// Number of Messages after which a new Sprite Frame is shown
        /// </summary>
        private const int UPDATECYCLE = 30;

        /// <summary>
        /// Number of Messages after which to play sound again if state is not changed
        /// </summary>
        private const int AUDIOCYCLE = 90;

        /// <summary>
        /// Crop Region for Sprite
        /// </summary>
        private Rect _spriteViewbox = new Rect(84, 0, 42, 60);

        /// <summary>
        /// Current Frame of Sprite Animation
        /// </summary>
        private int currentFrame = 0;

        /// <summary>
        /// Number of Messages received since Frame changed
        /// </summary>
        private int steps = 0;

        /// <summary>
        /// Number of Messages received since sound was played
        /// </summary>
        private int stepsSound = 0;

        /// <summary>
        /// Initialize Character with ID
        /// </summary>
        /// <param name="id">ID to match character by</param>
        /// <param name="color">Name of Color user will have</param>
        /// <param name="isEnemy">Determines if Character is an Enemy</param>
        public GameCharacter(string id, string color, bool isEnemy)
        {
            this._id = id;
            this._playerColor = Application.Current.Resources["COLOR_" + color] as SolidColorBrush;
            this.IsPlayer = !isEnemy;
            //this._sprite = Application.Current.Resources["SPRITE_" + color] as string;
            if (IsPlayer)
            {
                this._sprite = new BitmapImage(new Uri("./Resources/Images/sprites-" + color.ToLower() + ".png", UriKind.Relative));
                this._sprite.Freeze();
            }
            else
            {
                this._sprite = new BitmapImage(new Uri("./Resources/Images/prof-sprites.png", UriKind.Relative));
                this._sprite.Freeze();
            }
        }

        /// <summary>
        /// X-Position of character
        /// </summary>
        public float PosX
        {
            get => _posX;
            set
            {
                if (_posX != value)
                {
                    direction = _posX < value ? 1 : 0;
                    _posX = value;
                    calculateSprite(false);
                    OnPropertyChanged("PosX");
                }
            }
        }

        /// <summary>
        /// Y-Position of Character
        /// </summary>
        public float PosY
        {
            get => _posY;
            set
            {
                if (_posY != value)
                {
                    _posY = value;
                    calculateSprite(false);
                    OnPropertyChanged("PosY");
                }
            }
        }

        /// <summary>
        /// Character-ID
        /// </summary>
        public string Id => _id;

        /// <summary>
        /// Color of Character
        /// </summary>
        public SolidColorBrush PlayerColor { get => _playerColor; set => _playerColor = value; }

        /// <summary>
        /// Sprite of Character
        /// </summary>
        public BitmapImage Sprite { get => _sprite; }

        /// <summary>
        /// State of GameCharacter movement
        /// </summary>
        public GameCharacterState State
        {
            get => _state;
            set
            {
                bool changed = value != _state;
                calculateSprite(changed);
                _state = value;
                if (changed)
                {
                    OnPropertyChanged("State");
                }
            }
        }

        /// <summary>
        /// Cropped Region of the Players Sprite
        /// </summary>
        public Rect SpriteViewbox { get => _spriteViewbox; }

        /// <summary>
        /// Life points of character
        /// </summary>
        public int LifePoints
        {
            get => _lifePoints;
            set
            {
                if (_lifePoints != value)
                {
                    if (value == 0)
                    {
                        Opacity = 0.4;
                    }

                    _lifePoints = value;
                    OnPropertyChanged("LifePoints");
                }
            }
        }

        public double Opacity
        {
            get => _opacity;
            set
            {
                _opacity = value;
                OnPropertyChanged("Opacity");
            }
        }

        /// <summary>
        /// Credit points of character
        /// </summary>
        public int CreditPoints
        {
            get => _creditPoints;
            set
            {
                if (_creditPoints != value)
                {
                    _creditPoints = value;
                    OnPropertyChanged("CreditPoints");
                }
            }
        }

        /// <summary>
        /// Boolean determining if EnergyDrink is currently in use by Character
        /// </summary>
        public int EnergyDrink
        {
            get => _energyDrink;
            set
            {
                if (_energyDrink != value)
                {
                    _energyDrink = value;
                    OnPropertyChanged("EnergyDrink");
                }
            }
        }
        /// <summary>
        /// Calculate Which Portion of Sprite to show and if there's a sound to play
        /// </summary>
        /// <param name="changedState">Parameter needed to enter neutral position o new Character State</param>
        private void calculateSprite(bool changedState)
        {
            if (changedState)
            {
                currentFrame = 0;
                steps = 0;
                stepsSound = 0;
            }
            else
            {
                steps++;
                stepsSound++;
                if (
                    (steps %= UPDATECYCLE) == 0
                    && _state != GameCharacterState.FALLING
                    && _state != GameCharacterState.STANDING
                    && _state != GameCharacterState.JUMPING
                    )
                {
                    currentFrame %= NUMFRAMES;
                    currentFrame++;
                }
            }
            if ((stepsSound %= AUDIOCYCLE) == 0)
            {
                playerSound(this, new EventArgs());
            }

            _spriteViewbox.X = currentFrame * SPRITEWIDTH * 2 + direction * SPRITEWIDTH;
            _spriteViewbox.Y = stateToInt[_state] * SPRITEHEIGHT;
            OnPropertyChanged("SpriteViewbox");
        }

        /// <summary>
        /// Adds a collected item to the Characters Inventory
        /// </summary>
        /// <param name="item">Type of Item</param>
        public void AddItemToInventory(Item item)
        {
            if (this._inventory.ContainsKey(item))
            {

                switch (item)
                {
                    case Item.ENERGYDRINK:
                        EnergyDrink += 1;
                        break;
                }
                this._inventory[item] += 1;
                OnPropertyChanged("Inventory");
            }
            else
            {
                switch (item)
                {
                    case Item.ENERGYDRINK:
                        EnergyDrink += 1;
                        break;
                }
                this._inventory.Add(item, 1);
                OnPropertyChanged("Inventory");
            }
        }

        /// <summary>
        /// Removes an item form the Characters inventory
        /// </summary>
        /// <param name="item">Type of Item</param>
        public void RemoveItemFromInventory(Item item)
        {
            if (this._inventory.ContainsKey(item))
            {
                if (this._inventory[item] > 0)
                {
                    switch (item)
                    {
                        case Item.ENERGYDRINK:
                            EnergyDrink -= 1;
                            break;
                    }
                    this._inventory[item] -= 1;
                    OnPropertyChanged("Inventory");
                }
            }
        }


        /// <summary>
        /// Number of EnergyDrinks in Characters inventory
        /// </summary>
        public int EnergyInventory
        {
            get => _inventory[Item.ENERGYDRINK];
        }

        public bool IsPlayer { get => _isPlayer; set => _isPlayer = value; }

        public override string ToString()
        {
            return $"Character({_id},X={PosX},Y={PosY})";
        }
    }
}