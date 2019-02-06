namespace LoadRunnerClient.Messages
{
    public class LifePointsUpdateMessage
    {
        public const string TYPE = "LifePointsUpdateMessage";

        public LifePointsUpdateMessage()
        {
        }

        /// <summary>
        /// LifePointsUpdateMessage Constructor
        /// </summary>
        /// <param name="uuid"></param>
        /// <param name="lifePoints"></param>
        public LifePointsUpdateMessage(string uuid, int lifePoints)
        {
            this._uuid = uuid;
            this._lifePoints = lifePoints;
        }

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        private int _lifePoints;

        public int lifePoints
        {
            get => _lifePoints;
            set => _lifePoints = value;
        }
    }
}