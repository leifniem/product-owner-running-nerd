namespace LoadRunnerClient.Messages
{
    public class StartGameMessage
    {
        public const string TYPE = "StartGameMessage";

        public StartGameMessage()
        {
        }

        private bool _dummy;
        public bool dummy { get => _dummy; set => _dummy = value; }
    }
}