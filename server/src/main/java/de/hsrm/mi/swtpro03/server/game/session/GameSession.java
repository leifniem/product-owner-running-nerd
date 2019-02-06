package de.hsrm.mi.swtpro03.server.game.session;

import de.hsrm.mi.swtpro03.server.game.dtos.EnemySpawnPointDTO;
import de.hsrm.mi.swtpro03.server.game.model.*;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.GameNotReadyException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.SectionException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.handler.GameSessionEventHandler;
import de.hsrm.mi.swtpro03.server.handler.GameSessionMessageHandler;
import de.hsrm.mi.swtpro03.server.network.messages.game.GameEndMessage;

import java.util.Map;
import java.util.*;
import java.util.stream.Collectors;

public class GameSession extends Session {
	private static final int RANKING_AMOUNT = 3;
	private static final String DISPLAY_NAME_KI_UNITS = "Computer Units";
	private static final String CLIENT_ID_KI_UNITS = "0";

	private static String PYTHON_IMPORTS = "from de.hsrm.mi.swtpro03.server.game.model import Tile\r\n" +
			"from de.hsrm.mi.swtpro03.server.game.model import Vector2\n";

	private Map<User, PlayerGameCharacter> userGameCharacterHashMap = Collections.synchronizedMap(new HashMap<>());
	private List<EnemyGameCharacter> enemies = Collections.synchronizedList(new ArrayList<>());

	private Map<Integer, User> mapSectionUserHashMap = Collections.synchronizedMap(new HashMap<>());
	private boolean isRunning = false;
	private GameLoop gameLoop;

	private GameSessionEventHandler gameSessionEventHandler;
	private GameSessionMessageHandler gameMessageHandler;
	private HashMap<String, List<Object>> gameChanges;
	private ColorWheel colorWheel;
	private boolean sessionStarted = false;


	/**
	 * Constructor to initialize GameSession
	 *
	 * @param sessionOwner
	 * @param sessionName
	 * @param map
	 */
	public GameSession(User sessionOwner, String sessionName, de.hsrm.mi.swtpro03.server.game.model.Map map) {
		super(sessionOwner, sessionName, map);
		colorWheel = new ColorWheel();
		gameMessageHandler = new GameSessionMessageHandler(this, sessionChannel);
		prepareGame();
	}

	public boolean isSomeoneTrappedAtTile(int gridX, int gridY) {
		boolean userTrappedInside = userGameCharacterHashMap
				.values()
				.stream()
				.anyMatch(playerGameCharacter -> (int) playerGameCharacter.getCenter().x == gridX && (int) playerGameCharacter.getCenter().y == gridY && playerGameCharacter.isTrapped());

		boolean botTrappedInside = enemies
				.stream()
				.anyMatch(gameCharacter -> (int) gameCharacter.getCenter().x == gridX
						&& (int) gameCharacter.getCenter().y == gridY && gameCharacter.isTrapped());

		return userTrappedInside || botTrappedInside;
	}


	public String getColor() {
		return colorWheel.pickColor().toString();
	}


	/**
	 * Initializes objects necessary for the game to begin
	 */
	private void prepareGame() {
		gameChanges = new HashMap<>();
		gameChanges.put("MoveActorMessages", new LinkedList<>());
		gameChanges.put("LifePointsUpdateMessages", new LinkedList<>());
		gameChanges.put("TileChangedMessages", new LinkedList<>());
		gameChanges.put("ItemPickedUpMessages", new LinkedList<>());
		gameChanges.put("ItemUsedMessages", new LinkedList<>());
		gameChanges.put("GameEndMessages", new LinkedList<>());
		gameChanges.put("CreditPointsUpdateMessages", new LinkedList<>());

		gameSessionEventHandler = new GameSessionEventHandler(this, gameMessageHandler, gameChanges);

		map.addObserver(gameSessionEventHandler);

		createEnemies();
	}


	/**
	 * Checks if the necessary conditions are met to begin game preparation
	 *
	 * @throws GameNotReadyException if no map is selected or not all sections are assigned
	 */
	public void gameReadyCheck() throws GameNotReadyException {
		if (map == null)
			throw new GameNotReadyException("No map selected");
		if (!allMapSectionsAssigned())
			throw new GameNotReadyException("Not all map sections assigned");
	}


	/**
	 * Checks if all map sections are assigned
	 *
	 * @return true if all available map sections were assigned to players
	 */
	private boolean allMapSectionsAssigned() {
		return mapSectionUserHashMap.size() == map.getNumberOfSections();
	}


	/**
	 * Creates a game character for each non-spectating user
	 */
	private void createGameCharacters() {
		for (User user : this.usersInSession.keySet()) {
			if (isSpectator(user)) {
				continue;
			}
			PlayerGameCharacter gameCharacter = new PlayerGameCharacter(this, (int) map.getSpawn().x, (int) map.getSpawn().y, map, new InputState());
			gameCharacter.addObserver(gameSessionEventHandler);
			userGameCharacterHashMap.put(user, gameCharacter);
		}
	}


