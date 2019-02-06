namespace LoadRunnerClient
{
    /// <summary>
    /// Basic Vector2 class 
    /// </summary>
    public class Vector2
    {
        private float _x;
        private float _y;

        public Vector2(float x, float y)
        {
            this._x = x;
            this._y = y;
        }

        public Vector2()
        {
        }

        public float X
        {
            get => _x;
            set => _x = value;
        }

        public float Y
        {
            get => _y;
            set => _y = value;
        }

        public override string ToString()
        {
            return _x + " " + _y;
        }
    }
}