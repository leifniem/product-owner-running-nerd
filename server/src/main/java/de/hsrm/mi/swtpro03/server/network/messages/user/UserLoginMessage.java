package de.hsrm.mi.swtpro03.server.network.messages.user;

public class UserLoginMessage {

	public static final String TYPE = "UserLoginMessage";
	private String username;

	public UserLoginMessage() {
	}

	public UserLoginMessage(String username) {
		this.username = username;
	}

	public String getUsername() {
		return username;
	}

	public void setUsername(String username) {
		this.username = username;
	}
}
