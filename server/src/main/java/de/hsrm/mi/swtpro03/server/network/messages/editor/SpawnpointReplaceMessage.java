package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;

public class SpawnpointReplaceMessage {

	public static final String TYPE = "SpawnpointReplaceMessage";

	private Vector2Int spawn;

	public SpawnpointReplaceMessage() {
	}

	public SpawnpointReplaceMessage(Vector2Int spawn) {
		this.spawn = spawn;
	}

	public Vector2Int getSpawn() {
		return spawn;
	}

	public void setSpawn(Vector2Int spawn) {
		this.spawn = spawn;
	}
}
