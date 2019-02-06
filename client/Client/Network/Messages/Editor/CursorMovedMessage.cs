using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Network.Messages
{
    public class CursorMovedMessage
    {
        public const string TYPE = "CursorMovedMessage";
        private CursorDTO _cursor;

        public CursorMovedMessage(CursorDTO cdto)
        {
            _cursor = cdto;
        }

        public CursorDTO cursor
        {
            get => _cursor;
            set => _cursor = value;
        }
    }
}