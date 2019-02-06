package de.hsrm.mi.swtpro03.server.game.session;

import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserIsAlreadyInSessionException;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;

public class User {

	private String clientID;
	private String username;
	private String color;
	private boolean ready = false;
	private Session activeSession = null;

	public User(String clientID, String username) {
		this.clientID = clientID;
		this.username = username;
	}

	/**
	 * Creates a session
	 *
	 * @param sessionDTO - Session to create
	 * @return - Created session
	 * @throws UserIsAlreadyInSessionException
	 * @throws MapBlockedException
	 */
	public Session createSession(SessionDTO sessionDTO) throws UserIsAlreadyInSessionException, MapBlockedException {
		if (activeSession != null)
			throw new UserIsAlreadyInSessionException("User(" + username + ") is already in a session");

		String name = sessionDTO.getName();
		SessionType type = SessionType.GAME_SESSION;
		if (sessionDTO.isEditorSession()) type = SessionType.EDITOR_SESSION;
		String mapName = sessionDTO.getMapMetaDTO().getName();

		activeSession = SessionManager.getInstance().createSession(this, name, type, mapName);
		return activeSession;
	}

	/**
	 * Makes the user join a session in a specified role
	 *
	 * @param session     - Session to join
	 * @param sessionRole - Desired session role
	 * @throws UserIsAlreadyInSessionException thrown if the user is already in a Session
	 */
	public void joinSession(Session session, SessionRole sessionRole) throws UserIsAlreadyInSessionException {
		if (activeSession != null && activeSession != session)
			throw new UserIsAlreadyInSessionException("User(" + username + ") is already in a session");
		if (activeSession != session) {
			session.addUser(this, sessionRole);
		}
		activeSession = session;
	}

	/**
	 * Makes the user leave his active session
	 */
	public void leaveActiveSession() {
		if (activeSession == null)
			return;

		activeSession.removeUser(this);
		activeSession = null;
	}

	public String getClientID() {
		return clientID;
	}

	public String getUsername() {
		return username;
	}

	public boolean isReady() {
		return ready;
	}

	public void setReady(boolean ready) {
		this.ready = ready;
	}

	public String getColor() {
		return color;
	}

	public void setColor(String color) {
		this.color = color;
	}

	public Session getActiveSession() {
		return activeSession;
	}

	void removeActiveSession() {
		this.activeSession = null;
	}

}
