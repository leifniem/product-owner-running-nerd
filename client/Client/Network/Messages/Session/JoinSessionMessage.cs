namespace LoadRunnerClient.Messages
{
    public enum SessionRole
    {
        SESSION_OWNER, SESSION_GUEST, SPECTATOR
    }

    public class JoinSessionMessage
    {
        public const string TYPE = "JoinSessionMessage";

        public JoinSessionMessage()
        {
        }

        public JoinSessionMessage(string sessionID, SessionRole sessionRole)
        {
            this._sessionID = sessionID;
            this._sessionRole = sessionRole;
        }

        private string _sessionID;

        public string sessionID
        {
            get => _sessionID;
            set => _sessionID = value;
        }

        private SessionRole _sessionRole;

        public SessionRole sessionRole
        {
            get => _sessionRole;
            set => _sessionRole = value;
        }
    }
}