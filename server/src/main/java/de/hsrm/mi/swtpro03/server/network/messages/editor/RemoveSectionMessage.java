package de.hsrm.mi.swtpro03.server.network.messages.editor;

public class RemoveSectionMessage {

    public static final String TYPE = "RemoveSectionMessage";

    private int sections;

	public int getSections() {
		return sections;
	}

	public void setSections(int sections) {
		this.sections = sections;
	}
    
    
    
}
