package de.hsrm.mi.swtpro03.server.network;

import javax.jms.*;

/**
 * An internally used wrapper-class that encapsulates a clients tempQueue for direct 1-1 messages, 
 * as well as a timer for the clients last keepalive message.
 * @author tschn002
 */
class Client {

	private Session session;
	private MessageProducer producer = null;

	long lastKeepAlive;

	/**
	 * Creates a new Client
	 * @param connection The active MQ connection (handled in NetworkService)
	 * @param clientQueueIdentifier The clients tempQueue Destination
	 */
	Client(Connection connection, Destination clientQueueIdentifier) {
		try {
			this.session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
			this.producer = session.createProducer(clientQueueIdentifier);
			this.lastKeepAlive = System.currentTimeMillis();

		} catch (JMSException e) {
			System.err.println("[ClientChannel] failed to initialize");
			e.printStackTrace();
			close();
		}
	}

	/**
	 * Used internally. Send a direct 1-1 message to this client.
	 * @param channelID The source channel name/ID
	 * @param messageType The type of the message
	 * @param message The actual message
	 */
	synchronized void sendMessage(String channelID, String messageType, String message) {
		if (this.producer == null) {
			System.err.println("[ClientChannel] not initialized");
			return;
		}

		try {
			Message amqMessage = session.createTextMessage(message);
			amqMessage.setStringProperty(ComChannel.CHANNELID_PROPERTY, channelID);
			amqMessage.setStringProperty(ComChannel.MESSAGETYPE_PROPERTY, messageType);
			producer.send(amqMessage);
		} catch (JMSException e) {
			System.err.println("[ClientChannel] failed to send message");
			e.printStackTrace();
		}
	}

	/**
	 * Closes all network resources
	 */
	void close() {
		try {
			if (producer != null) {
				producer.close();
				producer = null;
			}
			if (session != null) {
				session.close();
				session = null;
			}
		} catch (JMSException e) {
			e.printStackTrace();
		}
	}


}
