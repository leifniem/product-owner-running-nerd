package de.hsrm.mi.swtpro03.server.network.messages.session;

public class StartGameMessage {
	public static final String TYPE = "StartGameMessage";

	private boolean dummy;

	public StartGameMessage() {
	}

	public StartGameMessage(boolean dummy) {
		this.dummy = dummy;
	}

	public boolean isDummy() {
		return dummy;
	}

	public void setDummy(boolean dummy) {
		this.dummy = dummy;
	}
}
