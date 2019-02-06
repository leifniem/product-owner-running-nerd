namespace LoadRunnerClient.Network.Messages
{
    public class ItemRemovedMessage
    {
        public const string TYPE = "ItemRemovedMessage";

        public readonly int posX;
        public readonly int posY;

        public ItemRemovedMessage(int posX, int posY)
        {
            this.posX = posX;
            this.posY = posY;
        }
    }
}