	private void createEnemies() {
		List<EnemySpawnPointDTO> spawnPoints = this.map.getEnemySpawnPoints();
		for (EnemySpawnPointDTO spawnPoint : spawnPoints) {
			EnemyGameCharacter enemy = new EnemyGameCharacter(
					spawnPoint.getGridX(),
					spawnPoint.getGridY(),
					map,
					new InputState(),
					this,
					PYTHON_IMPORTS + spawnPoint.getCode());
			enemies.add(enemy);
			enemy.addObserver(gameSessionEventHandler);
		}
	}

	/**
	 * Starts a game session
	 */
	public void startSession() {
		if (sessionStarted)
			return;

		sessionStarted = true;
		createGameCharacters();
		gameMessageHandler.gameInit(this.map, userGameCharacterHashMap, enemies);
		this.gameLoop = new GameLoop(this, this.map);
		isRunning = true;
		this.gameLoop.start();
	}


	public void onFrameFinished() {
		gameMessageHandler.flushGameChanges(gameSessionEventHandler.getChanges());
	}


	/**
	 * Returns input state of a specific user
	 *
	 * @param user User you want the input state from
	 * @return input state
	 * @throws UserNotFoundException if the User in not in the session
	 * @see InputState
	 */
	public InputState getInputOfUser(User user) throws UserNotFoundException {
		if (!userGameCharacterHashMap.containsKey(user))
			throw new UserNotFoundException("User(" + user.getUsername() + ") is not in this session");

		return userGameCharacterHashMap.get(user).getInputState();
	}


	/**
	 * Checks if a given user is a spectator
	 *
	 * @param user user to check
	 * @return true if user in question is a spectator
	 */
	private boolean isSpectator(User user) {
		return getRole(user).equals(SessionRole.SPECTATOR);
	}


	/**
	 * Getter for userGameCharacterHashMap
	 *
	 * @return Hashmap of Users and their GameCharacters
	 * @see User
	 * @see GameCharacter
	 */
	public Map<User, PlayerGameCharacter> getUserGameCharacterHashMap() {
		return userGameCharacterHashMap;
	}


	/**
	 * Getter for mapSectionUserHashMap
	 *
	 * @return
	 */
	public Map<Integer, User> getMapSectionUserHashMap() {
		return mapSectionUserHashMap;
	}


	/**
	 * Returns a game character belonging to a specific user
	 *
	 * @param user
	 * @return GameCharacter
	 */
	public GameCharacter getGameCharacterByUser(User user) {
		if (!userGameCharacterHashMap.keySet().contains(user))
			return null;
		return userGameCharacterHashMap.get(user);
	}


	/**
	 * Getter for GameChanges
	 *
	 * @return game changes as HashMap
	 */
	public HashMap<String, List<Object>> getGameChanges() {
		return gameChanges;
	}


	/**
	 * Removes a GameCharacter belonging to a specific user
	 *
	 * @param user - user whose character has to be removed
	 */
	public void removeCharacter(User user) {
		if (userGameCharacterHashMap.keySet().contains(user))
			userGameCharacterHashMap.remove(user);
	}


	/**
	 * Checks if a specific map section is available
	 *
	 * @param section map section
	 * @return true if the map section is currently available
	 */
	private boolean sectionIsAvailable(int section) {
		boolean isNotTaken = !mapSectionUserHashMap.keySet().contains(section);
		boolean isValidSection = section > 0 && section <= map.getNumberOfSections();
		return isNotTaken && isValidSection;
	}


	/**
	 * Assigns a map section to a specific user
	 *
	 * @param assignToUser    user that wants to select a map section
	 * @param sectionToAssign map section that has to be selected
	 */
	public void assignSection(User assignToUser, int sectionToAssign) throws SectionException {
		if (isSpectator(assignToUser) || !sectionIsAvailable(sectionToAssign))
			throw new SectionException("Can't assign Section " + sectionToAssign + "to " + assignToUser.getUsername());

		if (mapSectionUserHashMap.containsValue(assignToUser)) {
			removeUserEntry(assignToUser);
		}
		mapSectionUserHashMap.put(sectionToAssign, assignToUser);
	}


	private void removeUserEntry(User user) {
		int entryKey = getUserEntryKey(user);
		mapSectionUserHashMap.remove(entryKey);
	}


	private int getUserEntryKey(User user) {
		for (Map.Entry<Integer, User> entry : mapSectionUserHashMap.entrySet()) {
			String username = entry.getValue().getUsername();
			if (username.equals(user.getUsername())) {
				return entry.getKey();
			}
		}
		return -1; // invalid key, util.Map does not care, nor should it occur
	}

