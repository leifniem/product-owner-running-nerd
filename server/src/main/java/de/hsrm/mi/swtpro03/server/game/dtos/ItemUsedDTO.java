package de.hsrm.mi.swtpro03.server.game.dtos;

public class ItemUsedDTO {

    private String uuid;
    private String type;

    public ItemUsedDTO() {
    }

    public ItemUsedDTO(String uuid, String type) {
        this.uuid = uuid;
        this.type = type;
    }

    public String getUuid() {
        return uuid;
    }

    public void setUuid(String uuid) {
        this.uuid = uuid;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }
}
