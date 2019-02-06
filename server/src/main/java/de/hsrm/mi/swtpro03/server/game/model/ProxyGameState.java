package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.session.GameSession;

import java.util.ArrayList;
import java.util.Collection;
import java.util.List;

public class ProxyGameState {

	private GameSession session;
	private EnemyGameCharacter gameCharacter;
	private Map map;

	public ProxyGameState(GameSession session, EnemyGameCharacter enemyGameCharacter, Map map) {
		this.session = session;
		this.gameCharacter = enemyGameCharacter;
		this.map = map;
	}


	/**
	 * Calculates the Tiles around the computer controlled unit.
	 * The unit is always in the center of n*n Array.
	 * Tiles outside of the Map are Null!
	 *
	 * @return n*n Array of Tiles around the EnemyGameCharacter
	 * @see EnemyGameCharacter
	 */
	public Tile[][] getTilesInRange() {
		int posX = (int) gameCharacter.getPosition().x;
		int posY = (int) gameCharacter.getPosition().y;

		int arraySize = EnemyGameCharacter.RANGE_OF_SIGHT * 2 + 1;

		Tile[][] tilesInRange = new Tile[arraySize][arraySize];
		Tile[][] entireMap = map.getTiles();

		for (int y = 0; y < arraySize; y++) {

			int worldCoordsY = posY - EnemyGameCharacter.RANGE_OF_SIGHT + y;
			if (worldCoordsY < 0 || worldCoordsY + 1 > entireMap[0].length)
				continue;

			for (int x = 0; x < arraySize; x++) {

				int worldCoordsX = posX - EnemyGameCharacter.RANGE_OF_SIGHT + x;
				if (worldCoordsX < 0 || worldCoordsX + 1 > entireMap.length)
					continue;

				tilesInRange[x][y] = entireMap[worldCoordsX][worldCoordsY];

			}
		}

		return tilesInRange;
	}


	/**
	 * Looks if players are currently in range of sight of the computer controlled unit.
	 * The coordinates are relative so they match the Array from getTilesInRange.
	 *
	 * @return List of Vector2 classes which represent player coordinates
	 * @see Vector2
	 */
	public List<Vector2> getPlayersInRange() {

		Vector2 myPosition = gameCharacter.getPosition();

		List<Vector2> players = new ArrayList<>();

		Collection<PlayerGameCharacter> gameCharacters = session.getUserGameCharacterHashMap().values();

		for (PlayerGameCharacter character : gameCharacters) {
			if (character.isDead())
				continue;

			Vector2 playerPosition = character.getPosition();

			float difX = Math.abs(playerPosition.x - myPosition.x);
			float difY = Math.abs(playerPosition.y - myPosition.y);

			if (difX == 0 && difY == 0) {
				continue;
			}

			if (difX < EnemyGameCharacter.RANGE_OF_SIGHT && difY < EnemyGameCharacter.RANGE_OF_SIGHT) {
				Vector2 relativePosition = new Vector2(playerPosition.x - myPosition.x, playerPosition.y - myPosition.y);
				players.add(relativePosition);
			}
		}

		return players;
	}

}

