package de.hsrm.mi.swtpro03.server.network.messages.session;

import de.hsrm.mi.swtpro03.server.game.session.SessionRole;

public class JoinSessionMessage {

	public static final String TYPE = "JoinSessionMessage";

	private String sessionID;
	private SessionRole sessionRole;

	public JoinSessionMessage() {
	}

	public JoinSessionMessage(String sessionID, SessionRole sessionRole) {
		this.sessionID = sessionID;
		this.sessionRole = sessionRole;
	}

	public String getSessionID() {
		return sessionID;
	}

	public void setSessionID(String sessionID) {
		this.sessionID = sessionID;
	}

	public SessionRole getSessionRole() {
		return sessionRole;
	}

	public void setSessionRole(SessionRole sessionRole) {
		this.sessionRole = sessionRole;
	}
}