	/**
	 * Cancels section selection of a specific user
	 *
	 * @param user    user that wants to cancel their section selection
	 * @param section map section that has to be unselected
	 * @return true in case of success
	 */
	public boolean cancelSectionSelection(User user, int section) {
		return mapSectionUserHashMap.remove(section, user);
	}

	private Map<String, Integer> getTop3Players() {
		HashMap<String, Integer> top3PlayerMap = new HashMap<>();

		List<PlayerGameCharacter> playerGameCharacters = sortGameCharactersByCP();
		playerGameCharacters = getThe3CharactersWithHighestScores(playerGameCharacters);

		for (PlayerGameCharacter character : playerGameCharacters) {
			top3PlayerMap.put(getUsernameByGameCharacter(character), character.getScore());
		}

		return sortTop3PlayerMap(top3PlayerMap);
	}


	/**
	 * @param top3PlayerMap the unsorted map of the top 3 players
	 * @return returns the sorted hashmap by value (by creditpoints), descending
	 */
	private LinkedHashMap<String, Integer> sortTop3PlayerMap(HashMap<String, Integer> top3PlayerMap) {
		return top3PlayerMap.entrySet()
				.stream()
				.sorted(
						Collections.reverseOrder(
								Map.Entry.comparingByValue()
						)
				)
				.collect(
						Collectors.toMap(
								Map.Entry::getKey,
								Map.Entry::getValue,
								(e1, e2) -> e2,
								LinkedHashMap::new
						)
				);
	}


	/**
	 * @param playersSortedByCreditPoints the list of players, where the best 3 players (sorted by credit points) will be returned
	 * @return returns a list of the top 3 characters
	 */
	private List<PlayerGameCharacter> getThe3CharactersWithHighestScores(List<PlayerGameCharacter> playersSortedByCreditPoints) {
		int from = playersSortedByCreditPoints.size() < RANKING_AMOUNT ? 0 : playersSortedByCreditPoints.size() - RANKING_AMOUNT;
		int to = playersSortedByCreditPoints.size();

		return playersSortedByCreditPoints.subList(from, to);
	}


	/**
	 * @return returns the game characters sorted by their creditpoints (top to low)
	 */
	private List<PlayerGameCharacter> sortGameCharactersByCP() {
		return userGameCharacterHashMap.values()
				.stream()
				.sorted(Comparator.comparing(PlayerGameCharacter::getScore))
				.collect(Collectors.toList());
	}

	/**
	 * @param username the username of the player, which color you want
	 * @return returns the color of the given username
	 */
	private String getColorByUsername(String username) {
		for (Map.Entry<User, PlayerGameCharacter> entry : userGameCharacterHashMap.entrySet()) {
			if (entry.getKey().getUsername().equals(username)) {
				return entry.getKey().getColor();
			}
		}

		return null;
	}


	/**
	 * @param searchedGameCharacter the GameCharacter of which you want the username to get
	 * @return returns the username of the given GameCharacter
	 * @see GameCharacter
	 */
	private String getUsernameByGameCharacter(GameCharacter searchedGameCharacter) {
		for (Map.Entry<User, PlayerGameCharacter> entry : userGameCharacterHashMap.entrySet()) {
			if (entry.getValue() == searchedGameCharacter) {
				return entry.getKey().getUsername();
			}
		}

		return null;
	}

	/**
	 * checks if one of the game ending contitions is triggered and handles the game end if so
	 */
	public void checkIfGameEnded() {
		if (everyCharacterIsDead()) {
			endGameByLoss();
		} else if (!areCreditPointsLeft()) {
			endGameByWin();
		}
	}
	
	public void forceEndGame() {
		endGame(GameEndMessage.QUIT_STATUS);
	}

	private void endGameByLoss() {
		endGame(GameEndMessage.LOSS_STATUS);
	}

	private void endGameByWin() {
		endGame(GameEndMessage.WIN_STATUS);
	}

	private void endGame(String status) {
		isRunning = false;
		Map<String, Integer> top3Players = getTop3Players();
		gameSessionEventHandler.startGameEnding(new GameEndMessage(
				top3Players,
				status,
				getColorByUsername(top3Players.keySet().iterator().next())));
		gameLoop.interrupt();
		onFrameFinished();
		SessionManager.getInstance().removeSession(this);
	}


	/**
	 * @return true if every game character is dead
	 */
	private boolean everyCharacterIsDead() {
		return userGameCharacterHashMap.values()
				.stream()
				.allMatch(gameCharacter -> gameCharacter.getLifePoints() == 0);
	}


	/**
	 * @return true if there are still credit points on the map
	 */
	private boolean areCreditPointsLeft() {
		return Arrays.stream(map.getItems())
				.flatMap(Arrays::stream)
				.anyMatch(
						Item::isCreditpoints
				);
	}

	public List<EnemyGameCharacter> getEnemies() {
		return enemies;
	}

	public boolean isRunning() {
		return isRunning;
	}
	
}
