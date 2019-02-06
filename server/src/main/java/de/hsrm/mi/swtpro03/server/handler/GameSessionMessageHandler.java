package de.hsrm.mi.swtpro03.server.handler;

import de.hsrm.mi.swtpro03.server.game.dtos.GameCharacterDTO;
import de.hsrm.mi.swtpro03.server.game.model.EnemyGameCharacter;
import de.hsrm.mi.swtpro03.server.game.model.InputState;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.PlayerGameCharacter;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;
import de.hsrm.mi.swtpro03.server.game.session.SessionRole;
import de.hsrm.mi.swtpro03.server.game.session.SpriteColor;
import de.hsrm.mi.swtpro03.server.game.session.User;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.GameNotReadyException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.SectionException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.game.session.management.UserManager;
import de.hsrm.mi.swtpro03.server.network.ComChannel;
import de.hsrm.mi.swtpro03.server.network.IMessageListener;
import de.hsrm.mi.swtpro03.server.network.messages.game.GameInitMessage;
import de.hsrm.mi.swtpro03.server.network.messages.game.PlayerCommandMessage;
import de.hsrm.mi.swtpro03.server.network.messages.game.PlayerReadyMessage;
import de.hsrm.mi.swtpro03.server.network.messages.game.TickUpdateMessage;
import de.hsrm.mi.swtpro03.server.network.messages.session.*;
import de.hsrm.mi.swtpro03.server.utils.Serializer;

import java.util.ArrayList;
import java.util.Collection;
import java.util.HashMap;
import java.util.List;

public class GameSessionMessageHandler implements IMessageListener {

	private static final int TESTING_MIN_X = 0;
	private static final int TESTING_MAX_X = 1920;
	private GameSession session;
	private ComChannel sessionChannel;

	public GameSessionMessageHandler(GameSession session, ComChannel sessionChannel) {
		this.session = session;
		this.sessionChannel = sessionChannel;
		this.sessionChannel.addMessageListener(this);
	}

	/**
	 * Generates a GameInitMessage and sends it to everyone in current session
	 *
	 * @param map        - Map for current game session
	 * @param characters - All characters including AI enemies
	 */
	public void gameInit(Map map, java.util.Map<User, PlayerGameCharacter> characters, List<EnemyGameCharacter> enemies) {
		ArrayList<GameCharacterDTO> characterDTOs = new ArrayList<>();
		characters.forEach((k, v) -> {
			GameCharacterDTO gameCharacterDTO = v.toGameCharacterDTO();
			gameCharacterDTO.setColor(k.getColor());
			characterDTOs.add(gameCharacterDTO);
		});
		enemies.forEach(enemy -> {
			GameCharacterDTO gameCharacterDTO = enemy.toGameCharacterDTO();
			gameCharacterDTO.setColor(SpriteColor.RED.toString());
			characterDTOs.add(gameCharacterDTO);
		});
		GameInitMessage gameInitMessage = new GameInitMessage(map.toMapDTO(), TESTING_MIN_X, TESTING_MAX_X, characterDTOs);
		sessionChannel.sendToAll(GameInitMessage.TYPE, Serializer.serialize(gameInitMessage, GameInitMessage.class));
	}

	@Override
	public void onMessageReceived(String clientID, String messageType, String message) {
		switch (messageType) {
			case SelectSectionMessage.TYPE:
				selectSection(clientID, (SelectSectionMessage) Serializer.deserialize(message, SelectSectionMessage.class));
				break;
			case DeselectSectionMessage.TYPE:
				deselectSection(clientID, (DeselectSectionMessage) Serializer.deserialize(message, DeselectSectionMessage.class));
				break;
			case PlayerCommandMessage.TYPE:
				handlePlayerCommand(clientID, (PlayerCommandMessage) Serializer.deserialize(message, PlayerCommandMessage.class));
				break;
			case StartGameMessage.TYPE:
				startGame(clientID, (StartGameMessage) Serializer.deserialize(message, StartGameMessage.class));
				break;
			case PlayerQuitMessage.TYPE:
				handlePlayerQuit(clientID);
				break;
			case PlayerReadyMessage.TYPE:
				handlePlayerReady(clientID);
		}
	}

