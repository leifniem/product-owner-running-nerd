package de.hsrm.mi.swtpro03.server.handler;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.TileDTO;
import de.hsrm.mi.swtpro03.server.game.model.Cursor;
import de.hsrm.mi.swtpro03.server.game.model.Item;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Tile;
import de.hsrm.mi.swtpro03.server.game.session.EditorSession;
import de.hsrm.mi.swtpro03.server.game.session.User;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapToSmallException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.game.session.management.UserManager;
import de.hsrm.mi.swtpro03.server.network.ComChannel;
import de.hsrm.mi.swtpro03.server.network.IMessageListener;
import de.hsrm.mi.swtpro03.server.network.messages.editor.*;
import de.hsrm.mi.swtpro03.server.network.messages.session.KickMessage;
import de.hsrm.mi.swtpro03.server.network.messages.session.PlayerQuitMessage;
import de.hsrm.mi.swtpro03.server.utils.Serializer;

import java.util.ArrayList;
import java.util.UUID;

/**
 * Handles all incomming and outgonging messages of {@link EditorSession}
 * created by: Pascal Niessner (pnies001)
 */
public class EditorSessionMessageHandler implements IMessageListener {

	private EditorSession editorSession;
	private ComChannel sessionChannel;

	public EditorSessionMessageHandler(EditorSession editorSession, ComChannel sessionChannel) {
		this.editorSession = editorSession;
		this.sessionChannel = sessionChannel;
		this.sessionChannel.addMessageListener(this);
	}

	/**
	 * initializes a new EditorSession
	 *
	 * @param mapDTO : map that will be edited
	 */
	public void editorInit(MapDTO mapDTO, String clientId) {
		EditorInitMessage message = new EditorInitMessage(mapDTO);
		sessionChannel.sendToClient(clientId, EditorInitMessage.TYPE, Serializer.serialize(message, EditorInitMessage.class));
	}

	/**
	 * Checks what message came in and triggers the right method
	 */
	@Override
	public void onMessageReceived(String clientID, String messageType, String message) {
		switch (messageType) {
			case TilePlacedMessage.TYPE:
				placeTile((TilePlacedMessage) Serializer.deserialize(message, TilePlacedMessage.class));
				break;
			case TileRemovedMessage.TYPE:
				removeTile((TileRemovedMessage) Serializer.deserialize(message, TileRemovedMessage.class));
				break;
			case ItemPlacedMessage.TYPE:
				placeItem((ItemPlacedMessage) Serializer.deserialize(message, ItemPlacedMessage.class));
				break;
			case ItemRemovedMessage.TYPE:
				removeItem((ItemRemovedMessage) Serializer.deserialize(message, ItemRemovedMessage.class));
				break;
			case CursorMovedMessage.TYPE:
				moveUserCursor((CursorMovedMessage) Serializer.deserialize(message, CursorMovedMessage.class), clientID);
				break;
			case AddSectionMessage.TYPE:
				addSection();
				break;
			case RemoveSectionMessage.TYPE:
				removeSection();
				break;
			case PlayerQuitMessage.TYPE:
				handlePlayerQuit(clientID);
				break;
			case EnemySpawnPointPlacedMessage.TYPE:
				placeEnemySpawnPoint((EnemySpawnPointPlacedMessage) Serializer.deserialize(message, EnemySpawnPointPlacedMessage.class));
				break;
			case EnemySpawnPointRequestLockMessage.TYPE:
				lockEnemySpawnPoint((EnemySpawnPointRequestLockMessage) Serializer.deserialize(message, EnemySpawnPointRequestLockMessage.class), clientID);
				break;
			case EnemySpawnPointUnlockMessage.TYPE:
				unlockEnemySpawnpoint((EnemySpawnPointUnlockMessage) Serializer.deserialize(message, EnemySpawnPointUnlockMessage.class));
				break;
			case SpawnpointReplaceMessage.TYPE:
				replaceSpawnPoint((SpawnpointReplaceMessage) Serializer.deserialize(message, SpawnpointReplaceMessage.class));
				break;
		}
	}

	/**
	 * Handlers Message for Changing the Players spawn in Game.
	 *
	 * @param message that was sended
	 */
	private void replaceSpawnPoint(SpawnpointReplaceMessage message) {
		editorSession.setSpawn(message.getSpawn());
		System.out.println("PLAYER: " + message.getSpawn().x + " " + message.getSpawn().y);
		sessionChannel.sendToAll(SpawnpointReplaceMessage.TYPE, Serializer.serialize(message, SpawnpointReplaceMessage.class));
	}

