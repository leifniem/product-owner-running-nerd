package de.hsrm.mi.swtpro03.server.game.model;

public class Cursor {

	private int gridX;
	private int gridY;
	private String uuid;
	private String color;

	public Cursor() {
	}


	public Cursor(String uuid) {
		this.uuid = uuid;
	}

	public int getGridX() {
		return gridX;
	}

	public void setGridX(int gridX) {
		this.gridX = gridX;
	}

	public int getGridY() {
		return gridY;
	}

	public void setGridY(int gridY) {
		this.gridY = gridY;
	}

	public String getUuid() {
		return uuid;
	}

	public void setUuid(String uuid) {
		this.uuid = uuid;
	}

	public String getColor() {
		return color;
	}

	public void setColor(String color) {
		this.color = color;
	}
}
