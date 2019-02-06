package de.hsrm.mi.swtpro03.server.network.messages.game;

public class CreditPointsUpdateMessage {

    public static final String TYPE = "CreditPointsUpdateMessage";

    private String uuid;
    private int creditPoints;

    public CreditPointsUpdateMessage() {
    }

    public CreditPointsUpdateMessage(String uuid, int creditPoints) {
        this.uuid = uuid;
        this.creditPoints = creditPoints;
    }

    public String getUuid() {
        return uuid;
    }

    public void setUuid(String uuid) {
        this.uuid = uuid;
    }

    public int getCreditPoints() {
        return creditPoints;
    }

    public void setCreditPoints(int creditPoints) {
        this.creditPoints = creditPoints;
    }
}
