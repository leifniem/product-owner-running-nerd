package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DenyDeselectSectionMessage {
	public static final String TYPE = "DenyDeselectSectionMessage";

	private String reason;

	public DenyDeselectSectionMessage() {
	}

	public DenyDeselectSectionMessage(String reason) {
		this.reason = reason;
	}

	public String getReason() {
		return this.reason;
	}

	public void setReason(String reason) {
		this.reason = reason;
	}
}
