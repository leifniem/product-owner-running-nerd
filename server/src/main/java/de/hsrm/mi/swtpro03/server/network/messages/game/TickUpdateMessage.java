package de.hsrm.mi.swtpro03.server.network.messages.game;

import java.util.HashMap;
import java.util.List;

public class TickUpdateMessage {

	public static final String TYPE = "TickUpdateMessage";

	private HashMap<String, List<Object>> changes;

	public TickUpdateMessage() {
	}

	public TickUpdateMessage(HashMap<String, List<Object>> changes) {
		this.changes = changes;
	}

	public HashMap<String, List<Object>> getChanges() {
		return changes;
	}

	public void setChanges(HashMap<String, List<Object>> changes) {
		this.changes = changes;
	}
}
