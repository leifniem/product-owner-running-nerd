package de.hsrm.mi.swtpro03.server.game.dtos;

public class CreditPointsDTO {

    private String uuid;
    private int creditPoints;

    public CreditPointsDTO() {
    }

    public CreditPointsDTO(String uuid, int creditPoints) {
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