	/**
	 * Handles Message of a User that quited the Session
	 *
	 * @param clientID Id of User that quited
	 */
	private void handlePlayerQuit(String clientID) {
		if (editorSession.getSessionOwner().getClientID().equals(clientID)) {
			SessionManager.getInstance().removeSession(editorSession.getSessionID());
			editorSession.saveMap();
			editorSession.endSession();
		} else {
			tryToRemoveUser(clientID);
		}
	}


	/**
	 * Tries to remove a User from the current EditorSession
	 *
	 * @param clientID
	 */
	private void tryToRemoveUser(String clientID) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			editorSession.removeUser(user);
			sessionChannel.sendToClient(clientID, KickMessage.TYPE, Serializer.serialize(new PlayerQuitMessage(), PlayerQuitMessage.class));
		} catch (UserNotFoundException e) {
			e.printStackTrace();    // Not critical
		}
	}


	/**
	 * handles Command To remove the last Section
	 */
	private void removeSection() {
		try {
			Map map = editorSession.removeSection();
			RemoveSectionMessage removeSectionsMessage = new RemoveSectionMessage();
			removeSectionsMessage.setSections(map.getNumberOfSections());
			sessionChannel.sendToAll(RemoveSectionMessage.TYPE,
					Serializer.serialize(removeSectionsMessage, RemoveSectionMessage.class));
			sessionChannel.sendToAll(SpawnpointReplaceMessage.TYPE, Serializer.serialize(new SpawnpointReplaceMessage(map.getSpawn()), SpawnpointReplaceMessage.class));
		} catch (MapToSmallException e) {
		}
	}

	/**
	 * handles Command To add a Section to the end of the Map
	 */
	private void addSection() {
		Map map = editorSession.addSection();
		AddSectionMessage addSectionMessage = new AddSectionMessage();
		addSectionMessage.setSections(map.getNumberOfSections());
		sessionChannel.sendToAll(AddSectionMessage.TYPE, Serializer.serialize(addSectionMessage, AddSectionMessage.class));
	}

	/**
	 * sends a message to all users that a new user joined the session
	 * Message for all Contains a list of all Cursors
	 * also Sends a message to the connected User with the Map data
	 *
	 * @param user : user that joined the message
	 */
	public void sendUserJoinedMessage(User user) {
		String color = editorSession.getColorWheel().pickColor().toString();
		Cursor cursor = new Cursor(UUID.randomUUID().toString());
		cursor.setColor(color);
		editorSession.getUserCursor().put(user, cursor);

		EditorSessionJoinedMessage message = new EditorSessionJoinedMessage(new ArrayList<>(editorSession.getUserCursor().values()));
		sessionChannel.sendToAll(EditorSessionJoinedMessage.TYPE, Serializer.serialize(message, EditorSessionJoinedMessage.class));
		sessionChannel.sendToClient(user.getClientID(), CursorIdMessage.TYPE, Serializer.serialize(new CursorIdMessage(cursor.getUuid()), CursorIdMessage.class));
	}

	/**
	 * handles TilePlacedCommand
	 * @param message
	 */
	private void placeTile(TilePlacedMessage message) {
		TileDTO dto = message.getTileDTO();
		try {
			editorSession.addTile(dto.getGridX(), dto.getGridY(), Tile.valueOf(dto.getType()));
			sessionChannel.sendToAll(TilePlacedMessage.TYPE, Serializer.serialize(message, TilePlacedMessage.class));
			EnemySpawnPointDTO enemy = editorSession.getEnemySpawnPoint(dto.getGridX(), dto.getGridY());
			sendEnemySpawnPointRemoveMessage(enemy);
		} catch (IndexOutOfBoundsException e) {
			e.printStackTrace();
		}
	}

	private void sendEnemySpawnPointRemoveMessage(EnemySpawnPointDTO enemy) {
		if (enemy != null) {
			editorSession.removeEnemySpawnPoint(enemy);
			EnemySpawnPointRemovedMessage enemyMessage = new EnemySpawnPointRemovedMessage(enemy);
			sessionChannel.sendToAll(EnemySpawnPointRemovedMessage.TYPE, Serializer.serialize(enemyMessage, EnemySpawnPointRemovedMessage.class));
		}
	}

	/**
	 * handles TileRemoved Command
	 * @param message
	 */
	private void removeTile(TileRemovedMessage message) {
		try {
			editorSession.removeTile(message.getPosX(), message.getPosY());
			sessionChannel.sendToAll(TileRemovedMessage.TYPE, Serializer.serialize(message, TileRemovedMessage.class));

			EnemySpawnPointDTO enemy = editorSession.getEnemySpawnPoint(message.getPosX(), message.getPosY());
			sendEnemySpawnPointRemoveMessage(enemy);
		} catch (IndexOutOfBoundsException e) {
			e.printStackTrace();
		}
	}


	/**
	 * handles ItemPlaced Command
	 *
	 * @param message
	 */
	private void placeItem(ItemPlacedMessage message) {
		ItemDTO dto = message.getItemDTO();
		editorSession.addItem(dto.getGridX(), dto.getGridY(), Item.valueOf(dto.getType()));
		sessionChannel.sendToAll(ItemPlacedMessage.TYPE, Serializer.serialize(message, ItemPlacedMessage.class));
	}

	/**
	 * handles ItemRemoved Command
	 *
	 * @param message
	 */
	private void removeItem(ItemRemovedMessage message) {
		try {
			editorSession.removeItem(message.getPosX(), message.getPosY());
			sessionChannel.sendToAll(ItemRemovedMessage.TYPE, Serializer.serialize(message, ItemRemovedMessage.class));
		} catch (IndexOutOfBoundsException e) {
			e.printStackTrace();
		}
	}

	/**
	 * handles CursorMovedCommand
	 *
	 * @param cursorMovedMessage
	 * @param userID             : user that moved its Cursor
	 */
	private void moveUserCursor(CursorMovedMessage cursorMovedMessage, String userID) {
		Cursor messageCursor = cursorMovedMessage.getCursor();
		try {
			User user = UserManager.getInstance().getUserByClientID(userID);
			Cursor cursor = editorSession.getUserCursor().get(user);
			cursor.setGridX(messageCursor.getGridX());
			cursor.setGridY(messageCursor.getGridY());
			CursorMovedMessage message = new CursorMovedMessage(cursor);
			sessionChannel.sendToAll(CursorMovedMessage.TYPE, Serializer.serialize(message, CursorMovedMessage.class));
		} catch (UserNotFoundException e) {
			e.printStackTrace();
		}
	}

	/**
	 * Handles command that an {@link EnemySpawnPoint} is added to Map
	 *
	 * @param enemySpawnPlaced
	 */
	private void placeEnemySpawnPoint(EnemySpawnPointPlacedMessage enemySpawnPlaced) {
		EnemySpawnPointDTO dto = enemySpawnPlaced.getEnemySpawnDTO();
		if (editorSession.isSolid(dto.getGridX(), dto.getGridY())) {
			editorSession.removeTile(dto.getGridX(), dto.getGridY());
			TileRemovedMessage message = new TileRemovedMessage(dto.getGridX(), dto.getGridY());
			sessionChannel.sendToAll(TileRemovedMessage.TYPE, Serializer.serialize(message, TileRemovedMessage.class));
		}

		EnemySpawnPointDTO enemy = editorSession.getEnemySpawnPoint(dto.getName());
		sendEnemySpawnPointRemoveMessage(enemy);

		editorSession.placeEnemySpawnPoint(dto);
		sessionChannel.sendToAll(EnemySpawnPointPlacedMessage.TYPE, Serializer.serialize(enemySpawnPlaced, EnemySpawnPointPlacedMessage.class));
	}

	private void unlockEnemySpawnpoint(EnemySpawnPointUnlockMessage enemySpawnUnlock) {
		editorSession.unlockEnemySpawnPoint(enemySpawnUnlock.getGridX(), enemySpawnUnlock.getGridY());
	}

	/**
	 * Handles command that an {@link EnemySpawnPoint} is removed from Map
	 *
	 * @param clientID
	 */
	private void lockEnemySpawnPoint(EnemySpawnPointRequestLockMessage requestLockMessage, String clientID) {
		if (editorSession.requestEnemySpawnLock(requestLockMessage.getGridX(), requestLockMessage.getGridY())) {
			EnemySpawnPointDTO enemy = editorSession.getEnemySpawnPoint(requestLockMessage.getGridX(), requestLockMessage.getGridY());
			EnemySpawnPointLockAcceptMessage message = new EnemySpawnPointLockAcceptMessage(enemy);
			sessionChannel.sendToClient(clientID, EnemySpawnPointLockAcceptMessage.TYPE, Serializer.serialize(message, EnemySpawnPointLockAcceptMessage.class));
		} else {
			EnemySpawnPointLockDenyMessage message = new EnemySpawnPointLockDenyMessage("EnemySpawnPoint is already locked or was not found.");
			sessionChannel.sendToClient(clientID, EnemySpawnPointLockDenyMessage.TYPE, Serializer.serialize(message, EnemySpawnPointLockDenyMessage.class));
		}
	}

}
