package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DenyCreateSessionMessage {

	public static final String TYPE = "DenyCreateSessionMessage";

	private String reason;

	public DenyCreateSessionMessage() {
	}

	public DenyCreateSessionMessage(String reason) {
		this.reason = reason;
	}

	public String getReason() {
		return this.reason;
	}

	public void setReason(String reason) {
		this.reason = reason;
	}
}
