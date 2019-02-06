package de.hsrm.mi.swtpro03.server.game.model;


import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapMetaDTO;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapToSmallException;
import de.hsrm.mi.swtpro03.server.network.messages.game.TileChangedMessage;

import java.util.*;

public class Map extends Observable implements Updateable {
	public static final int SECTION_WIDTH = 32;
	public static final int SECTION_HEIGHT = 16;

	private static final int TILE_RESTORE_DELAY = 5000;

	private int numberOfSections;
	private final String name;
	private Tile[][] tiles;
	private Item[][] items;
	private Queue<TileTimer> destroyedTiles;
	private Vector2Int spawn;
	private List<EnemySpawnPointDTO> enemySpawnPoints;
	private List<EnemySpawnPointDTO> lockedEnemies;


	/**
	 * Timer class which stores the x, y coordinates of a destroyed tile
	 * and the time which the tile will remain destroyed for.
	 */
	private class TileTimer {
		private int gridX;
		private int gridY;
		private int timer;

		TileTimer(int gridX, int gridY) {
			this.gridX = gridX;
			this.gridY = gridY;
			this.timer = TILE_RESTORE_DELAY;
		}
	}


	/**
	 * @param numberOfSections number of screens/mapsections this map should be composed of
	 * @param name             name of this map
	 */
	public Map(int numberOfSections, String name, Vector2Int spawn) {
		this.name = name;
		this.numberOfSections = numberOfSections;
		tiles = new Tile[numberOfSections * SECTION_WIDTH][SECTION_HEIGHT];
		for (Tile[] tile : tiles) {
			Arrays.fill(tile, Tile.EMPTY);
		}
		items = new Item[numberOfSections * SECTION_WIDTH][SECTION_HEIGHT];
		for (Item[] item : items) {
			Arrays.fill(item, Item.EMPTY);
		}

		enemySpawnPoints = new ArrayList<>();
		lockedEnemies = new ArrayList<>();
		destroyedTiles = new LinkedList<>();
		this.spawn = spawn;
	}


	@Override
	public void update(long deltaTime) {
		Iterator<TileTimer> it = destroyedTiles.iterator();
		while (it.hasNext()) {
			TileTimer tileTimer = it.next();
			tileTimer.timer -= deltaTime;
			if (tileTimer.timer < 0) {
				restoreTile(tileTimer.gridX, tileTimer.gridY);
				it.remove();
			}
		}
	}


	/**
	 * Adds tile to specified coordinates.
	 *
	 * @param gridX x-coordinate where the new tile should be placed
	 * @param gridY y-coordinate where the new tile should be placed
	 * @param tile  tile type which should be added
	 */
	public void addTile(int gridX, int gridY, Tile tile) {
		tiles[gridX][gridY] = tile;
	}


	/**
	 * Returns the tile type at specified coordinates. If these coordinates are
	 * outside the array bounds the method will return a SOLID.
	 *
	 * @param gridX x-coordinate from which to retrieve tile
	 * @param gridY y-coordinate from which to retrieve tile
	 * @return tile type at coordinates
	 */
	public Tile getTile(int gridX, int gridY) {
		if (gridX < 0 || gridX >= numberOfSections * SECTION_WIDTH || gridY < 0 || gridY >= SECTION_HEIGHT)
			return Tile.SOLID;
		return tiles[gridX][gridY];
	}


	/**
	 * Destroys tile (place a DESTROYED_SOLID) at given position and
	 * if the current tile is DESTROYABLE_SOLID and starts a timer that is scheduled
	 * to restore the destroyed tile after a constant delay.
	 *
	 * @param gridX x-coordinate of tile
	 * @param gridY y-coordinate of tile
	 */
	public void destroyTile(int gridX, int gridY) {
		if (gridX < 0 || gridX >= numberOfSections * SECTION_WIDTH || gridY < 0 || gridY >= SECTION_HEIGHT)
			return;
		if (tiles[gridX][gridY] == Tile.DESTROYABLE_SOLID && tiles[gridX][gridY - 1] == Tile.EMPTY) {
			tiles[gridX][gridY] = Tile.DESTROYED_SOLID;
			destroyedTiles.add(new TileTimer(gridX, gridY));
			setChanged();
			notifyObservers(new TileChangedMessage(TileChangedMessage.DESTROYED_STATE, gridX, gridY));
		}
	}


