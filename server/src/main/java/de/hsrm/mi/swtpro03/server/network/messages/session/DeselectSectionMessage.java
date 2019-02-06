package de.hsrm.mi.swtpro03.server.network.messages.session;

public class DeselectSectionMessage {
	public static final String TYPE = "DeselectSectionMessage";
	private int section;

	public DeselectSectionMessage() {
	}

	public DeselectSectionMessage(int x) {
		this.section = x;
	}

	public int getSection() {
		return section;
	}

	public void setSection(int section) {
		this.section = section;
	}
}
