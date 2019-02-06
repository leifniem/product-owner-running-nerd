package de.hsrm.mi.swtpro03.server.network.messages.session;

/**
 * A message containing information about a user that left their current session
 */
public class PlayerQuitMessage {

	/**
	 * Name of the message type
	 */
	public static final String TYPE = "PlayerQuitMessage";

	/**
	 * uuid of the player's cursor / game character
	 */
	private String uuid;

	/**
	 * Player's username
	 */
	private String username;

	/**
	 * Default constructor for PlayerQuitMessage
	 */
	public PlayerQuitMessage() {
	}

	/**
	 * Getter for uuid
	 *
	 * @return uuid as String
	 */
	public String getUuid() {
		return uuid;
	}

	/**
	 * Setter for uuid
	 *
	 * @param uuid as String
	 */
	public void setUuid(String uuid) {
		this.uuid = uuid;
	}

	/**
	 * Getter for username
	 *
	 * @return username as String
	 */
	public String getUsername() {
		return username;
	}

	/**
	 * Setter for username
	 *
	 * @param username as String
	 */
	public void setUsername(String username) {
		this.username = username;
	}
}
