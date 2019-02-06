package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.model.Cursor;

public class CursorMovedMessage {

	public static final String TYPE = "CursorMovedMessage";
	private Cursor cursor;

	public CursorMovedMessage() {
	}

	public CursorMovedMessage(Cursor cursor) {
		this.cursor = cursor;
	}

	public Cursor getCursor() {
		return cursor;
	}

	public void setCursor(Cursor cursor) {
		this.cursor = cursor;
	}
}
