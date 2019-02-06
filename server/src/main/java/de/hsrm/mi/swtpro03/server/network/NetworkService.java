package de.hsrm.mi.swtpro03.server.network;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import javax.jms.Connection;
import javax.jms.Destination;
import javax.jms.JMSException;

import org.apache.activemq.ActiveMQConnectionFactory;
import org.apache.activemq.broker.BrokerService;

/**
 * This class provides static access to all networking features
 * @author lvonk001, tschn002
**/
public class NetworkService {

	// ServerSettings
	public static String CONNECTOR_IP = "tcp://0.0.0.0:";
	public static String CONNECTION_URL = "vm://localhost";
	public static String CONNECTION_MANAGER_TOPIC = "Server";
	public static long CONNECTION_MANAGER_TIMOUT = 6000;

	private static BrokerService broker = null;
	private static Connection connection = null;

	private static ConnectionManager connectionManager;
	private static Map<String, ComChannel> allComChannels;

	
	/**
	 * Creates and starts an active MQ message broker, establishes a local connection, 
	 * adds a TCP-connector with the designated port and starts connection managment, 
	 * which listens for incoming connection event messages, such as "Connect", 
	 * "Disconnect", and "Keepalive".
	 * @param port The port for the brokers TCP-connector
	 */
	public static void start(int port) {
		if (broker != null) {
			return;
		}

		try {
			broker = new BrokerService();
			broker.setPersistent(false);

			broker.addConnector(CONNECTOR_IP + port);
			broker.start();

			ActiveMQConnectionFactory factory = new ActiveMQConnectionFactory(CONNECTION_URL);
			connection = factory.createConnection();
			connection.start();

			connectionManager = new ConnectionManager(connection, CONNECTION_MANAGER_TOPIC);
			allComChannels = Collections.synchronizedMap(new HashMap<>());

		} catch (Exception e) {
			System.err.println("[NetworkService] failed to initialize");
			e.printStackTrace();
			shutdown();
		}
	}

	/**
	 * Shuts down the message broker and closes all network resources
	 */
	public static void shutdown() {
		if (allComChannels != null) {
			for (ComChannel channel : allComChannels.values()) {
				channel._closeInternal();
			}
			allComChannels = null;
		}

		try {
			if (connectionManager != null) {
				connectionManager.close();
				connectionManager = null;
			}
			if (connection != null) {
				connection.close();
				connection = null;
			}
			if (broker != null) {
				broker.stop();
				broker = null;
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
	}

	/**
	 * Add a connection listener. 
	 * All connection listeners get notified, when a Client connects, 
	 * disconnects or fails to send keepalive messages.
	 * @param listener A connection listener
	 */
	public static void addConnectionListener(IConnectionListener listener) {
		if (connectionManager == null) {
			System.err.println("[NetworkService] not initialized");
			return;
		}
		connectionManager.addConnectionListener(listener);
	}

	/**
	 * Creates a ComChannel with the specified channel name. start() has to be called first.
	 * @param channelID The name/ID of the ComChannel
	 * @return The created ComChannel or null if something went wrong
	 */
	public static ComChannel createComChannel(String channelID) {
		if (connection == null) {
			System.err.println("[NetworkService] not initialized");
			return null;
		}
		if (allComChannels.containsKey(channelID)) {
			System.err.println("[NetworkService] duplicate channelID: " + channelID);
			return null;
		}

		ComChannel channel = new ComChannel(connection, channelID);
		allComChannels.put(channelID, channel);

		return channel;
	}

	/**
	 * Used internally. Removes a ComChannel from the ComChannel managment, after calling ComChannel.close().
	 * @param channelID The name/ID of the ComChannel
	 */
	protected static void _removeChannel(String channelID) {
		allComChannels.remove(channelID);
	}

	/**
	 * Used internally by ConnectionManager. Creates a client object with an active MQ tempQueue.
	 * @param clientQueueDestination The clients tempQueue destination
	 * @return The created client or null if something went wrong
	 */
	protected static Client _createClient(Destination clientQueueDestination) {
		if (connectionManager == null) {
			System.err.println("[NetworkService] not initialized");
			return null;
		}

		return new Client(connection, clientQueueDestination);
	}

	/**
	 * Used internally by ComChannel. Sends a direct 1-1 message to the specified client using its tempQueue.
	 * If the clientID is invalid, the call is ignored.
	 * @param channelID The name/ID of the source channel
	 * @param clientID The ID of the target client
	 * @param messageType The type of the message
	 * @param message The actual message
	 */
	protected static void _sendToClient(String channelID, String clientID, String messageType, String message) {
		Client client = connectionManager.getClientChannel(clientID);
		if (client != null)
			client.sendMessage(channelID, messageType, message);
	}

}
