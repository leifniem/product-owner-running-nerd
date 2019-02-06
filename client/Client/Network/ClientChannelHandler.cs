using LoadRunnerClient.DTOs;
using LoadRunnerClient.Messages;
using LoadRunnerClient.Network.Messages;
using LoadRunnerClient.Network.Messages.Session;
using System;

namespace LoadRunnerClient.Network
{
	/// <summary>
	/// Author : Florian Ortmann
	/// Manages every Channel the client sends and receives messages from
	/// </summary>
	public class ClientChannelHandler
	{
		private ComChannel _clientUserChannel;
		private ComChannel _clientSessionsChannel;
		private ComChannel _sessionChannel;
		private static ClientChannelHandler instance;
		//private string _clientChannelName;

		public ComChannel clientUserChannel
		{
			get => _clientUserChannel;
			set => _clientUserChannel = value;
		}

		/// <summary>
		/// ClientSessionChannel to receive messages from the serversession
		/// </summary>
		public ComChannel clientSessionsChannel
		{
			get => _clientSessionsChannel;
			set => _clientSessionsChannel = value;
		}

        /// <summary>
        /// SessionChannel which receives messages from the session
        /// </summary>
        public ComChannel sessionChannel
        {
            get => _sessionChannel;
            set => _sessionChannel = value;
        }

		private string _username;

		public string username
		{
			get => _username;
			set => _username = value;
		}

		#region Constructor

        /// <summary>
        /// GetInstance Method of the ClientChannelhandler
        /// </summary>
        /// <returns>An Instance of the ClientChannelHandler</returns>
        public static ClientChannelHandler getInstance()
        {
            if (instance == null)
            {
                instance = new ClientChannelHandler();
            }
            return instance;
        }

		private ClientChannelHandler()
		{
			
		}

		public void createDefaultChannels()
		{
			clientSessionsChannel = NetworkService.CreateComChannel("Server.Sessions");
			clientUserChannel = NetworkService.CreateComChannel("Server.Users");
		}

		#endregion Constructor

        /// <summary>
        /// Creates a session Channel with the id of the session
        /// </summary>
        /// <param name="session"></param>
        public void createSessionChannel(SessionDTO session)
        {
            this.sessionChannel = NetworkService.CreateComChannel(session.id);
        }

		#region sendMessages

		/// <summary>
		/// sends a sessionjoinmessage to the serverlobbychannel to join a session
		/// </summary>
		/// <param name="session"> session to join</param>
		/// <param name="sessionRole">sessionrole</param>
		public void sendSessionJoinMessage(SessionDTO sessionDTO, SessionRole role)
		{
			//JoinSessionMessage joinSessionMessage = new JoinSessionMessage(username, session, sessionRole);
			//ServerLobbySenderChannel.SendMessage(JoinSessionMessage.TYPE,
			//    Serializer.SerializeJoinSessionMessage(joinSessionMessage));
			JoinSessionMessage joinSessionMessage = new JoinSessionMessage(sessionDTO.id, role);
			System.Diagnostics.Debug.WriteLine("JSM:" + sessionDTO.id);
			clientSessionsChannel.SendMessage(JoinSessionMessage.TYPE,
				Serializer.Serialize(joinSessionMessage));
		}

		/// <summary>
		/// sends a userconnectedmessage to the serverlobbychannel
		/// </summary>
		public void sendUserLoginMessage()
		{
			clientUserChannel.SendMessage(UserLoginMessage.TYPE,
				Serializer.Serialize(new UserLoginMessage(username)));
		}

        /// <summary>
        /// Sends a deselectsectionmessage
        /// </summary>
        /// <param name="section"> Section which is deselected </param>
        public void sendDeselectSectionMessage(int section)
        {
            sessionChannel.SendMessage(DeselectSectionMessage.TYPE,
                Serializer.Serialize(new DeselectSectionMessage(section)));
        }
        /// <summary>
        /// Sends a playerCommandMessage
        /// </summary>
        /// <param name="key"> Key which is pressed </param>
        public void sendPlayerCommandMessage(PressedKey key)
        {
            PlayerCommandMessage playerCommandMessage = new PlayerCommandMessage(key);
            sessionChannel.SendMessage(PlayerCommandMessage.TYPE,
                Serializer.Serialize(playerCommandMessage));
        }

        /// <summary>
        /// Sends a create map message wth a given session
        /// </summary>
        /// <param name="session"></param>
        public void sendCreateMapMessage(SessionDTO session)
        {
            clientSessionsChannel.SendMessage(CreateMapMessage.TYPE, Serializer.Serialize(new CreateMapMessage(session)));
        }

		/// <summary>
		/// sends a createsessionmessage to the serverlobby
		/// </summary>
		/// <param name="sessionDTO"></param>
		public void sendCreateSessionMessage(SessionDTO sessionDTO)
		{
			CreateSessionMessage createSessionMessage = new CreateSessionMessage(sessionDTO);
			clientSessionsChannel.SendMessage(CreateSessionMessage.TYPE,
				Serializer.Serialize(createSessionMessage));
		}

		/// <summary>
		/// sends a empty MapListMessage to the server to request the maplist
		/// </summary>
		public void sendMapListMessage()
		{
			MapListMessage mapListMessage = new MapListMessage();
			clientSessionsChannel.SendMessage(MapListMessage.TYPE,
				Serializer.Serialize(mapListMessage));
		}



        /// <summary>
        /// Sends a tileremovedmessage
        /// </summary>
        /// <param name="x"> Pos X of the tile</param>
        /// <param name="y"> Pos Y of the tile</param>
        public void sendTileRemovedMessage(int x, int y)
        {
            TileRemovedMessage trm = new TileRemovedMessage(x, y);
            sessionChannel.SendMessage(TileRemovedMessage.TYPE, message: Serializer.Serialize(trm));
        }

