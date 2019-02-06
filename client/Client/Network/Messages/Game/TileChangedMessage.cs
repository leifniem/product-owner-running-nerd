namespace LoadRunnerClient.Network.Messages.Game
{
    public class TileChangedMessage
    {
        public const string TYPE = "TileChangedMessage";

        public const string DESTROYED_STATE = "DESTROYED";
        public const string RESTORED_STATE = "RESTORED";

        public string state { get; set; }

        public int gridX { get; set; }
        public int gridY { get; set; }

        public TileChangedMessage()
        {
        }

        public TileChangedMessage(string state, int gridX, int gridY)
        {
            this.state = state;
            this.gridX = gridX;
            this.gridY = gridY;
        }
    }
}