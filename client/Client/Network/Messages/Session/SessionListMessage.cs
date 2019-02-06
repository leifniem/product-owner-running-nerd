using LoadRunnerClient.DTOs;
using System.Collections.Generic;

namespace LoadRunnerClient.Messages
{
    public class SessionListMessage
    {
        public const string TYPE = "SessionListMessage";
        private List<SessionDTO> _listOfSessionDTOS;

        public SessionListMessage()
        {
        }

        public SessionListMessage(List<SessionDTO> sessions)
        {
            this._listOfSessionDTOS = sessions;
        }

        public List<SessionDTO> listOfSessionDTOS
        {
            get => _listOfSessionDTOS;
            set => _listOfSessionDTOS = value;
        }
    }
}