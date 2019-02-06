namespace LoadRunnerClient.Messages
{
    public class DenyUserLoginMessage
    {
        public const string TYPE = "DenyUserLoginMessage";

        public DenyUserLoginMessage()
        {
        }

        public DenyUserLoginMessage(string reason)
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