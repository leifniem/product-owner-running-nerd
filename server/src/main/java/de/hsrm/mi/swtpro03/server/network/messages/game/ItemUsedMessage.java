package de.hsrm.mi.swtpro03.server.network.messages.game;

public class ItemUsedMessage {

    private String uuid;
    private String type;

    public ItemUsedMessage() {
    }

    public ItemUsedMessage(String uuid, String type) {
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
