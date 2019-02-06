package de.hsrm.mi.swtpro03.server.network.messages.session;

import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;

public class CreateSessionMessage {

	public static final String TYPE = "CreateSessionMessage";
	private String sessionName;
	private SessionDTO sessionDTO;

	public CreateSessionMessage() {
	}

	public CreateSessionMessage(SessionDTO sessionDTO) {
		this.sessionDTO = sessionDTO;
	}

	public SessionDTO getSessionDTO() {
		return sessionDTO;
	}

	public void setSessionDTO(SessionDTO sessionDTO) {
		this.sessionDTO = sessionDTO;
	}
}
