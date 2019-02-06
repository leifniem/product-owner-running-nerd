package de.hsrm.mi.swtpro03.server.network.messages.editor;

public class CursorIdMessage {

	public static final String TYPE = "CursorIdMessage";

	private String uuid;

	public CursorIdMessage() {
	}

	public CursorIdMessage(String uuid) {
		this.uuid = uuid;
	}

	public String getUuid() {
		return uuid;
	}

	public void setUuid(String uuid) {
		this.uuid = uuid;
	}


}
