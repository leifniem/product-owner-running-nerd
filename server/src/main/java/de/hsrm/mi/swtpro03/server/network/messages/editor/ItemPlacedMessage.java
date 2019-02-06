package de.hsrm.mi.swtpro03.server.network.messages.editor;

import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

public class ItemPlacedMessage {

	public static final String TYPE = "ItemPlacedMessage";

	private ItemDTO itemDTO;

	public ItemPlacedMessage() {
	}

	public ItemPlacedMessage(ItemDTO itemDTO) {
		this.itemDTO = itemDTO;
	}

	public ItemDTO getItemDTO() {
		return itemDTO;
	}

	public void setItemDTO(ItemDTO itemDTO) {
		this.itemDTO = itemDTO;
	}
}
