namespace LoadRunnerClient.Messages
{
    public class DeselectSectionMessage
    {
        public const string TYPE = "DeselectSectionMessage";
        private int _section;
        public int section { get => _section; set => _section = value; }

        public DeselectSectionMessage()
        {
        }

        public DeselectSectionMessage(int sec)
        {
            this.section = sec;
        }
    }
}