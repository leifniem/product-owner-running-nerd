using System;
using System.Collections.Generic;

namespace LoadRunnerClient.Messages
{
    public class TickUpdateMessage
    {
        public const string TYPE = "TickUpdateMessage";

        private Dictionary<string, List<Object>> _changes;

        public TickUpdateMessage(Dictionary<string, List<object>> changes)
        {
            this._changes = changes;
        }

        public Dictionary<string, List<Object>> changes
        {
            get => _changes;
            set => _changes = value;
        }
    }
}