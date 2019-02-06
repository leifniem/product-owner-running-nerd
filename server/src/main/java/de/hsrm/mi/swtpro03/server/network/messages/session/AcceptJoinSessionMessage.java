package de.hsrm.mi.swtpro03.server.network.messages.session;

public class AcceptJoinSessionMessage {

	public static final String TYPE = "AcceptJoinSessionMessage";

	private String color;

	public AcceptJoinSessionMessage() {
	}

	public AcceptJoinSessionMessage(String color) {
		this.color = color;
	}

	public String getColor() {
		return color;
	}

	public void setColor(String color) {
		this.color = color;
	}
}
