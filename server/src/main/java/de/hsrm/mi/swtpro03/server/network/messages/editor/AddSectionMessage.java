package de.hsrm.mi.swtpro03.server.network.messages.editor;

public class AddSectionMessage {

    public static final String TYPE = "AddSectionMessage";

    private int sections;

	public int getSections() {
		return sections;
	}

	public void setSections(int sections) {
		this.sections = sections;
	}

}
