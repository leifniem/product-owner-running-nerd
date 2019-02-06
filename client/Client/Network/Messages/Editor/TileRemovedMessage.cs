namespace LoadRunnerClient.Network.Messages
{
    public class TileRemovedMessage
    {
        public const string TYPE = "TileRemovedMessage";

        public readonly int posX;
        public readonly int posY;

        public TileRemovedMessage(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }
    }
}