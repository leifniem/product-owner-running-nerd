using LoadRunnerClient.DTOs;
using System.Collections.Generic;

namespace LoadRunnerClient.Network.Messages
{
    public class EditorSessionJoinedMessage
    {
        public const string TYPE = "EditorSessionJoinedMessage";
        private List<CursorDTO> _cursor;

        public List<CursorDTO> cursor
        {
            get => _cursor;
            set => _cursor = value;
        }
    }
}