package de.hsrm.mi.swtpro03.server.network.messages.session;

import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;

public class CreateMapMessage {

    public static final String TYPE = "CreateMapMessage";

    private SessionDTO sessionDTO;

    public SessionDTO getSessionDTO() {
        return sessionDTO;
    }

    public void setSessionDTO(SessionDTO sessionDTO) {
        this.sessionDTO = sessionDTO;
    }
}
