package de.hsrm.mi.swtpro03.server.network.messages.game;

public class LifePointsUpdateMessage {

	public static final String TYPE = "LifePointsUpdateMessage";

	private String uuid;
	private int lifePoints;

	public LifePointsUpdateMessage() {
	}

	public LifePointsUpdateMessage(String uuid, int lifePoints) {
		this.uuid = uuid;
		this.lifePoints = lifePoints;
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