        /// <summary>
        /// Sends a itemremovedmessage
        /// </summary>
        /// <param name="x">Pos X of the item</param>
        /// <param name="y">Pos Y of the item</param>
        public void sendItemRemovedMessage(int x, int y)
        {
            ItemRemovedMessage irm = new ItemRemovedMessage(x, y);
            sessionChannel.SendMessage(ItemRemovedMessage.TYPE, message: Serializer.Serialize(irm));
        }

        /// <summary>
        /// Sends a cursormovemessage
        /// </summary>
        /// <param name="x">Pos X of the cursor</param>
        /// <param name="y">Pos Y of the cursor</param>
        public void sendCursorMoveMessage(int x, int y)
        {
            CursorMovedMessage cmm = new CursorMovedMessage(new CursorDTO(x, y));
            sessionChannel.SendMessage(CursorMovedMessage.TYPE, Serializer.Serialize(cmm));
        }

        /// <summary>
        /// Sends a selectsectionmessage
        /// </summary>
        /// <param name="section"> Section which is selected</param>
        public void sendSelectSectionMessage(int section)
        {
            SelectSectionMessage ssm = new SelectSectionMessage(section);
            sessionChannel.SendMessage(SelectSectionMessage.TYPE,
                Serializer.Serialize(ssm));
        }

        /// <summary>
        /// Sends a startgamemessage
        /// </summary>
        public void sendStartGameMessage()
        {
            StartGameMessage sgm = new StartGameMessage();
            sessionChannel.SendMessage(StartGameMessage.TYPE,
                Serializer.Serialize(sgm));
        }

        /// <summary>
        /// Sends a getsessionlistmessage
        /// </summary>
        public void sendGetSessionListMessage()
        {
            GetSessionListMessage sessionListMessage = new GetSessionListMessage();
            clientSessionsChannel.SendMessage(GetSessionListMessage.TYPE,
                Serializer.Serialize(sessionListMessage));
        }

        /// <summary>
        /// Sends a tileplacedmessage
        /// </summary>
        /// <param name="tileDTO"> Tile as DTO which is placed </param>
        public void sendTilePlacedMessage(TileDTO tileDTO)
        {
            sessionChannel.SendMessage(TilePlacedMessage.TYPE,
                Serializer.Serialize(new TilePlacedMessage(tileDTO)));
        }

        /// <summary>
        /// Sends a itemplacedmessage
        /// </summary>
        /// <param name="itemDTO"> Item as DTO which is placed</param>
        public void sendItemPlacedMessage(ItemDTO itemDTO)
        {
            sessionChannel.SendMessage(ItemPlacedMessage.TYPE,
                Serializer.Serialize(new ItemPlacedMessage(itemDTO)));
        }

        /// <summary>
        /// Sends a addsectionmessage
        /// </summary>
        public void sendAddSectionMessage()
        {
            sessionChannel.SendMessage(AddSectionMessage.TYPE, Serializer.Serialize(new AddSectionMessage()));
        }

        /// <summary>
        /// Sends a removesectionmessage
        /// </summary>
        public void sendRemoveSectionMessage()
        {
            sessionChannel.SendMessage(RemoveSectionMessage.TYPE, Serializer.Serialize(new RemoveSectionMessage()));
        }

        /// <summary>
        /// Sends a quiteditormessage
        /// </summary>
        public void sendQuitEditorMessage()
        {
            sessionChannel.SendMessage(PlayerQuitMessage.TYPE, Serializer.Serialize(new PlayerQuitMessage()));
        }

		public void SendEnemySpawnPointMessage(EnemySpawnPointDTO enemySpawnDTO)
		{
			EnemySpawnPointPlacedMessage esppm = new EnemySpawnPointPlacedMessage(enemySpawnDTO);
			sessionChannel.SendMessage(EnemySpawnPointPlacedMessage.TYPE, Serializer.Serialize(esppm));
		}

		public void SendEnemySpawnPointUnlockMessage(int gridX, int gridY)
		{
			EnemySpawnPointUnlockMessage espulm = new EnemySpawnPointUnlockMessage(gridX, gridY);
			sessionChannel.SendMessage(EnemySpawnPointUnlockMessage.TYPE, Serializer.Serialize(espulm));
		}

		public void SendEnemySpawnPointRequestLockMessage(int gridX, int gridY)
		{
			EnemySpawnPointRequestLockMessage lockrequest = new EnemySpawnPointRequestLockMessage(gridX, gridY);
			sessionChannel.SendMessage(EnemySpawnPointRequestLockMessage.TYPE, Serializer.Serialize(lockrequest));
		}
	
		/// <summary>
		/// Sends a PlayerQuitMessage
		/// </summary>
		public void sendPlayerQuitMessage()
		{
			sessionChannel.SendMessage(PlayerQuitMessage.TYPE, Serializer.Serialize(new PlayerQuitMessage()));
		}

		/// <summary>
		/// Sends a PlayerReadyMessage
		/// </summary>
		public void sendPlayerReadyMessage()
		{
			sessionChannel.SendMessage(PlayerReadyMessage.TYPE, Serializer.Serialize(new PlayerReadyMessage()));
		}

        public void sendReplaceSpawnPointMessage(SpawnPointReplaceMessage message)
        {
            sessionChannel.SendMessage(SpawnPointReplaceMessage.TYPE, Serializer.Serialize(message));
        }
            #endregion sendMessages
        }
}