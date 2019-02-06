package de.hsrm.mi.swtpro03.server.game.dtos;

public class TileDTO {
	private String type;
	private int gridX;
	private int gridY;

	// Default constructor for Jackson
	public TileDTO() {
	}

	public TileDTO(int gridX, int gridY, String type) {
		this.type = type;
		this.gridX = gridX;
		this.gridY = gridY;
	}

	public String getType() {
		return type;
	}

	public void setType(String type) {
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

	@Override
	public boolean equals(Object obj) {
		if (obj instanceof TileDTO) {
			TileDTO dto = (TileDTO) obj;
			return dto.getGridX() == this.gridX || dto.getGridY() == this.gridY || dto.getType().equals(this.type);
		}
		return false;
	}
}
