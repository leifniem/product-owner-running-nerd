package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

public class EnemySpawnPointUnlockMessage {

	public static final String TYPE = "EnemySpawnPointUnlockMessage";

	private int gridX, gridY;
	
	public EnemySpawnPointUnlockMessage() {
	}

	public EnemySpawnPointUnlockMessage(int gridX, int gridY) {	
		this.gridX = gridX;
		this.gridY = gridY;
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

	
	
}
