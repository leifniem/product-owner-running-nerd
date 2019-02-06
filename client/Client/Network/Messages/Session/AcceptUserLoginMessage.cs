namespace LoadRunnerClient.Messages
{
    public class AcceptUserLoginMessage
    {
        public const string TYPE = "AcceptUserLoginMessage";

        public AcceptUserLoginMessage()
        {
        }

        private bool _dummy;
        public bool dummy { get => _dummy; set => _dummy = value; }
    }
}