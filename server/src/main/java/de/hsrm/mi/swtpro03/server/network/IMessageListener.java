package de.hsrm.mi.swtpro03.server.network;

/**
 * Interface for message listeners.
 * Message listeners are notified whenever a new message arrives on a ComChannel.
 * @author lvonk001, tschn002
 **/
public interface IMessageListener {

	/**
	 * Called whenever a new message arrives on a ComChannel.
	 * @param clientID The ID of the client, who sent the message
	 * @param messageType The type of the message
	 * @param message The actual message
	 */
	void onMessageReceived(String clientID, String messageType, String message);
}
