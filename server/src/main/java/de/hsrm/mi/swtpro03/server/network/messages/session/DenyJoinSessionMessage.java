package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DenyJoinSessionMessage {

	public static final String TYPE = "DenyJoinSessionMessage";

	private String reason;

	public DenyJoinSessionMessage() {
	}

	public DenyJoinSessionMessage(String reason) {
		this.reason = reason;
	}

	public String getReason() {
		return this.reason;
	}

	public void setReason(String reason) {
		this.reason = reason;
	}
}
