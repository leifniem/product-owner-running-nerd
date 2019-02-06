package de.hsrm.mi.swtpro03.server.game.session;

import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.management.MapManager;

public class SessionFactory {

	/**
	 * creates a Session
	 *
	 * @param sessionOwner
	 * @param sessionName
	 * @param sessionType
	 * @param mapFileName
	 * @return Session
	 * @throws MapBlockedException
	 */
	public static Session getSession(User sessionOwner, String sessionName, SessionType sessionType, String mapFileName) throws MapBlockedException {
		if (sessionType.equals(SessionType.EDITOR_SESSION)) {
			return new EditorSession(sessionOwner, sessionName, mapFileName);
		}
		if (sessionType.equals(SessionType.GAME_SESSION)) {
			return new GameSession(sessionOwner, sessionName, MapManager.getMapForGameSession(mapFileName));
		}
		return null;
	}
}
