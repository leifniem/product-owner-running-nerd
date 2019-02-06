package de.hsrm.mi.swtpro03.server.game.dtos;

public class ItemDTO {
	private String type;
	private int gridX;
	private int gridY;

	public ItemDTO() {}

	public ItemDTO(int gridX, int gridY, String type) {
		this.gridX = gridX;
		this.gridY = gridY;
		this.type = type;
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

	public String getType() {
		return type;
	}

	public void setType(String type) {
		this.type = type;
	}

	public boolean equals(Object obj) {
		if (obj instanceof ItemDTO) {
			ItemDTO dto = (ItemDTO) obj;
			return dto.getGridX() == this.gridX || dto.getGridY() == this.gridY || dto.getType().equals(this.type);
		}
		return false;
	}
}
