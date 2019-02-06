namespace LoadRunnerClient.Messages
{
    public class DenySelectSectionMessage
    {
        public const string TYPE = "DenySelectSectionMessage";
        private string _reason;

        public string reason
        {
            get => _reason;
            set => _reason = value;
        }

        public DenySelectSectionMessage()
        {
        }

        public DenySelectSectionMessage(string reason)
        {
            this.reason = reason;
        }
    }
}