	/**
	 * Restores previously destroyed tile (replaces DESTROYED_SOLID with DESTROYABLE_SOLID).
	 *
	 * @param gridX x-coordinate of tile
	 * @param gridY y-coordinate of tile
	 */
	private void restoreTile(int gridX, int gridY) {
		tiles[gridX][gridY] = Tile.DESTROYABLE_SOLID;
		setChanged();
		notifyObservers(new TileChangedMessage(TileChangedMessage.RESTORED_STATE, gridX, gridY));
	}


	/**
	 * Removes tile at given coordinates (replaces it with EMPTY).
	 *
	 * @param gridX x-coordinate to remove tile from
	 * @param gridY y-coordinate to remove tile from
	 */
	public void removeTile(int gridX, int gridY) {
		tiles[gridX][gridY] = Tile.EMPTY;
	}


	/**
	 * Adds item to map at specified coordinates. If another item is already present
	 * it will be overwritten.
	 *
	 * @param gridX x-coordinate where the new item should be placed
	 * @param gridY y-coordinate where the new item should be placed
	 * @param item  item object to add
	 */
	public void addItem(int gridX, int gridY, Item item) {
		items[gridX][gridY] = item;
	}


	/**
	 * @param gridX x-coordinate to retrieve item from
	 * @param gridY x-coordinate to retrieve item from
	 * @return item at coordinates
	 */
	public Item getItem(int gridX, int gridY) {
		return items[gridX][gridY];
	}


	/**
	 * Removes item at given coordinates (replaces it with EMPTY).
	 *
	 * @param gridX x-coordinate to remove item from
	 * @param gridY y-coordinate to remove item from
	 */
	public void removeItem(int gridX, int gridY) {
		items[gridX][gridY] = Item.EMPTY;
	}


	/**
	 * Picks up item from given coordinates, replaces the item with EMPTY and
	 * returns the item that was picked up.
	 *
	 * @param gridX x-coordinate to pick up item from
	 * @param gridY y-coordinate to pick up item from
	 * @return picked up item or Item.EMPTY
	 */
	public Item pickUpItem(int gridX, int gridY) {
		Item item = items[gridX][gridY];
		if (item != Item.EMPTY) {
			items[gridX][gridY] = Item.EMPTY;
		}
		return item;
	}


	/**
	 * @return new MapDTO made from this map object
	 */
	public MapDTO toMapDTO() {
		MapMetaDTO meta = new MapMetaDTO(name, numberOfSections);
		EnemySpawnPointDTO[] enemies = new EnemySpawnPointDTO[0];
		enemies = enemySpawnPoints.toArray(enemies);
		return new MapDTO(meta, tiles, items, spawn, enemies);
	}


	/**
	 * Adds a new Section to right end of the map.
	 */
	public void addSection() {
		numberOfSections++;
		Tile[][] newTiles = new Tile[tiles.length + SECTION_WIDTH][SECTION_HEIGHT];
		Item[][] newItems = new Item[items.length + SECTION_WIDTH][SECTION_HEIGHT];

		for (int x = 0; x < tiles.length; x++) {
			System.arraycopy(tiles[x], 0, newTiles[x], 0, tiles[x].length);
			System.arraycopy(items[x], 0, newItems[x], 0, newItems[x].length);
		}

		for (int x = 0; x < newTiles.length; x++) {
			for (int y = 0; y < newTiles[x].length; y++) {
				if (newTiles[x][y] == null) {
					newTiles[x][y] = Tile.EMPTY;
				}
				if (newItems[x][y] == null) {
					newItems[x][y] = Item.EMPTY;
				}
			}
		}
		tiles = newTiles;
		items = newItems;
	}


