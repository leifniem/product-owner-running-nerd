package de.hsrm.mi.swtpro03.server.network.messages.game;

public class TileChangedMessage {
    public static final String TYPE = "TileChangedMessage";

    public static final String DESTROYED_STATE = "DESTROYED";
    public static final String RESTORED_STATE = "RESTORED";

    private String state;
    private int gridX;
    private int gridY;

    public TileChangedMessage() {
    }

    public TileChangedMessage(String state, int gridX, int gridY) {
        this.state = state;
        this.gridX = gridX;
        this.gridY = gridY;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public int getGridX() {
        return gridX;
    }

    public void setGridX(int gridX) {
        this.gridX = gridX;
    }

    public int getGridY() {
        return gridY;
    }

    public void setGridY(int gridY) {
        this.gridY = gridY;
    }
}
