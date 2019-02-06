package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.session.Session;
import de.hsrm.mi.swtpro03.server.game.session.SessionFactory;
import de.hsrm.mi.swtpro03.server.game.session.SessionType;
import de.hsrm.mi.swtpro03.server.game.session.User;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapNotAvailableException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.SessionNotFoundException;
import de.hsrm.mi.swtpro03.server.handler.SessionManagerMessageHandler;

import java.util.*;

public class SessionManager {

	private static SessionManager instance;

	private Map<UUID, Session> activeSessions = Collections.synchronizedMap(new HashMap<>());

	private SessionManagerMessageHandler messageHandler;

	private SessionManager() {
		this.messageHandler = new SessionManagerMessageHandler(this);
	}

	public static synchronized SessionManager getInstance() {
		if (instance == null) {
			instance = new SessionManager();
		}
		return instance;
	}

	public Session createSession(User user, String sessionName, SessionType sessionType, String mapName) throws MapBlockedException {
		Session newSession = SessionFactory.getSession(user, sessionName, sessionType, mapName);
		activeSessions.put(newSession.getSessionID(), newSession);
		return newSession;
	}

	public Session createPwdProtectedSession(User user, String sessionName, SessionType sessionType, String mapName, String password) throws MapBlockedException {
		Session newSession = createSession(user, sessionName, sessionType, mapName);
		newSession.setPassword(password);
		activeSessions.put(newSession.getSessionID(), newSession);
		return newSession;
	}

	public void removeSession(UUID sessionID) {
		if (!activeSessions.containsKey(sessionID))
			return;

		Session session = activeSessions.get(sessionID);

		if (session.getUsersInSession().size() > 0)
			session.removeAllUsers();

		activeSessions.remove(session.getSessionID());
	}

	public void removeSession(Session session) {
		session.endSession();
		activeSessions.remove(session.getSessionID());
	}

	public void endAllSessions() {
		for (Session session : activeSessions.values()) {
			session.endSession();
		}
		activeSessions.clear();
	}

	public Session getSessionByID(UUID sessionID) throws SessionNotFoundException {
		if (!activeSessions.containsKey(sessionID))
			throw new SessionNotFoundException("Session with this ID doesn't exist");

		return activeSessions.get(sessionID);
	}

	public List<Session> getActiveSessions() {
		return new ArrayList<>(activeSessions.values());
	}

	public void addSession(Session session) {
		activeSessions.put(session.getSessionID(), session);
	}

}
