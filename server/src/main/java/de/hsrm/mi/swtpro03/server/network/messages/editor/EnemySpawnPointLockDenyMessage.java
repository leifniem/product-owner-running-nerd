package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

public class EnemySpawnPointLockDenyMessage {

	public static final String TYPE = "EnemySpawnPointLockDenyMessage";

	private String reason;
	
	public EnemySpawnPointLockDenyMessage() {
		
	}

	public EnemySpawnPointLockDenyMessage(String reason) {
		super();
		this.reason = reason;
	}

	public String getReason() {
		return reason;
	}

	public void setReason(String reason) {
		this.reason = reason;
	}
}
