namespace LoadRunnerClient.DTOs
{
	/// <summary>
	/// Equivalent of Server-Side DTO to pass information about Cursors in Editor
	/// </summary>
    public class CursorDTO
    {
        private int _gridX;
        private int _gridY;
        private string _uuid;
        private string _color;
		/// <summary>
		/// CursorDTO Constructor
		/// </summary>
		/// <param name="x">X-Position</param>
		/// <param name="y">Y-Position</param>
        public CursorDTO(int x, int y)
        {
            this._gridX = x;
            this._gridY = y;
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

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        public string color
        {
            get => _color;
            set => _color = value;
        }
    }
}