namespace LoadRunnerClient.Network.Messages
{
    public class ItemUsedMessage
    {
        public const string TYPE = "ItemUsedMessage";

        public ItemUsedMessage()
        {
        }

        /// <summary>
        /// ItemUsedMessage Constructor
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="type"></param>
        public ItemUsedMessage(string uuid, string type)
        {
            this._uuid = uuid;
            this._type = type;
        }

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private string _type;

        public string type
        {
            get => _type;
            set => _type = value;
        }
    }
}