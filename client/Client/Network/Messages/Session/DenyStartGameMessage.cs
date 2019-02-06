namespace LoadRunnerClient.Messages
{
    public class DenyStartGameMessage
    {
        public const string TYPE = "DenyStartGameMessage";
        private string _reason;

        public string reason
        {
            get => _reason;
            set => _reason = value;
        }

        public DenyStartGameMessage()
        {
        }

        public DenyStartGameMessage(string reason)
        {
            this.reason = reason;
        }
    }
}