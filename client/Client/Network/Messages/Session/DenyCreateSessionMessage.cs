namespace LoadRunnerClient.Messages
{
    public class DenyCreateSessionMessage
    {
        public const string TYPE = "DenyCreateSessionMessage";

        public DenyCreateSessionMessage()
        {
        }

        public DenyCreateSessionMessage(string reason)
        {
            _reason = reason;
        }

        private string _reason;

        public string reason
        {
            get => _reason;
            set => _reason = value;
        }
    }
}