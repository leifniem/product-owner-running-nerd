namespace LoadRunnerClient.Messages
{
    public class AcceptJoinSessionMessage
    {
        public const string TYPE = "AcceptJoinSessionMessage";

        public AcceptJoinSessionMessage()
        {
        }

        private string _color;

        public string color
        {
            get => _color;
            set => _color = value;
        }
    }
}