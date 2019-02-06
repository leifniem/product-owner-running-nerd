package de.hsrm.mi.swtpro03.server.game.dtos;

import de.hsrm.mi.swtpro03.server.game.model.GameCharacterState;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;

public class GameCharacterDTO {
	private String uuid;
	private Vector2 position;
	private String color;
	private GameCharacterState gameCharacterState;
	private boolean enemy;

	public GameCharacterDTO() {
	}

	public Vector2 getPosition() {
		return position;
	}

	public void setPosition(Vector2 position) {
		this.position = position;
	}

	public GameCharacterState getGameCharacterState() {
		return gameCharacterState;
	}

	public void setGameCharacterState(GameCharacterState gameCharacterState) {
		this.gameCharacterState = gameCharacterState;
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

	public boolean isEnemy() {
		return enemy;
	}

	public void setEnemy(boolean enemy) {
		this.enemy = enemy;
	}	
	
}
