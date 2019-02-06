namespace LoadRunnerClient.Network.Messages
{
    public class AddSectionMessage
    {
        public const string TYPE = "AddSectionMessage";

        private int _sections;

        public int sections { get => _sections; set => _sections = value; }
    }
}