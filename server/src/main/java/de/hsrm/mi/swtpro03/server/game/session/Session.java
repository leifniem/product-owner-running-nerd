package de.hsrm.mi.swtpro03.server.game.session;

import de.hsrm.mi.swtpro03.server.game.dtos.MapMetaDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.session.management.MapManager;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.network.ComChannel;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import de.hsrm.mi.swtpro03.server.network.messages.session.KickMessage;
import de.hsrm.mi.swtpro03.server.network.messages.session.PlayerQuitMessage;
import de.hsrm.mi.swtpro03.server.utils.Serializer;

import java.util.HashMap;
import java.util.UUID;

public abstract class Session {
	protected UUID sessionID;
	protected String name;
	protected String password;
	protected Map map;
	protected HashMap<User, SessionRole> usersInSession = new HashMap<>();
	protected ComChannel sessionChannel;

	public Session(User sessionOwner, String sessionName, Map map) {
		this.sessionID = UUID.randomUUID();
		this.name = sessionName;
		this.sessionChannel = NetworkService.createComChannel(sessionID.toString());
		usersInSession.put(sessionOwner, SessionRole.SESSION_OWNER);

		setMap(map);
	}

	public Session(String sessionName, User sessionOwner) {
		this.sessionID = UUID.randomUUID();
		this.name = sessionName;
		this.sessionChannel = NetworkService.createComChannel(sessionID.toString());
		usersInSession.put(sessionOwner, SessionRole.SESSION_OWNER);
	}

	/**
	 * Adds a user to the session with a specified role
	 *
	 * @param user        - User to add
	 * @param sessionRole - User's session role
	 */
	public void addUser(User user, SessionRole sessionRole) {
		if (sessionRole == SessionRole.SESSION_OWNER)
			throw new IllegalArgumentException("[Session] nice try, session thief.");

		usersInSession.put(user, sessionRole);
	}

	/**
	 * Removes a specific user from current session and notifies remaining users
	 *
	 * @param user - User that left current session
	 */
	public void removeUser(User user) {
		if (!usersInSession.containsKey(user))
			return;

		usersInSession.remove(user);
		user.setReady(false);
		if (user.getActiveSession() == this) {
			user.removeActiveSession();
		}

		if (usersInSession.size() == 0 || usersInSession.get(user) == SessionRole.SESSION_OWNER) {
			SessionManager.getInstance().removeSession(this);
		} else {
			PlayerQuitMessage playerQuitMessage = preparePlayerQuitMessage(user);
			sessionChannel.sendToAll(PlayerQuitMessage.TYPE, Serializer.serialize(playerQuitMessage, PlayerQuitMessage.class));

			if (this instanceof GameSession)
				((GameSession) this).removeCharacter(user);
			if (this instanceof EditorSession)
				((EditorSession) this).removeCursor(user);
		}
	}

	/**
	 * Prepares a PlayerQuitMessage for a specific user
	 *
	 * @param user - User that left current session
	 * @return PlayerQuitMessage
	 */
	private PlayerQuitMessage preparePlayerQuitMessage(User user) {
		PlayerQuitMessage playerQuitMessage = new PlayerQuitMessage();
		playerQuitMessage.setUsername(user.getUsername());
		String uuid = "";
		if (this instanceof GameSession)
			// Maybe the GameSession isn't started yet so we dont have any characters in the Session yet
			try {
				uuid = ((GameSession) this).getGameCharacterByUser(user).getUuid().toString();
			}catch(NullPointerException e){
				uuid = "Not yet started";
			}
		if (this instanceof EditorSession)
			uuid = ((EditorSession) this).getCursorByUser(user).getUuid();
		playerQuitMessage.setUuid(uuid);
		return playerQuitMessage;
	}

	/**
	 * Removes all users in current session
	 */
	public void removeAllUsers() {
		sessionChannel.sendToAll(KickMessage.TYPE, "");
		for (User u : usersInSession.keySet()) {
			if (u.getActiveSession() == this) {
				u.removeActiveSession();
				u.setReady(false);
			}
		}
		usersInSession = new HashMap<>();
	}

	/**
	 * Ends current session
	 */
	public void endSession() {
		removeAllUsers();
		MapManager.release(map.getName());
	}

	public UUID getSessionID() {
		return this.sessionID;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public SessionRole getRole(User user) {
		return usersInSession.get(user);
	}

	public HashMap<User, SessionRole> getUsersInSession() {
		return usersInSession;
	}

	public String getPassword() {
		return password;
	}

	public void setPassword(String password) {
		this.password = password;
	}

	public void setMap(Map map) {
		this.map = map;
	}

	public Map getMap() {
		return this.map;
	}

	public MapMetaDTO getMapMeta() {
		return this.map.toMapDTO().getMeta();
	}

	/**
	 * Creates a SessionDTO with MetaData from the session
	 *
	 * @return SessionDTO
	 */
	public SessionDTO toSessionDTO() {
		SessionDTO sessionDTO = new SessionDTO();
		sessionDTO.setUsers(usersInSession.size());
		sessionDTO.setMapMetaDTO(getMapMeta());
		sessionDTO.setName(getName());
		sessionDTO.setId(getSessionID().toString());
		if (this instanceof GameSession) {
			sessionDTO.setGameSession(true);
		} else if (this instanceof EditorSession) {
			sessionDTO.setEditorSession(true);
		}
		sessionDTO.setMinUser(getMapMeta().numberOfSections);
		return sessionDTO;
	}

	/**
	 * Returns a user that is the session owner of the session
	 *
	 * @return - Session owner
	 */
	public User getSessionOwner() {
		for (User user : this.usersInSession.keySet()) {
			if (usersInSession.get(user) == SessionRole.SESSION_OWNER) {
				return user;
			}
		}
		return null;
	}

	public boolean isSessionOwner(String clientID){
		return getSessionOwner().getClientID().equals(clientID);
	}
}
