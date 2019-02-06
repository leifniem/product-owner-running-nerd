package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;

public class EditorInitMessage {

	public static final String TYPE = "EditorInitMessage";
	private MapDTO mapDTO;

	public EditorInitMessage() {
	}

	public EditorInitMessage(MapDTO mapDTO) {
		this.mapDTO = mapDTO;
	}

	public MapDTO getMapDTO() {
		return mapDTO;
	}

	public void setMapDTO(MapDTO mapDTO) {
		this.mapDTO = mapDTO;
	}
}
