package de.hsrm.mi.swtpro03.server.network.messages.editor;

public class TileRemovedMessage {

	public static final String TYPE = "TileRemovedMessage";
	private int posX;
	private int posY;

	public TileRemovedMessage() {
	}

	public TileRemovedMessage(int posX, int posY) {
		this.posX = posX;
		this.posY = posY;
	}

	public int getPosX() {
		return posX;
	}

	public void setPosX(int posX) {
		this.posX = posX;
	}

	public int getPosY() {
		return posY;
	}

	public void setPosY(int posY) {
		this.posY = posY;
	}
}
