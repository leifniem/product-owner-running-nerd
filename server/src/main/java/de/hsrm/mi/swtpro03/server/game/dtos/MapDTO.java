package de.hsrm.mi.swtpro03.server.game.dtos;

import de.hsrm.mi.swtpro03.server.game.model.Item;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Tile;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;
import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;

import java.util.Arrays;
import java.util.List;

public class MapDTO {

	private MapMetaDTO meta;
	private Tile[][] tiles;
	private Item[][] items;
	private EnemySpawnPointDTO[] enemySpawnPoints;
	private Vector2Int spawn;



	// Default Constructor for Jackson
	public MapDTO() {
	}

	public MapDTO(MapMetaDTO meta, Tile[][] tiles, Item[][] items, Vector2Int spawn, EnemySpawnPointDTO[] enemySpawnPoints) {
		this.meta = meta;
		this.tiles = tiles;
		this.items = items;
		this.enemySpawnPoints = enemySpawnPoints;
		this.spawn = spawn;
	}

	public MapMetaDTO getMeta() {
		return meta;
	}

	public void setMeta(MapMetaDTO meta) {
		this.meta = meta;
	}

	public Tile[][] getTiles() {
		return tiles;
	}

	public void setTiles(Tile[][] tiles) {
		this.tiles = tiles;
	}

	public Item[][] getItems() {
		return items;
	}

	public void setItems(Item[][] items) {
		this.items = items;
	}	

	public EnemySpawnPointDTO[] getEnemySpawnPoints() {
		return enemySpawnPoints;
	}

	public void setEnemySpawnPoints(EnemySpawnPointDTO[] enemySpawnPoints) {
		this.enemySpawnPoints = enemySpawnPoints;
	}

	public Vector2Int getSpawn() {
		return spawn;
	}

	public void setSpawn(Vector2Int spawn) {
		this.spawn = spawn;
	}

	public Map toMap() {
		Map map = new Map(meta.getNumberOfSections(), meta.getName(), spawn);
		for (int x = 0; x < meta.getNumberOfSections() * Map.SECTION_WIDTH; x++) {
			for (int y = 0; y < Map.SECTION_HEIGHT; y++) {
				map.addTile(x, y, tiles[x][y]);
				map.addItem(x, y, items[x][y]);
			}
		}

		if (enemySpawnPoints != null) {
			for (EnemySpawnPointDTO enemySpawnPoint : enemySpawnPoints) {
				map.addEnemySpawnPoint(enemySpawnPoint);
			}
		}
		return map;
	}

	@Override
	public boolean equals(Object obj) {
		if (obj instanceof MapDTO) {
			MapDTO dto = (MapDTO) obj;
			return dto.meta.equals(this.meta)
					&& Arrays.deepEquals(dto.tiles, this.tiles)
					&& Arrays.deepEquals(dto.items, this.items)
					&& Arrays.deepEquals(dto.enemySpawnPoints, this.enemySpawnPoints);
		}
		return false;
	}
}
