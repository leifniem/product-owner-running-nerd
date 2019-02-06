package de.hsrm.mi.swtpro03.server.network.messages.game;

import de.hsrm.mi.swtpro03.server.game.dtos.ItemDTO;

public class ItemPickedUpMessage {

    private String uuid;
    private ItemDTO itemDTO;

    public ItemPickedUpMessage() {
    }

    public ItemPickedUpMessage(String uuid, ItemDTO itemDTO) {
        this.uuid = uuid;
        this.itemDTO = itemDTO;
    }

    public String getUuid() {
        return uuid;
    }

    public void setUuid(String uuid) {
        this.uuid = uuid;
    }

    public ItemDTO getItemDTO() {
        return itemDTO;
    }

    public void setItemDTO(ItemDTO itemDTO) {
        this.itemDTO = itemDTO;
    }
}
