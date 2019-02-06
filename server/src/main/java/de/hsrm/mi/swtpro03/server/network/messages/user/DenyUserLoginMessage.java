package de.hsrm.mi.swtpro03.server.network.messages.user;

public class DenyUserLoginMessage {

	public static final String TYPE = "DenyUserLoginMessage";

	private String reason;

	public DenyUserLoginMessage() {
	}

	public DenyUserLoginMessage(String reason) {
		this.reason = reason;
	}

	public String getReason() {
		return this.reason;
	}

	public void setReason(String reason) {
		this.reason = reason;
	}
}
