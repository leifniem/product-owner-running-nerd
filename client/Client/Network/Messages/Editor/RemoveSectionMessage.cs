namespace LoadRunnerClient.Network.Messages
{
    public class RemoveSectionMessage
    {
        public const string TYPE = "RemoveSectionMessage";

        private int _sections;

        public int sections { get => _sections; set => _sections = value; }

    }
}