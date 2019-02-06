using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using System;

namespace LoadRunnerClient.MapAndModel
{
	/// <summary>
	/// Model Handling actiosn related to the Login Process
	/// </summary>
    public class LoginModel : ObservableModelBase
    {
        private ClientChannelHandler _clientChannelHandler;

        public ClientChannelHandler ClientChannelHandler
        {
            get => _clientChannelHandler;
            set => _clientChannelHandler = value;
        }

        private bool _loggedIn = false;

        public bool loggedIn
        {
            get => _loggedIn;
            set
            {
                if (_loggedIn != value)
                {
                    _loggedIn = value;
                    OnPropertyChanged("loggedIn");
                }
            }
        }

		private string _serverAddress;
		public string serverAddress
		{
			get => _serverAddress;
			set
			{
				if (_serverAddress != value)
				{
					_serverAddress = value;					
					OnPropertyChanged("serverAddress");
				}
			}
		}

		private string _username;
		public string username
		{
			get => _username;
			set
			{
				if (_username != value)
				{
					_username = value;
					OnPropertyChanged("username");
				}
			}
		}

		public LoginModel()
        {
			serverAddress = "localhost:61616";
        }

		public void Login()
		{
			if (username == null)
				return;
			if (username.Equals(string.Empty))
				return;

			Console.WriteLine(username);

			try
			{
				if (!NetworkService.IsConnected)
				{
					NetworkService.Connect("tcp://" + this.serverAddress);
					ClientChannelHandler = ClientChannelHandler.getInstance();
					ClientChannelHandler.createDefaultChannels();
					AddListener();
				}

				this.ClientChannelHandler.username = this.username;
				this.ClientChannelHandler.sendUserLoginMessage();
			}
			catch
			{
				// TODO: trigger connection error message box in view
			}			
		}

        #region Listener

        public void AddListener()
        {
            ClientChannelHandler.clientUserChannel.OnMessageReceived += listener;
        }

        public void RemoveListener()
        {
            ClientChannelHandler.clientUserChannel.OnMessageReceived -= listener;
        }

        #endregion Listener

        private void listener(string type, string message)
        {
            switch (type)
            {
                case AcceptUserLoginMessage.TYPE:
                    RemoveListener();
                    loggedIn = true;
                    return;

                case DenyUserLoginMessage.TYPE:
                    return;
            }
        }
    }
}