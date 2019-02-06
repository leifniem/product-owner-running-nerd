package de.hsrm.mi.swtpro03.server.game.dtos;

public class LifePointsDTO {

	private String uuid;
	private int lifePoints;

	public LifePointsDTO() {
	}

	public String getUuid() {
		return uuid;
	}

	public void setUuid(String uuid) {
		this.uuid = uuid;
	}

	public int getLifePoints() {
		return lifePoints;
	}

	public void setLifePoints(int lifePoints) {
		this.lifePoints = lifePoints;
	}
}
