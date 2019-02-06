using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Messages
{
    public class CreateSessionMessage
    {
        public const string TYPE = "CreateSessionMessage";
        private SessionDTO _sessionDTO;
        public SessionDTO sessionDTO { get => _sessionDTO; set => _sessionDTO = value; }

        public CreateSessionMessage()
        {
        }

        public CreateSessionMessage(SessionDTO sessionDTO)
        {
            this.sessionDTO = sessionDTO;
        }
    }
}