	/**
	 * Message handling for SelectSectionMessage
	 * Attempts to assign a desired section to a user
	 *
	 * @param clientID
	 * @param selectSectionMessage
	 */
	private void selectSection(String clientID, SelectSectionMessage selectSectionMessage) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);

			session.assignSection(user, selectSectionMessage.getSection());
			sessionChannel.sendToAll(
					SelectSectionMessage.TYPE,
					Serializer.serialize(new SelectSectionMessage(getSectionMap()), SelectSectionMessage.class)
			);
		} catch (UserNotFoundException e) {
			DenySelectSectionMessage denySelectSectionMessage = new DenySelectSectionMessage("User not found");
			sessionChannel.sendToClient(clientID, DenySelectSectionMessage.TYPE, Serializer.serialize(denySelectSectionMessage, DenySelectSectionMessage.class));
			e.printStackTrace();
		} catch (SectionException e) {
			DenySelectSectionMessage denySelectSectionMessage = new DenySelectSectionMessage("Section not available");
			sessionChannel.sendToClient(clientID, DenySelectSectionMessage.TYPE, Serializer.serialize(denySelectSectionMessage, DenySelectSectionMessage.class));
			e.printStackTrace();
		}
	}

	/**
	 * Message handling for DeselectSectionMessage
	 * Attemts to deselect a section
	 *
	 * @param clientID
	 * @param deselectSectionMessage
	 */
	private void deselectSection(String clientID, DeselectSectionMessage deselectSectionMessage) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);

			if (session.cancelSectionSelection(user, deselectSectionMessage.getSection()))
				sessionChannel.sendToAll(SelectSectionMessage.TYPE, Serializer.serialize(new SelectSectionMessage(getSectionMap()), SelectSectionMessage.class));
			else {
				DenyDeselectSectionMessage denyDeselectSectionMessage = new DenyDeselectSectionMessage();
				sessionChannel.sendToClient(clientID, DenyDeselectSectionMessage.TYPE, Serializer.serialize(denyDeselectSectionMessage, DenyDeselectSectionMessage.class));
			}
		} catch (UserNotFoundException e) {
			DenyDeselectSectionMessage denyDeselectSectionMessage = new DenyDeselectSectionMessage("User not found");
			sessionChannel.sendToClient(clientID, DenyDeselectSectionMessage.TYPE, Serializer.serialize(denyDeselectSectionMessage, DenyDeselectSectionMessage.class));
			e.printStackTrace();
		}
	}

	/**
	 * Message handling for StartGameMessage
	 * Attempts to start current game session
	 *
	 * @param clientID
	 * @param startGameMessage
	 */
	private void startGame(String clientID, StartGameMessage startGameMessage) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);

			if (session.getRole(user) != SessionRole.SESSION_OWNER) {
				DenyStartGameMessage denyStartGameMessage = new DenyStartGameMessage("You are not the owner of this session");
				sessionChannel.sendToClient(clientID, DenyStartGameMessage.TYPE, Serializer.serialize(denyStartGameMessage, DenyStartGameMessage.class));
				return;
			}
			session.gameReadyCheck();
			sessionChannel.sendToAll(StartGameMessage.TYPE, Serializer.serialize(startGameMessage, StartGameMessage.class));
		} catch (UserNotFoundException e) {
			e.printStackTrace();
		} catch (GameNotReadyException e) {
			DenyStartGameMessage denyStartGameMessage = new DenyStartGameMessage(e.getMessage());
			sessionChannel.sendToClient(clientID, DenyStartGameMessage.TYPE, Serializer.serialize(denyStartGameMessage, DenyStartGameMessage.class));
		}
	}

	/**
	 * Message handling for PlayerCommandMessage
	 * Processes player input
	 *
	 * @param clientID
	 * @param playerCommandMessage
	 */
	private void handlePlayerCommand(String clientID, PlayerCommandMessage playerCommandMessage) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			InputState input = session.getInputOfUser(user);
			setUserInputState(playerCommandMessage, input);
		} catch (UserNotFoundException e) {
			e.printStackTrace();
		}
	}

	private void setUserInputState(PlayerCommandMessage playerCommandMessage, InputState input) {
		switch (playerCommandMessage.getPressedKey()) {
			case LEFT:
				input.setLeftKeyPressed(!input.isLeftKeyPressed());
				break;
			case RIGHT:
				input.setRightKeyPressed(!input.isRightKeyPressed());
				break;
			case UP:
				input.setUpKeyPressed(!input.isUpKeyPressed());
				break;
			case DOWN:
				input.setDownKeyPressed(!input.isDownKeyPressed());
				break;
			case JUMP:
				input.setJumpKeyPressed(true);
				break;
			case DIG_LEFT:
				input.setDigLeftKeyPressed(true);
				break;
			case DIG_RIGHT:
				input.setDigRightKeyPressed(true);
				break;
			case ENERGY:
				input.setEnergyDrinkKeyPressed(true);
				break;
		}
	}

	private void handlePlayerQuit(String clientID) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			
			if (session.isSessionOwner(clientID) || session.getMapSectionUserHashMap().containsValue(user)) {
				if(session.isRunning()){
					session.forceEndGame();
				}else{
					if(session.isSessionOwner(clientID)){
					SessionManager.getInstance().removeSession(session.getSessionID());
					session.removeAllUsers();}
				}
			} else {
				removeUserFromSession(clientID);
			}
		} catch (UserNotFoundException e) {
			e.printStackTrace();
		}	
	}

	private void removeUserFromSession(String clientID) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			session.removeUser(user);
			sessionChannel.sendToClient(clientID, KickMessage.TYPE, Serializer.serialize(new PlayerQuitMessage(), PlayerQuitMessage.class));
		} catch (UserNotFoundException e) {
			e.printStackTrace();
		}
	}

	private void handlePlayerReady(String clientID) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			user.setReady(true);
		} catch (UserNotFoundException e) {
			e.printStackTrace();
			return;
		}
		startSessionIfEverythingIsReady();
	}

	private void startSessionIfEverythingIsReady() {
		if (isEverybodyReady()) {
			session.startSession();
		}
	}

	private boolean isEverybodyReady() {
		Collection<User> usersInSession = session.getMapSectionUserHashMap().values();
		return usersInSession.stream().allMatch(User::isReady);
	}

	/**
	 * Sends a TickUpdateMessage with all game changes
	 * since last frame to all users
	 *
	 * @param gameChanges
	 */
	public void flushGameChanges(HashMap<String, List<Object>> gameChanges) {
		TickUpdateMessage message = new TickUpdateMessage(gameChanges);
		sessionChannel.sendToAll(TickUpdateMessage.TYPE, Serializer.serialize(message, TickUpdateMessage.class));
		gameChanges.values().forEach(List::clear);
	}

	private HashMap<Integer, List<String>> getSectionMap() {
		HashMap<Integer, List<String>> sections = new HashMap<>();
		session.getMapSectionUserHashMap().forEach((k, v) -> {
			List<String> list = new ArrayList<>();
			list.add(v.getUsername());
			list.add(v.getColor());
			sections.put(k, list);
		});
		return sections;
	}
}
