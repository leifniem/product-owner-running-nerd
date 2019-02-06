package de.hsrm.mi.swtpro03.server.network.messages.editor;


import de.hsrm.mi.swtpro03.server.game.dtos.TileDTO;

public class TilePlacedMessage {

	public static final String TYPE = "TilePlacedMessage";

	private TileDTO tileDTO;

	public TilePlacedMessage() {
	}

	public TilePlacedMessage(TileDTO tileDTO) {
		this.tileDTO = tileDTO;
	}

	public TileDTO getTileDTO() {
		return tileDTO;
	}

	public void setTileDTO(TileDTO tileDTO) {
		this.tileDTO = tileDTO;
	}
}
