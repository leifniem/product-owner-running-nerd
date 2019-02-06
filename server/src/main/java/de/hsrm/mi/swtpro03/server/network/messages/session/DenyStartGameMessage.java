package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DenyStartGameMessage {
    public static final String TYPE = "DenyStartGameMessage";

    private String reason;

    public DenyStartGameMessage() {
    }

    public DenyStartGameMessage(String reason) {
        this.reason = reason;
    }

    public String getReason() {
        return this.reason;
    }

    public void setReason(String reason) {
        this.reason = reason;
    }
}
