package de.hsrm.mi.swtpro03.server.network.messages.session;

import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;

import java.util.List;
import java.util.Map;

public class SessionListMessage {

	public static final String TYPE = "SessionListMessage";

	private List<SessionDTO> listOfSessionDTOS;

	public SessionListMessage() {
	}

	public SessionListMessage(List<SessionDTO> listOfSessionDTOS) {
		this.listOfSessionDTOS = listOfSessionDTOS;
	}

	public List<SessionDTO> getListOfSessionDTOS() {
		return listOfSessionDTOS;
	}

	public void setListOfSessionDTOS(List<SessionDTO> listOfSessionDTOS) {
		this.listOfSessionDTOS = listOfSessionDTOS;

	}
}