	/**
	 * Removes the last added Section.
	 */
	public void removeSection() {
		if (numberOfSections <= 1)
			throw(new MapToSmallException(String.format("Map has only %s section.",numberOfSections)));

		numberOfSections--;
		int newWidth = tiles.length - SECTION_WIDTH;
		Tile[][] newTiles = new Tile[newWidth][SECTION_HEIGHT];
		Item[][] newItems = new Item[newWidth][SECTION_HEIGHT];
		for (int x = 0; x < newTiles.length; x++) {
			for (int y = 0; y < newTiles[x].length; y++) {
				newTiles[x][y] = tiles[x][y];
				newItems[x][y] = items[x][y];
			}
		}
		tiles = newTiles;
		items = newItems;
		if (spawn.x > this.numberOfSections * SECTION_WIDTH)
			spawn.x -= SECTION_WIDTH;

		ListIterator<EnemySpawnPointDTO> iter = enemySpawnPoints.listIterator();
		while (iter.hasNext()) {
			if (iter.next().getGridX() >= newWidth) {
				iter.remove();
			}
		}
		
	}

	// FIXME: Duplicate code in MapDTO, thanks weird map creating
	public Map deepCopy() {
		return toMapDTO().toMap();
	}


	public String getName() {
		return name;
	}


	public int getNumberOfSections() {
		return numberOfSections;
	}


	public List<EnemySpawnPointDTO> getEnemySpawnPoints() {
		return enemySpawnPoints;
	}


	public void addEnemySpawnPoint(EnemySpawnPointDTO dto) {
		EnemySpawnPointDTO enemy = getEnemySpawnPoint(dto.getGridX(), dto.getGridY());
		if (enemy != null)
			removeEnemySpawnPoint(enemy);

		enemySpawnPoints.add(dto);
	}


	public void removeEnemySpawnPoint(EnemySpawnPointDTO enemy) {
		lockedEnemies.remove(enemy);
		enemySpawnPoints.remove(enemy);
	}


	public EnemySpawnPointDTO getEnemySpawnPoint(int gridX, int gridY) {
		for (EnemySpawnPointDTO enemy : enemySpawnPoints) {
			if (enemy.getGridX() == gridX && enemy.getGridY() == gridY)
				return enemy;
		}
		return null;
	}


	public EnemySpawnPointDTO getEnemySpawnPoint(String name) {
		for (EnemySpawnPointDTO enemy : enemySpawnPoints) {
			if (enemy.getName().equals(name))
				return enemy;
		}
		return null;
	}


	public boolean requestEnemySpawnLock(int gridX, int gridY) {
		ListIterator<EnemySpawnPointDTO> iter = enemySpawnPoints.listIterator();
		while (iter.hasNext()) {
			EnemySpawnPointDTO enemy = iter.next();
			if (enemy.getGridX() == gridX && enemy.getGridY() == gridY) {
				if (lockedEnemies.contains(enemy)) {
					return false;
				}

				lockedEnemies.add(enemy);
				return true;
			}
		}
		return false;
	}


	public void unlockEnemySpawnPoint(int gridX, int gridY) {
		ListIterator<EnemySpawnPointDTO> iter = lockedEnemies.listIterator();
		while (iter.hasNext()) {
			EnemySpawnPointDTO enemy = iter.next();
			if (enemy.getGridX() == gridX && enemy.getGridY() == gridY) {
				iter.remove();
				return;
			}
		}
	}


	public Tile[][] getTiles() {
		return tiles;
	}


	public Item[][] getItems() {
		return items;
	}


	public Vector2Int getSpawn() {
		return spawn;
	}

}
