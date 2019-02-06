package de.hsrm.mi.swtpro03.server.game.dtos;

public class EnemySpawnPointDTO {
	private String name;
	private String code;
	private int gridX;
	private int gridY;

	// Default constructor for Jackson
	public EnemySpawnPointDTO() {
	}

	public EnemySpawnPointDTO(int gridX, int gridY, String name, String code) {
		this.name = name;
		this.code = code;
		this.gridX = gridX;
		this.gridY = gridY;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getCode() {
		return code;
	}

	public void setCode(String code) {
		this.code = code;
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
		if (!(obj instanceof EnemySpawnPointDTO))
			return false;		
		
		EnemySpawnPointDTO dto = (EnemySpawnPointDTO) obj;
		if (dto.getGridX() != this.gridX)
			return false;
		if (dto.getGridY() != this.gridY)
			return false;		
		if ((!dto.getName().equals(this.name)))
			return false;		
		if ((!dto.getCode().equals(this.code)))
			return false;
		
		return true;
	}
	
	@Override
	public String toString() {
		return "EnemySpawnPointDTO(name=" + this.name + " x="+ this.gridX + " y=" + this.gridY + ")";
	}
}
