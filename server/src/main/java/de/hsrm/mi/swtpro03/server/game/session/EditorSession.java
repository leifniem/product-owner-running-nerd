package de.hsrm.mi.swtpro03.server.game.session;

import de.hsrm.mi.swtpro03.server.game.model.*;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.management.MapManager;
import de.hsrm.mi.swtpro03.server.handler.EditorSessionMessageHandler;
import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

import java.io.IOException;
import java.util.HashMap;

public class EditorSession extends Session {
	private java.util.Map<User, Cursor> userCursor;
	private boolean isUserConnected;

	private EditorSessionMessageHandler editorMessageHandler;
	private ColorWheel colorWheel = new ColorWheel();


	/**
	 * Constructor for an EditorSession
	 *
	 * @param sessionOwner User who created this Session in his Client
	 * @param sessionName  Display name of the Session, given by User in his Client
	 * @param mapFileName  FileName of the Map the user chose to edit
	 * @throws MapBlockedException thrown if the map is already used in an EditorSession
	 */
	EditorSession(User sessionOwner, String sessionName, String mapFileName) throws MapBlockedException {
		super(sessionName, sessionOwner);
		userCursor = new HashMap<>();
		isUserConnected = false;
		setMap(MapManager.getMapForEditorSession(mapFileName));
		saveMap();
	}

	/**
	 * Removes the cursor of a specific user
	 *
	 * @param user - user whose cursor has to be removed
	 */
	void removeCursor(User user) {
		if (userCursor.keySet().contains(user))
			userCursor.remove(user);
	}

	/**
	 * Returns the cursor belonging to a specific user
	 *
	 * @param user
	 * @return Cursor
	 */
	Cursor getCursorByUser(User user) {
		return userCursor.get(user);
	}

	/**
	 * starts the new Session when the first user connects. Is also called when new User joins in Message
	 *
	 * @param user : user that joined the message
	 */
	public void startEditing(User user) {
		if (!isUserConnected) {
			isUserConnected = true;
			editorMessageHandler = new EditorSessionMessageHandler(this, sessionChannel);
		}
		editorMessageHandler.editorInit(map.toMapDTO(), user.getClientID());
		editorMessageHandler.sendUserJoinedMessage(user);
	}

	/**
	 * removes tiles at given Coordinates
	 *
	 * @param x : X-Coordinate
	 * @param y : Y-Coordinate
	 */
	public void removeTile(int x, int y) {
		map.removeTile(x, y);
	}

	/**
	 * removes Items at given Coordinates
	 *
	 * @param x : X-Coordinate
	 * @param y : Y-Coordinate
	 */
	public void removeItem(int x, int y) {
		map.removeItem(x, y);
	}

	/**
	 * adds A new Tile to Map
	 */
	public void addTile(int gridX, int gridY, Tile tile) throws IndexOutOfBoundsException {
		map.addTile(gridX, gridY, tile);
	}

	/**
	 * adds a new Item to Map at given position
	 * @param gridX X position of placed item
	 * @param gridY Y position of placed item
	 * @param item Item to be added
	 */
	public void addItem(int gridX, int gridY, Item item) {
		map.addItem(gridX, gridY, item);
	}

	/**
	 * adds a section to current map
	 * @return map after section is added
	 */
	public Map addSection() {
		map.addSection();
		return map;
	}

	/**
	 * saves the current state of the map
	 */
	public void saveMap() {
		try {
			MapManager.saveMap(this.map);
		} catch (IOException e) {
			e.printStackTrace();
		}
	}

	public ColorWheel getColorWheel() {
		return colorWheel;
	}

	public java.util.Map<User, Cursor> getUserCursor() {
		return userCursor;
	}

	/**
	 * removes a section from current map
	 * @return map after section removed
	 */
	public Map removeSection() {
		map.removeSection();
		return map;
	}

	public boolean isSolid(int gridX, int gridY) {
		return map.getTile(gridX, gridY).isSolid();
	}

	public void placeEnemySpawnPoint(EnemySpawnPointDTO dto) {
		map.addEnemySpawnPoint(dto);
	}

	public void removeEnemySpawnPoint(EnemySpawnPointDTO enemy) {
		map.removeEnemySpawnPoint(enemy);
	}

	public EnemySpawnPointDTO getEnemySpawnPoint(int gridX, int gridY) {
		return map.getEnemySpawnPoint(gridX, gridY);
	}

	public EnemySpawnPointDTO getEnemySpawnPoint(String name) {
		return map.getEnemySpawnPoint(name);
	}

	public boolean requestEnemySpawnLock(int gridX, int gridY) {
		return map.requestEnemySpawnLock(gridX, gridY);
	}

	public void unlockEnemySpawnPoint(int gridX, int gridY) {
		map.unlockEnemySpawnPoint(gridX, gridY);
	}

	/**
	 * changes current Player Spawnpoint to given vector
	 * @param spawn new X and Y Postition of Spawnpoint
	 */
	public void setSpawn(Vector2Int spawn) {
		this.map.getSpawn().x = spawn.x;
		this.map.getSpawn().y = spawn.y;
	}
}
