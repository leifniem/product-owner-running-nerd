package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DenySelectSectionMessage {
    public static final String TYPE = "DenySelectSectionMessage";

    private String reason;

    public DenySelectSectionMessage() {
    }

    public DenySelectSectionMessage(String reason) {
        this.reason = reason;
    }

    public String getReason() {
        return this.reason;
    }

    public void setReason(String reason) {
        this.reason = reason;
    }
}
