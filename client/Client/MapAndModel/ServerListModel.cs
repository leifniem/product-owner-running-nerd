using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using LoadRunnerClient.Util;
using System;
using System.Collections.ObjectModel;

namespace LoadRunnerClient.MapAndModel
{
    /// <summary>
    /// Model to represent the ServerListView
    /// </summary>
    public class ServerListModel : ObservableModelBase
    {
		/// <summary>
		/// Session which is selected
		/// </summary>
		private SessionDTO _chosenSession;

        public SessionDTO chosenSession { get => _chosenSession; set => _chosenSession = value; }

        private ClientChannelHandler _clientChannelHandler;

        public ClientChannelHandler ClientChannelHandler
        {
            get => _clientChannelHandler;
            set => _clientChannelHandler = value;
        }

		/// <summary>
		/// Property determining if a Session was joined yet
		/// </summary>
        private bool _joinProperty = false;

        public bool joinProperty
        {
            get => _joinProperty;
            set
            {
//                if (_joinProperty != value)
//                {
                    _joinProperty = value;
                    OnPropertyChanged("joinProperty");
  //              }
            }
        }

        /// <summary>
		/// List of GameSessions
		/// </summary>
        private ObservableCollection<SessionDTO> _gamesessions = new ObservableCollection<SessionDTO>();
        public ObservableCollection<SessionDTO> Gamesession { get => _gamesessions; set => _gamesessions = value; }

		/// <summary>
		/// List of EditorSessions
		/// </summary>
        private ObservableCollection<SessionDTO> _editorsessions = new ObservableCollection<SessionDTO>();
		public EventHandler<ErrorMessageEventArgs> denyEvent;

		public ObservableCollection<SessionDTO> Editorsession { get => _editorsessions; set => _editorsessions = value; }

        public ServerListModel()
        {
            this.ClientChannelHandler = ClientChannelHandler.getInstance();
        }

        #region Listener

        public void AddListener() => ClientChannelHandler.clientSessionsChannel.OnMessageReceived += this.listener;

        public void RemoveListener() => ClientChannelHandler.clientSessionsChannel.OnMessageReceived -= this.listener;

        #endregion Listener

        #region MessageHandling

		/// <summary>
		/// MessageHandler for the ServerList
		/// </summary>
		/// <param name="type"></param>
		/// <param name="msg"></param>
        public void listener(string type, string msg)
        {
            switch (type)
            {
                case SessionListMessage.TYPE:
                    ParseSessionListMessage(msg);
                    return;

                case AcceptJoinSessionMessage.TYPE:
                    ParseAcceptJoinSessionMessage(msg);
                    joinProperty = true;
                    return;

                case DenyJoinSessionMessage.TYPE:
					ParseDenyMessage(msg);
                    return;
            }
            return;
        }

		/// <summary>
		/// Parse and Execute a AcceptJoinSessionMessage
		/// </summary>
		/// <param name="msg">Message Contents</param>
        private void ParseAcceptJoinSessionMessage(string msg)
        {
            AcceptJoinSessionMessage acceptJoinSessionMessage = Serializer.Deserialize<AcceptJoinSessionMessage>(msg);
            Console.WriteLine(acceptJoinSessionMessage.color);
        }

		private void ParseDenyMessage(string msg)
		{
			DenyJoinSessionMessage message = Serializer.Deserialize<DenyJoinSessionMessage>(msg);
			ErrorMessageEventArgs args = new ErrorMessageEventArgs();
			args.message = message.reason;
			if (denyEvent != null) denyEvent(this, args);
		}


		/// <summary>
		/// Parse and translate a SessionListMessage into usable data
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseSessionListMessage(string msg)
        {
            SessionListMessage sessionListMessage;
            sessionListMessage = Serializer.Deserialize<SessionListMessage>(msg);
            Gamesession.Clear();
            Editorsession.Clear();
            foreach (SessionDTO session in sessionListMessage.listOfSessionDTOS)
            {
                if (session.gameSession && !Contains(Gamesession, session))
                {
                    Gamesession.Add(session);
                }
                else if (session.editorSession && !Contains(Editorsession, session))
                {
                    Editorsession.Add(session);
                }
            }
            Console.WriteLine("SessionListMessage received");
        }

		#endregion MessageHandling

		/// <summary>
		/// Contains method that compares the ids of the sessiondtos
		/// </summary>
		/// <param name="list">List of sessions</param>
		/// <param name="session">Session to look for</param>
		/// <returns></returns>
		private bool Contains(ObservableCollection<SessionDTO> list, SessionDTO session)
        {
            foreach (SessionDTO cur in list)
            {
                if (cur.id.Equals(session.id))
                {
                    return true;
                }
            }
            return false;
        }

		/// <summary>
		/// Method to join a session. Creates a new SessionChannel and sends a joinmessage
		/// </summary>
		/// <param name="session">Session to join</param>
		public void joinSession(SessionDTO session)
        {
            chosenSession = session;
            this.ClientChannelHandler.createSessionChannel(session);
            this.ClientChannelHandler.sendSessionJoinMessage(session, SessionRole.SESSION_GUEST);
        }

		/// <summary>
		/// Method to request a SessionList
		/// </summary>
		public void sendGetSessionListMessage()
        {
            this.ClientChannelHandler.sendGetSessionListMessage();
        }
    }
}