package de.hsrm.mi.swtpro03.server.network.messages;

public class EditMapAllowedMessage {
	public static final String TYPE = "EditMapAllowedMessage";

	private boolean isAllowed;

	public EditMapAllowedMessage() {
	}

	public EditMapAllowedMessage(boolean isAllowed) {
		setAllowed(isAllowed);
	}

	public boolean isAllowed() {
		return isAllowed;
	}

	public void setAllowed(boolean allowed) {
		isAllowed = allowed;
	}
}
