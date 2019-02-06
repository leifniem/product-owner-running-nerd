using LoadRunnerClient.DTOs;
using System.Collections.Generic;

namespace LoadRunnerClient.Messages
{
    public class SelectSectionMessage
    {
        public const string TYPE = "SelectSectionMessage";
        private int _section;
        public int section { get => _section; set => _section = value; }

        private Dictionary<int, List<string>> _sections;
        public Dictionary<int, List<string>> sections { get => _sections; set => _sections = value; }

        public SelectSectionMessage()
        {
        }

        public SelectSectionMessage(int sec)
        {
            this.section = sec;
        }
    }
}