using Apache.NMS;
using Apache.NMS.ActiveMQ.Commands;
using System;

namespace LoadRunnerClient.Network
{

	/// <summary>
	/// A wrapper class, that encapsulates multiple active MQ topics. 
	/// One to receive 1-n messages from the server and one to send n-1 messages to the server. 
	/// This way, each ComChannel provides two-way communication without the need to filter incomming messages. 
	/// Direct 1-1 messages from the server are received by the NetworkService-Class on an internally used 
	/// tempQueue and automatically dispatched to their corresponding ComChannel, 
	/// where they can be handled just like 1-n messages that were sent to all clients.
	/// </summary>
	public class ComChannel
    {
        internal const string INPUT_TOPIC = ".ServerOutput";
        internal const string OUTPUT_TOPIC = ".ServerInput";

        internal const string CLIENTID_PROPERTY = "Username";
        internal const string CHANNELID_PROPERTY = "ChannelID";
        internal const string MESSAGETYPE_PROPERTY = "MessageType";

        private string channelID;
        private ISession Session;
        private ITopic InputTopic;
        private ITopic OutputTopic;

        private IMessageProducer Producer;
        private IMessageConsumer Consumer;

        public delegate void MessageListener(string messageType, string message);

        public event MessageListener OnMessageReceived;

		/// <summary>
		/// Do not use this constructor. Use NetworkService.CreateComChannel() instead. 		
		/// </summary>
		/// <param name="connection">The active MQ connection (handled in NetworkService)</param>
		/// <param name="channelID">The name/ID of the channel to be created</param>
        internal ComChannel(IConnection connection, String channelID)
        {
            this.channelID = channelID;
            this.Session = connection.CreateSession();

            this.InputTopic = new ActiveMQTopic(channelID + INPUT_TOPIC);
            this.OutputTopic = new ActiveMQTopic(channelID + OUTPUT_TOPIC);

            this.Consumer = this.Session.CreateConsumer(this.InputTopic);
            this.Consumer.Listener += _OnMessage;

            this.Producer = this.Session.CreateProducer(this.OutputTopic);
            this.Producer.DeliveryMode = MsgDeliveryMode.NonPersistent;
        }

		/// <summary>
		/// Send a message to the server.
		/// </summary>
		/// <param name="messageType">The type of the message</param>
		/// <param name="message">The actual message-string</param>
        public void SendMessage(string messageType, string message)
        {
            if (this.Producer == null)
            {
                Console.Error.WriteLine("[ComChannel] not initialized");
                return;
            }

            ITextMessage amqMessage = this.Session.CreateTextMessage(message);
            amqMessage.Properties.SetString(CLIENTID_PROPERTY, NetworkService.ClientID);
            amqMessage.Properties.SetString(MESSAGETYPE_PROPERTY, messageType);

            this.Producer.Send(amqMessage);
        }

		/// <summary>
		/// Removes this ComChannel from the NetworkService channel managment 
		/// and closes all its network resources.
		/// </summary>
		public void Close()
        {
            NetworkService._RemoveChannel(this.channelID);
            _CloseInternal();
        }

		/// <summary>
		/// Used internally. Closes all network resources of this ComChannel, 
		/// without removing it from the NetworkService channel managment.
		/// </summary>
		internal void _CloseInternal()
        {
            if (Producer != null)
            {
                Producer.Close();
                Producer = null;
            }
            if (Consumer != null)
            {
                Consumer.Close();
                Consumer = null;
            }
            if (Session != null)
            {
                Session.Close();
                Session = null;
            }
            InputTopic = null;
            OutputTopic = null;
            OnMessageReceived = null;
        }

		/// <summary>
		/// Used internally. Receives as message and passes it to the ComChannels listeners.
		/// </summary>
		/// <param name="amqMessage">The received raw amq message</param>
        internal void _OnMessage(IMessage amqMessage)
        {
            if (!(amqMessage is ITextMessage))
            {
                Console.Error.WriteLine("[ComChannel] Recieved unsupported amq message type: " + amqMessage.GetType());
                return;
            }

            string messageType = null;

            messageType = amqMessage.Properties.GetString(MESSAGETYPE_PROPERTY);

            if (messageType == null)
                Console.Error.WriteLine("[ComChannel] Cannot determine message type");
            else if (messageType.Equals(string.Empty))
                Console.Error.WriteLine("[ComChannel] Cannot determine message type");

            ITextMessage amqTextMessage = amqMessage as ITextMessage;
            string message = amqTextMessage.Text;

            OnMessageReceived?.Invoke(messageType, message);
        }
    }
}