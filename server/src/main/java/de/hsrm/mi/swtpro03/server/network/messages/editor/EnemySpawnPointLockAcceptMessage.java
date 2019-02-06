package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

public class EnemySpawnPointLockAcceptMessage {

	public static final String TYPE = "EnemySpawnPointLockAcceptMessage";

	private EnemySpawnPointDTO enemySpawnDTO;
	
	public EnemySpawnPointLockAcceptMessage() {
		
	}

	public EnemySpawnPointLockAcceptMessage(EnemySpawnPointDTO enemySpawnDTO) {
		this.enemySpawnDTO = enemySpawnDTO;	
	}

	public EnemySpawnPointDTO getEnemySpawnDTO() {
		return enemySpawnDTO;
	}

	public void setEnemySpawnDTO(EnemySpawnPointDTO enemySpawnDTO) {
		this.enemySpawnDTO = enemySpawnDTO;
	}

	
}
