package de.hsrm.mi.swtpro03.server.network.messages.game;


import de.hsrm.mi.swtpro03.server.game.model.GameCharacterState;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;

public class MoveActorMessage {

	public static final String TYPE = "MoveActorMessage";

	private String uuid;
	private Vector2 newPosition;
	private GameCharacterState state;

	public MoveActorMessage() {
	}

	public MoveActorMessage(String uuid, Vector2 newPosition, GameCharacterState state) {
		this.uuid = uuid;
		this.newPosition = newPosition;
		this.state = state;
	}

	public String getUuid() {
		return uuid;
	}

	public void setUuid(String uuid) {
		this.uuid = uuid;
	}

	public Vector2 getNewPosition() {
		return newPosition;
	}


	public void setNewPosition(Vector2 newPosition) {
		this.newPosition = newPosition;
	}

	public GameCharacterState getState() {
		return state;
	}

	public void setState(GameCharacterState state) {
		this.state = state;
	}
}
