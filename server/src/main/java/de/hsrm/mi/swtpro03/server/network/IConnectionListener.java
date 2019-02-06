package de.hsrm.mi.swtpro03.server.network;

/**
 * Interface for connection listeners.
 * Connection listeners will be notified whenever a client 
 * connects, disconnects or fails to send keepalive messages.
 * @author tschn002
 *
 */
public interface IConnectionListener {

	/**
	 * Called when a new client connects to the server.
	 * @param clientID The clients ID
	 */
	public void onClientConnected(String clientID);

	/**
	 * Called when a client disconnects from the server.
	 * @param clientID The clients ID
	 */
	public void onClientDisconnected(String clientID);

	/**
	 * Called when a client fails to send keepalive messages.
	 * @param clientID The clients ID
	 */
	public void onClientTimeout(String clientID);
}
