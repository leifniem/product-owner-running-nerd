using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Network;
using System;
using LoadRunnerClient.Util;
using System.Collections.ObjectModel;
using static LoadRunnerClient.Network.ComChannel;

namespace LoadRunnerClient.MapAndModel.ViewModel
{
	/// <summary>
	/// Model linked to the CreateSession View
	/// </summary>
    public class CreateSessionModel : ObservableModelBase
    {
		public bool isNewMap;
		public event EventHandler<ErrorMessageEventArgs> denyEvent;

		/// <summary>
		/// Instance of the ClientChannelHandler used for Messaging
		/// </summary>
		private ClientChannelHandler _clientChannelHandler;
		public ClientChannelHandler ClientChannelHandler
		{
			get => _clientChannelHandler;
			set => _clientChannelHandler = value;
		}


		/// <summary>
		/// Session which will be created
		/// </summary>
		private SessionDTO _sessionDTO;
		public SessionDTO sessionDTO { get => _sessionDTO; set => _sessionDTO = value; }

		/// <summary>
		/// Collection of all available Maps
		/// </summary>
		private ObservableCollection<MapMetaDTO> _items = new ObservableCollection<MapMetaDTO>();
		public ObservableCollection<MapMetaDTO> Items { get => _items; set => _items = value; }


		/// <summary>
		/// Property determining if the Session is to be started
		/// </summary>
		private bool _startProperty = false;
        public bool startProperty
        {
            get => _startProperty;
            set
            {
                if (_startProperty != value)
                {
                    _startProperty = value;
                    OnPropertyChanged("startProperty");
                }
            }
        }

		/// <summary>
		/// Constructor of the CreateSessionModel
		/// </summary>
		public CreateSessionModel()
        {
            this.sessionDTO = new SessionDTO();
            this.ClientChannelHandler = ClientChannelHandler.getInstance();
        }


		#region ClientChannelHandler

		/// <summary>
		/// Adds a listener to the channel that listens to incoming messages
		/// </summary>
		public void AddListener() => ClientChannelHandler.clientSessionsChannel.OnMessageReceived += listener;

		/// <summary>
		/// Creates a sessionChannel to connect to the session
		/// </summary>
		public void createSessionChannel()
        {
            this.ClientChannelHandler.createSessionChannel(sessionDTO);
        }

		/// <summary>
		/// Removes the listener from the channel
		/// </summary>
		public void RemoveListener() => ClientChannelHandler.clientSessionsChannel.OnMessageReceived -= listener;

		/// <summary>
		/// Sends a createsessionMessage with the sessionDTO that was created
		/// </summary>
		public void sendCreateSessionMessage()
        {
            if (isNewMap && sessionDTO.editorSession)
            {
                this.sendCreateSessionWithNewMap();
            }
            else
            {
                this.ClientChannelHandler.sendCreateSessionMessage(sessionDTO);
            }
        }

		/// <summary>
		/// Requests the List of Maps available
		/// </summary>
        public void sendMapListMessage()
        {
            this.ClientChannelHandler.sendMapListMessage();
        }
        #endregion ClientChannelHandler

        #region Messagehandling
		/// <summary>
		/// Method Handling different Messages that will be received when in this view
		/// </summary>
		/// <param name="type">Type of the Message</param>
		/// <param name="msg">Message Contents</param>
        public void listener(string type, string msg)
        {
            switch (type)
            {
                case MapListMessage.TYPE:
                    ParseMapListMessage(msg);
                    return;

                case AcceptCreateSessionMessage.TYPE:
                    ParseAcceptCreateSessionMessage(msg);
                    return;

                case DenyCreateSessionMessage.TYPE:
                    ParseDenyCreateSessionMessage(msg);
                    return;
            }
            return;
        }

		/// <summary>
		/// Method to parse an accepted Session creation and starting it
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseAcceptCreateSessionMessage(string msg)
		{
			AcceptCreateSessionMessage accept = Serializer.Deserialize<AcceptCreateSessionMessage>(msg);
			sessionDTO.id = accept.sessionID;
			startProperty = true;
		}

		/// <summary>
		/// Method dedicated to handling denied requests of creating a Session
		/// </summary>
		/// <param name="msg">Message Contents</param>
		private void ParseDenyCreateSessionMessage(string msg)
		{
            DenyCreateSessionMessage message = Serializer.Deserialize<DenyCreateSessionMessage>(msg);
			ErrorMessageEventArgs args = new ErrorMessageEventArgs();
			args.message = message.reason;
			if (denyEvent != null)
				denyEvent(this, args);
		}

		/// <summary>
		/// Method to parse the Map List into a reusable Collection
		/// </summary>
		/// <param name="msg">Message Contents</param>
        private void ParseMapListMessage(string msg)
        {
            MapListMessage mapListMessage = Serializer.Deserialize<MapListMessage>(msg);
            System.Diagnostics.Debug.WriteLine(mapListMessage.listOfMaps);
            mapListMessage.listOfMaps.ForEach(map => Items.Add(map));
        }

        #endregion Messagehandling

		/// <summary>
		/// Method causing the ClientChannelHandler to rquest the creation of a Map
		/// </summary>
        public void sendCreateSessionWithNewMap()
        {
            this.ClientChannelHandler.sendCreateMapMessage(sessionDTO);
        }
    }
}