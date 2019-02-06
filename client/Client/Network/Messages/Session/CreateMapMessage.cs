using LoadRunnerClient.DTOs;

namespace LoadRunnerClient.Network.Messages.Session
{
    public class CreateMapMessage
    {
        public const string TYPE = "CreateMapMessage";
        private SessionDTO _sessionDTO;
        public SessionDTO sessionDTO { get => _sessionDTO; set => _sessionDTO = value; }

        public CreateMapMessage()
        {
        }

        public CreateMapMessage(SessionDTO sessionDTO)
        {
            this.sessionDTO = sessionDTO;
        }
    }
}