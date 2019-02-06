package de.hsrm.mi.swtpro03.server.network;

import javax.jms.*;
import java.util.ArrayList;
import java.util.List;

/**
 * A wrapper class, that encapsulates multiple active MQ topics. 
 * One to send 1-n messages to all clients and one to receive n-1 messages from all clients. 
 * This way, each ComChannel provides two-way communication without the need to filter incomming messages. 
 * Direct 1-1 messages to individual clients are sent via a seperate per-client tempQueue, which is handled in ConnectionManager. 
 * On the client-side, incomming 1-1 messages are automatically dispatched to the corresponding ComChannel.
 * 
 * @author tschn002
 */
public class ComChannel implements MessageListener {

	protected static final String INPUT_TOPIC = ".ServerInput";
	protected static final String OUTPUT_TOPIC = ".ServerOutput";

	protected static final String CLIENTID_PROPERTY = "Username";
	protected static final String CHANNELID_PROPERTY = "ChannelID";
	protected static final String MESSAGETYPE_PROPERTY = "MessageType";

	private String channelID;

	private Session session;
	private Topic inputTopic;
	private Topic outputTopic;

	private MessageConsumer consumer = null;
	private List<IMessageListener> listeners = null;

	private MessageProducer producer = null;

	/**
	 * Do not use this constructor. Use NetworkService.createComChannel() instead. 
	 * @param connection The active MQ connection (handled in NetworkService)
	 * @param channelID The name of the channel to be created
	 */
	protected ComChannel(Connection connection, String channelID) {
		try {
			this.channelID = channelID;
			this.session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);

			this.inputTopic = session.createTopic(channelID + INPUT_TOPIC);
			this.outputTopic = session.createTopic(channelID + OUTPUT_TOPIC);

			this.listeners = new ArrayList<>();
			this.consumer = session.createConsumer(inputTopic);
			this.consumer.setMessageListener(this);

			this.producer = this.session.createProducer(outputTopic);
			this.producer.setDeliveryMode(DeliveryMode.NON_PERSISTENT);

		} catch (JMSException e) {
			System.err.println("[ComChannel] failed to initialize");
			e.printStackTrace();
			close();
		}
	}

	/**
	 * Add a message listener. All message listeners are notified when new messages arrive. 
	 * @param listener A message listener
	 */
	public void addMessageListener(IMessageListener listener) {
		if (this.listeners == null) {
			System.err.println("[ComChannel] not initialized");
			return;
		}

		this.listeners.add(listener);
	}

	/**
	 * Send a message to all subscribers
	 * @param messageType The type of the message
	 * @param message The actual message
	 */
	public synchronized void sendToAll(String messageType, String message) {
		if (this.producer == null) {
			System.err.println("[ComChannel] not initialized");
			return;
		}

		try {

			Message amqMessage = session.createTextMessage(message);
			amqMessage.setStringProperty(MESSAGETYPE_PROPERTY, messageType);
			producer.send(amqMessage);

		} catch (JMSException e) {
			System.err.println("[ComChannel] failed to send message");
			e.printStackTrace();
		}

	}

	/**
	 * Send a direct 1-1 message to one specific client. 
	 * Direct 1-1 messages to individual clients are sent via a seperate per-client tempQueue, which is handled in ConnectionManager. 
	 * On the client-side, incomming 1-1 messages are automatically dispatched to the corresponding ComChannel.
	 * 
	 * @param clientID
	 * @param messageType
	 * @param message
	 */
	public synchronized void sendToClient(String clientID, String messageType, String message) {
		if (this.producer == null) {
			System.err.println("[ComChannel] not initialized");
			return;
		}
		NetworkService._sendToClient(this.channelID, clientID, messageType, message);
	}

	/**
	 * Removes this ComChannel from the NetworkService channel managment 
	 * and closes all its network resources.
	 */
	public void close() {
		NetworkService._removeChannel(this.channelID);
		_closeInternal();
	}

	/**
	 *  Used internally. Closes all network resources of this ComChannel, 
	 *  without removing it from the NetworkService channel managment.
	 */
	protected void _closeInternal() {

		try {
			if (producer != null) {
				producer.close();
				producer = null;
			}
			if (consumer != null) {
				consumer.close();
				consumer = null;
			}
			if (session != null) {
				session.close();
				session = null;
			}
			inputTopic = null;
			outputTopic = null;
			listeners = null;
		} catch (JMSException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Listens for new messages on the internal active MQ message consumer.
	 */
	@Override
	public void onMessage(Message amqMessage) {

		if (!(amqMessage instanceof TextMessage)) {
			System.err.println("[ComChannel] Received unsupported amq message type: " + amqMessage.getClass());
		}

		String clientID = null;
		try {
			clientID = amqMessage.getStringProperty(CLIENTID_PROPERTY);
		} catch (JMSException e1) {
			System.err.println("[ComChannel] Cannot determine clientID");
			e1.printStackTrace();
		}
		if (clientID == null) {
			System.err.println("[ComChannel] Cannot determine clientID");
		}

		String messageType = null;
		try {
			messageType = amqMessage.getStringProperty(MESSAGETYPE_PROPERTY);
		} catch (JMSException e1) {
			System.err.println("[ComChannel] Cannot determine message type");
			e1.printStackTrace();
		}
		if (messageType == null) {
			System.err.println("[ComChannel] Cannot determine message type");
		}

		TextMessage amqTextMessage = (TextMessage) amqMessage;
		String message = null;

		try {
			message = amqTextMessage.getText();
		} catch (JMSException e) {
			System.err.println("[ComChannel] Failed to extract message-string");
			e.printStackTrace();
		}

		for (IMessageListener listener : listeners) {
			listener.onMessageReceived(clientID, messageType, message);
		}
	}

}
