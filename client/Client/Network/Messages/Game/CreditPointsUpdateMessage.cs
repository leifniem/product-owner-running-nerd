namespace LoadRunnerClient.Network.Messages
{
    public class CreditPointsUpdateMessage
    {
        public const string TYPE = "CreditPointsUpdateMessage";

        public CreditPointsUpdateMessage()
        {
        }

        /// <summary>
        /// CreditPointsUpdateMessage Constructor
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="creditPoints"></param>
        public CreditPointsUpdateMessage(string uuid, int creditPoints)
        {
            this._uuid = uuid;
            this._creditPoints = creditPoints;
        }

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private int _creditPoints;

        public int creditPoints
        {
            get => _creditPoints;
            set => _creditPoints = value;
        }
    }
}