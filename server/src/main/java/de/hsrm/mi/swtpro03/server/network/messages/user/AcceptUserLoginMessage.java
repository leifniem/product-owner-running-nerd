package de.hsrm.mi.swtpro03.server.network.messages.user;

public class AcceptUserLoginMessage {

	public static final String TYPE = "AcceptUserLoginMessage";

	private boolean dummy;

	public AcceptUserLoginMessage() {
	}

	public AcceptUserLoginMessage(boolean dummy) {
		this.dummy = dummy;
	}

	public boolean isDummy() {
		return dummy;
	}

	public void setDummy(boolean dummy) {
		this.dummy = dummy;
	}
}
