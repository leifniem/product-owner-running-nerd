namespace LoadRunnerClient.Messages
{
    public class UserLoginMessage
    {
        public const string TYPE = "UserLoginMessage";

        /// Author Florian Ortmann
		public UserLoginMessage() { }

        public UserLoginMessage(string username)
        {
            this._username = username;
        }

        private string _username;

        public string username
        {
            get => _username;
            set => _username = value;
        }
    }
}