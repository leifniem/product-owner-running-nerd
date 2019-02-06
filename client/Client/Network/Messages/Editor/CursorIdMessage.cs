namespace LoadRunnerClient.Network.Messages
{
    public class CursorIdMessage
    {
        public const string TYPE = "CursorIdMessage";

        private string _uuid;

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }
    }
}