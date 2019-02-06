using System;

namespace LoadRunnerClient.Messages
{
    public class AcceptCreateSessionMessage
    {
        public const string TYPE = "AcceptCreateSessionMessage";

        public AcceptCreateSessionMessage()
        {
        }

        public AcceptCreateSessionMessage(String sessionID)
        {
            this._sessionID = sessionID;
        }

        private string _sessionID;

        public string sessionID
        {
            get => _sessionID;
            set => _sessionID = value;
        }

        private string _color;

        public string color
        {
            get => _color;
            set => _color = value;
        }
    }
}