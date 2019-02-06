using Apache.NMS;
using Apache.NMS.ActiveMQ;
using Apache.NMS.ActiveMQ.Commands;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LoadRunnerClient.Network
{
	/// <summary>
	/// This class provides static access to all networking features
	/// </summary>
    public class NetworkService
    {
        public static string CONNECTION_TOPIC = "Server";
        public static int KEEPALIVE_MESSAGE_INTERVAL = 1000;

        private const string CONNECTION_EVENT_PROPERTY = "ConnectionEvent";
        private const string CONNECT = "Connect";
        private const string DISCONNECT = "Disconnect";
        private const string KEEPALIVE = "Keepalive";

        private static IConnection connection = null;
        private static ISession mainSession = null;

        private static ITemporaryQueue localTempQueue = null;
        private static IMessageConsumer serverMessageConsumer = null;

        private static ITopic connectionTopic = null;
        private static IMessageProducer connectionMessageProducer = null;

        private static Dictionary<string, ComChannel> allComChannels = null;

		/// <summary>
		/// The name of the clients automatically generated active MQ tempQueue servers as a unique clientID
		/// </summary>
        public static string ClientID
        {
            get; private set;
        }

		/// <summary>
		/// True if the NetworkService is connected to a server
		/// </summary>
		public static bool IsConnected 
		{
			get {
				if (connection == null)
					return false;
				if (!connection.IsStarted)
					return false;
				return true;
			}
		}

		/// <summary>
		/// Connect to a server and start sending Keepalive messages
		/// on an internally used connection event topic.
		/// </summary>
		/// <param name="url">The server adress</param>
		public static void Connect(string url) {
            if (IsConnected) {
                return;
            }

            IConnectionFactory connectionFactory = new ConnectionFactory(url);
            connection = connectionFactory.CreateConnection();
            connection.Start();

            mainSession = connection.CreateSession(AcknowledgementMode.AutoAcknowledge);

            localTempQueue = mainSession.CreateTemporaryQueue();
            serverMessageConsumer = mainSession.CreateConsumer(localTempQueue);
            serverMessageConsumer.Listener += DispatchMessageToChannel;
            ClientID = localTempQueue.ToString();

            connectionTopic = new ActiveMQTopic(CONNECTION_TOPIC);
            connectionMessageProducer = mainSession.CreateProducer(connectionTopic);

            allComChannels = new Dictionary<string, ComChannel>();

            SendConnectionEventMessage(CONNECT);
            StartKeepaliveMessageThread();

            if (!IsConnected)
            {
                Console.Error.WriteLine("[NetworkService] failed to connect.");
                Close();
            }
        }

		/// <summary>
		/// Creates a ComChannel with the specified channel name/ID. Connect() has to be called first.
		/// </summary>
		/// <param name="channelID">The name/ID of the ComChannel</param>
		/// <returns></returns>
		public static ComChannel CreateComChannel(string channelID)
        {
            if (!IsConnected)
            {
                Console.Error.WriteLine("[NetworkService] not connected");
                return null;
            }

			lock (allComChannels) {
				if (allComChannels.ContainsKey(channelID)) {
					Console.Error.WriteLine("[NetworkService] duplicate channelID: " + channelID);
					return null;
				}

				ComChannel comChannel = new ComChannel(connection, channelID);
				allComChannels.Add(channelID, comChannel);

				return comChannel;
			}			
        }

		/// <summary>
		/// Sends a disconnect event message to the server, disconnects and closes all network resources.
		/// </summary>
        public static void Disconnect()
        {			
            if (connectionMessageProducer != null)
                SendConnectionEventMessage(DISCONNECT);
            Close();
        }

		/// <summary>
		/// Disconnect and close all network resources without sending a disconnect event message.
		/// </summary>
        public static void Close()
        {
			//lock (allComChannels) {
			//	if (allComChannels != null) {
			//		foreach (ComChannel channel in allComChannels.Values) {
			//			Console.WriteLine(channel.channelID);
			//			channel._CloseInternal();
			//		}
			//		allComChannels = null;
			//	}
			//}

			if (connectionMessageProducer != null) {
                connectionMessageProducer.Close();
                connectionMessageProducer = null;
            }        

			if (serverMessageConsumer != null) {
                serverMessageConsumer.Close();
                serverMessageConsumer = null;
            }

			if (localTempQueue != null) {
				localTempQueue.Dispose();
				localTempQueue = null;
			}

			if (connectionTopic != null) {
				connectionTopic.Dispose();
				connectionTopic = null;
			}

			if (mainSession != null) {
                mainSession.Close();
                mainSession = null;
            }

			if (connection != null) {
                connection.Close();
                connection = null;
            }		
        }

		/// <summary>
		/// Used internally. Removes a ComChannel from the ComChannel managment, after calling ComChannel.Close().
		/// </summary>
		/// <param name="channelID">The name/ID of the ComChannel</param>
		internal static void _RemoveChannel(string channelID)
        {
			lock (allComChannels) {
				if (allComChannels.ContainsKey(channelID)) {
					allComChannels.Remove(channelID);
				}
			}
        }

		/// <summary>
		/// Used internally. Direct 1-1 Messages from the server are sent via a tempQueue. 
		/// They are dispatched to their corresponding ComChannel, where they are handled just like 1-n messages.
		/// </summary>
		/// <param name="amqMessage">The received direct 1-1 message</param>
        private static void DispatchMessageToChannel(IMessage amqMessage)
        {
            string channelID = null;
            channelID = amqMessage.Properties.GetString(ComChannel.CHANNELID_PROPERTY);

            if (channelID == null)
            {
                Console.Error.WriteLine("[NetworkService] Received server message without channelID");
                return;
            }
            else if (channelID.Equals(string.Empty))
            {
                Console.Error.WriteLine("[NetworkService] Received server message without channelID");
                return;
            }

            if (!allComChannels.ContainsKey(channelID))
            {
                Console.Error.WriteLine("[NetworkService] Received server message for unknown channelID: " + channelID);
                return;
            }

            ComChannel channel = allComChannels[channelID];
            channel._OnMessage(amqMessage);
        }

		/// <summary>
		/// Starts a thread to send Keepalive messages in a loop
		/// </summary>
        private static void StartKeepaliveMessageThread()
        {
            Thread keepaliveThread = new Thread(AsyncKeepaliveMessageLoop);
            keepaliveThread.Start();
        }

		/// <summary>
		/// Sends Keepalive messsages to the server in regular intervals. 
		/// Send interval length can be set with KEEPALIVE_MESSAGE_INTERVAL in milliseconds.
		/// </summary>
		private static void AsyncKeepaliveMessageLoop()
        {
            while (true)
            {
                Thread.Sleep(KEEPALIVE_MESSAGE_INTERVAL);
                if (!IsConnected)
                    return;

                SendConnectionEventMessage(KEEPALIVE);
            }
        }

		/// <summary>
		/// Sends a connection event message on a designated internal message topic.
		/// </summary>
		/// <param name="eventType">The type of the connection event</param>
        private static void SendConnectionEventMessage(String eventType)
        {
            IMessage amqMessage = mainSession.CreateMessage();
            amqMessage.NMSReplyTo = localTempQueue;
            amqMessage.Properties.SetString(CONNECTION_EVENT_PROPERTY, eventType);
            connectionMessageProducer.Send(amqMessage);
        }
    }
}