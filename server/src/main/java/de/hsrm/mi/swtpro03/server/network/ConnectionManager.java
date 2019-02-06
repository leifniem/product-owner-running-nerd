package de.hsrm.mi.swtpro03.server.network;

import javax.jms.*;
import java.util.*;
import java.util.Map.Entry;

/**
 * This class handles connections by listening for connection event messages
 * on an internally used connection event topic.
 * @author tschn002
 */
class ConnectionManager implements MessageListener {

	private static final String CONNECTION_EVENT_PROPERTY = "ConnectionEvent";
	private static final String CONNECT = "Connect";
	private static final String DISCONNECT = "Disconnect";
	private static final String KEEPALIVE = "Keepalive";

	private Session session;
	private Topic topic;
	private MessageConsumer consumer;
	private Map<String, Client> connections;

	private List<IConnectionListener> connectionListeners;

	/**
	 * Creates a new ConnectionManager
	 * @param connection The active MQ connection (handled in NetworkService)
	 * @param connectionTopicName Name of the internally used connection event topic
	 * @throws JMSException propagates the exception back to the NetworkService if initialization fails
	 */
	protected ConnectionManager(Connection connection, String connectionTopicName) throws JMSException {

		this.connections = new HashMap<>();
		this.connectionListeners = new ArrayList<>();

		this.session = connection.createSession(false, Session.AUTO_ACKNOWLEDGE);
		this.topic = session.createTopic(connectionTopicName);
		this.consumer = session.createConsumer(topic);
		this.consumer.setMessageListener(this);

		startCheckConnectionsThread();
	}

	/**
	 * Used internally. Access through NetworkService
	 * @param listener
	 */
	protected void addConnectionListener(IConnectionListener listener) {
		if (this.connectionListeners == null) {
			System.err.println("[ConnectionManager] not initialized");
			return;
		}
		this.connectionListeners.add(listener);
	}

	/**
	 * Shutdown the ConnectionManager and close all network resources
	 */
	protected void close() {
		try {
			if (consumer != null) {
				consumer.close();
				consumer = null;
			}
			if (session != null) {
				session.close();
				session = null;
			}
			topic = null;
			connectionListeners = null;
		} catch (JMSException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Listens for connection event messages on the internal connection event topic.
	 */
	@Override
	public void onMessage(Message amqMessage) {

		Destination clientQueueIdentifier;
		String clientID;
		try {
			clientQueueIdentifier = amqMessage.getJMSReplyTo();
			clientID = clientQueueIdentifier.toString();
		} catch (JMSException e1) {
			System.err.println("[ConnectionManager] Failed to extract reply channel");
			e1.printStackTrace();
			return;
		}

		String connectionEvent;
		try {
			connectionEvent = amqMessage.getStringProperty(CONNECTION_EVENT_PROPERTY);
		} catch (JMSException e1) {
			System.err.println("[ConnectionManager] Cannot determine ConnectionEvent type");
			e1.printStackTrace();
			return;
		}

		switch (connectionEvent) {
			case CONNECT:
				handleClientConnect(clientID, clientQueueIdentifier);
				break;
			case KEEPALIVE:
				handleClientKeepalive(clientID);
				break;
			case DISCONNECT:
				handleClientDisconnect(clientID);
				break;
		}
	}

	/**
	 * Used internally. Retrieve a client-object by the clients ID.
	 * @param clientID The clients ID
	 * @return
	 */
	protected Client getClientChannel(String clientID) {
		if (!connections.containsKey(clientID)) {
			System.err.println("[ConnectionManager] client not found: " + clientID);
			return null;
		}
		return connections.get(clientID);
	}

	/**
	 * Handles connection event messages of type "Connected".
	 * Creates a new client-object using the clients tempQueue identifier as a unique clientID, 
	 * and keeps it in an internal list for future reference. 
	 * Notifies all connection listeners.
	 * @param clientID The clients ID
	 * @param clientQueueIdentifier The clients tempQueue destination
	 */
	private void handleClientConnect(String clientID, Destination clientQueueIdentifier) {
		if (connections.containsKey(clientID)) {
			System.err.println("[ConnectionManager] Duplicate connect message from client: " + clientID);
			return;
		}

		Client client = NetworkService._createClient(clientQueueIdentifier);
		connections.put(clientID, client);

		for (IConnectionListener listener : connectionListeners) {
			listener.onClientConnected(clientID);
		}
	}

	/**
	 * Handles connection event messages of type "Disconnect".
	 * Removes the corresponding client-object from connection managment. 
	 * Notifies all connection listeners.
	 * @param clientID The clients ID
	 */
	private void handleClientDisconnect(String clientID) {
		if (!connections.containsKey(clientID)) {
			System.err.println("[ConnectionManager] Disconnect message from unknown client: " + clientID);
			return;
		}

		connections.get(clientID).close();
		connections.remove(clientID);

		for (IConnectionListener listener : connectionListeners) {
			listener.onClientDisconnected(clientID);
		}
	}

	/**
	 * Handles connection event messages of type "Keepalive".
	 * Updates an internal timer in the client-object. When a client fails to send keepalive messages,
	 * it is removed from connection managment and all connection listeners are notified.
	 * @param clientID The clients ID
	 */
	private void handleClientKeepalive(String clientID) {
		if (!connections.containsKey(clientID)) {
			System.err.println("[ConnectionManager] Keepalive message from unknown client: " + clientID);
			return;
		}

		connections.get(clientID).lastKeepAlive = System.currentTimeMillis();
	}

	/**
	 * Starts a thread that checks all clients keepalive timers in regular intervals. 
	 * Clients that failed to send keepalive messages will be removed from connection managment 
	 * and all connection listeners will be notified.
	 */
	private void startCheckConnectionsThread() {

		Runnable checkConnectionsLoop = () -> {

			while (consumer != null) {

				try {
					Thread.sleep(NetworkService.CONNECTION_MANAGER_TIMOUT);
				} catch (InterruptedException e) {
					e.printStackTrace();
					return;
				}

				long currentTime = System.currentTimeMillis();

				Iterator<Entry<String, Client>> iter = connections.entrySet().iterator();
				while (iter.hasNext()) {
					Map.Entry<String, Client> entry = iter.next();

					String clientID = entry.getKey();
					Client client = entry.getValue();

					if (currentTime > client.lastKeepAlive + NetworkService.CONNECTION_MANAGER_TIMOUT) {
						client.close();
						iter.remove();

						for (IConnectionListener listener : connectionListeners) {
							listener.onClientTimeout(clientID);
						}
					}
				}
			}
		};

		Thread loopThread = new Thread(checkConnectionsLoop);
		loopThread.start();
	}

}
