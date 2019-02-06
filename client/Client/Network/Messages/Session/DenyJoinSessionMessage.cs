namespace LoadRunnerClient.Messages
{
    public class DenyJoinSessionMessage
    {
        public const string TYPE = "DenyJoinSessionMessage";

        public DenyJoinSessionMessage()
        {
        }

        public DenyJoinSessionMessage(string reason)
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