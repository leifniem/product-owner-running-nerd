package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.model.Cursor;

import java.util.List;

public class EditorSessionJoinedMessage {

	public static final String TYPE = "EditorSessionJoinedMessage";
	private List<Cursor> cursor;

	public EditorSessionJoinedMessage() {
	}

	public EditorSessionJoinedMessage(List<Cursor> cursor) {
		this.cursor = cursor;
	}

	public List<Cursor> getCursor() {
		return cursor;
	}

	public void setCursor(List<Cursor> cursor) {
		this.cursor = cursor;
	}

}
