package de.hsrm.mi.swtpro03.server.network.messages.game;

import de.hsrm.mi.swtpro03.server.game.dtos.GameCharacterDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;

import java.util.List;

public class GameInitMessage {

	public static final String TYPE = "GameInitMessage";

	private MapDTO mapDTO;
	private int minX;
	private int maxX;
	private List<GameCharacterDTO> PlayerDTOs;

	public GameInitMessage() {
	}

	public GameInitMessage(MapDTO mapDTO, int minX, int maxX, List<GameCharacterDTO> gameCharacterDTOS) {
		this.mapDTO = mapDTO;
		this.minX = minX;
		this.maxX = maxX;
		this.PlayerDTOs = gameCharacterDTOS;
	}

	public MapDTO getMapDTO() {
		return mapDTO;
	}

	public void setMapDTO(MapDTO mapDTO) {
		this.mapDTO = mapDTO;
	}

	public int getMinX() {
		return minX;
	}

	public void setMinX(int minX) {
		this.minX = minX;
	}

	public int getMaxX() {
		return maxX;
	}

	public void setMaxX(int maxX) {
		this.maxX = maxX;
	}

	public List<GameCharacterDTO> getPlayerDTOs() {
		return PlayerDTOs;
	}

	public void setPlayerDTOs(List<GameCharacterDTO> gameCharacterDTOList) {
		this.PlayerDTOs = gameCharacterDTOList;
	}
}
