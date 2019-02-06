using SharpDX.XInput;
using System;
using System.Timers;

namespace LoadRunnerClient.Util
{
    public class GameController : ObservableViewModelBase
    {
        private Controller _controller;

        ///Axis
        private int _xAxis = 0;

        private int _yAxis = 0;

        public const short TRIGGER_LIMIT = 10;
        public const short STICK_LIMIT = 5000;

        /// <summary>
        ///  Buttons
        /// </summary>
        private bool _a = false;
        private bool _b = false;
        //private bool _x = false;
        //private bool _y = false;
        private bool _up = false;
        private bool _down = false;
        private bool _left = false;
        private bool _right = false;
        private bool _leftShoulder = false;
        private bool _rightShoulder = false;
        //private bool _leftStickPress = false;
        //private bool _rightStickPress = false;
        private bool _start = false;
        //private bool _back = false;
        //private bool _leftTrigger = false;
        //private bool _rightTrigger = false;

        private Gamepad gpState;

        public GameController()
        {
            _controller = new Controller(UserIndex.One);

            if (!_controller.IsConnected)
            {
                throw new Exception("Controller is not connected.");
            }
        }

        public void checkInput(object sender, ElapsedEventArgs e)
        {
            gpState = _controller.GetState().Gamepad;
            XAxis = gpState.LeftThumbX;
            YAxis = gpState.LeftThumbY;
            A = gpState.Buttons.HasFlag(GamepadButtonFlags.A);
            B = gpState.Buttons.HasFlag(GamepadButtonFlags.B);
            //X = gpState.Buttons.HasFlag(GamepadButtonFlags.X);
            //Y = gpState.Buttons.HasFlag(GamepadButtonFlags.Y);
            LeftShoulder = gpState.Buttons.HasFlag(GamepadButtonFlags.LeftShoulder);
            RightShoulder = gpState.Buttons.HasFlag(GamepadButtonFlags.RightShoulder);
            //LeftStickPress = gpState.Buttons.HasFlag(GamepadButtonFlags.LeftThumb);
            //RightStickPress = gpState.Buttons.HasFlag(GamepadButtonFlags.RightThumb);
            Start = gpState.Buttons.HasFlag(GamepadButtonFlags.Start);
            //Back = gpState.Buttons.HasFlag(GamepadButtonFlags.Back);
            //LeftTrigger = gpState.LeftTrigger > TRIGGER_LIMIT;
            //RightTrigger = gpState.RightTrigger > TRIGGER_LIMIT;
            Left = gpState.Buttons.HasFlag(GamepadButtonFlags.DPadLeft);
            Right = gpState.Buttons.HasFlag(GamepadButtonFlags.DPadRight);
            Up = gpState.Buttons.HasFlag(GamepadButtonFlags.DPadUp);
            Down = gpState.Buttons.HasFlag(GamepadButtonFlags.DPadDown);
        }

        #region Properties

        public bool A
        {
            get => _a; set
            {
                if (_a == value) return;
                _a = value;
                OnPropertyChanged("A");
            }
        }

        public bool B
        {
            get => _b; set
            {
                if (_b == value) return;
                _b = value;
                OnPropertyChanged("B");
            }
        }

        //public bool X
        //{
        //    get => _x; set
        //    {
        //        if (_x == value) return;
        //        _x = value;
        //        OnPropertyChanged("X");
        //    }
        //}

        //public bool Y
        //{
        //    get => _y; set
        //    {
        //        if (_y == value) return;
        //        _y = value;
        //        OnPropertyChanged("Y");
        //    }
        //}

        public bool Up
        {
            get => _up; set
            {
                if (_up == value) return;
                _up = value;
                OnPropertyChanged("Up");
            }
        }

        public bool Down
        {
            get => _down; set
            {
                if (_down == value) return;
                _down = value;
                OnPropertyChanged("Down");
            }
        }

        public bool Left
        {
            get => _left; set
            {
                if (_left == value) return;
                _left = value;
                OnPropertyChanged("Left");
            }
        }

        public bool Right
        {
            get => _right; set
            {
                if (_right == value) return;
                _right = value;
                OnPropertyChanged("Right");
            }
        }

        public bool LeftShoulder
        {
            get => _leftShoulder; set
            {
                if (_leftShoulder == value) return;
                _leftShoulder = value;
                OnPropertyChanged("LeftShoulder");
            }
        }

        public bool RightShoulder
        {
            get => _rightShoulder; set
            {
                if (_rightShoulder == value) return;
                _rightShoulder = value;
                OnPropertyChanged("RightShoulder");
            }
        }

        //public bool LeftStickPress
        //{
        //    get => _leftStickPress; set
        //    {
        //        if (_leftStickPress == value) return;
        //        _leftStickPress = value;
        //        OnPropertyChanged("LeftStickPress");
        //    }
        //}

        //public bool RightStickPress
        //{
        //    get => _rightStickPress; set
        //    {
        //        if (_rightStickPress == value) return;
        //        _rightStickPress = value;
        //        OnPropertyChanged("RightStickPress");
        //    }
        //}

        public bool Start
        {
            get => _start; set
            {
                if (_start == value) return;
                _start = value;
                OnPropertyChanged("Start");
            }
        }

        //public bool Back
        //{
        //    get => _back; set
        //    {
        //        if (_back == value) return;
        //        _back = value;
        //        OnPropertyChanged("Back");
        //    }
        //}

        //public bool LeftTrigger
        //{
        //    get => _leftTrigger; set
        //    {
        //        if (_leftTrigger == value) return;
        //        _leftTrigger = value;
        //        OnPropertyChanged("LeftTrigger");
        //    }
        //}

        //public bool RightTrigger
        //{
        //    get => _rightTrigger; set
        //    {
        //        if (_rightTrigger == value) return;
        //        _rightTrigger = value;
        //        OnPropertyChanged("RightTrigger");
        //    }
        //}

        public int XAxis
        {
            get => _xAxis;
            set
            {
                _xAxis = value;
                OnPropertyChanged("XAxis");
            }
        }

        public int YAxis
        {
            get => _yAxis;
            set
            {
                _yAxis = value;
                OnPropertyChanged("YAxis");
            }
        }

        #endregion Properties
    }
}