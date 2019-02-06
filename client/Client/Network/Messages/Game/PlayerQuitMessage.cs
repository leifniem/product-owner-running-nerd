using Newtonsoft.Json;

namespace LoadRunnerClient.Messages
{
    /// Author Florian Ortmann
	public class PlayerQuitMessage
    {
        public const string TYPE = "PlayerQuitMessage";

        private string _uuid;
        private string _username;

        public PlayerQuitMessage()
        {
        }

        public PlayerQuitMessage(string uuid, string username)
        {
            this._uuid = uuid;
            this._username = username;
        }

        public string uuid
        {
            get => _uuid;
            set => _uuid = value;
        }

        public string username
        {
            get => _username;
            set => _username = value;
        }

        public string ToJSON()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}