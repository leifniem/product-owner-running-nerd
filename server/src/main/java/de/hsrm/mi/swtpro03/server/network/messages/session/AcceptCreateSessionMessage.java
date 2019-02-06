package de.hsrm.mi.swtpro03.server.network.messages.session;

public class AcceptCreateSessionMessage {

	public static final String TYPE = "AcceptCreateSessionMessage";

	private String sessionID;
	private String color;

	public AcceptCreateSessionMessage() {
	}

	public AcceptCreateSessionMessage(String sessionID,String color) {
		this.sessionID = sessionID;
		this.color = color;
	}

	public String getSessionID() {
		return this.sessionID;
	}

	public void setSessionID(String sessionID) {
		this.sessionID = sessionID;
	}

	public String getColor() {
		return color;
	}

	public void setColor(String color) {
		this.color = color;
	}
}
