package de.hsrm.mi.swtpro03.server.network.messages.session;

import java.util.HashMap;
import java.util.List;

public class SelectSectionMessage {
	public static final String TYPE = "SelectSectionMessage";
	private int section;
	private HashMap<Integer,List<String>> sections;

	public SelectSectionMessage() {
	}

	public SelectSectionMessage(int x) {
		this.section = x;
	}

	public SelectSectionMessage(HashMap<Integer,List<String>> list){
		this.sections = list;
	}

	public int getSection() {
		return section;
	}

	public void setSection(int section) {
		this.section = section;
	}

	public HashMap<Integer,List<String>> getSections() {
		return sections;
	}

	public void setSections(HashMap<Integer,List<String>> sections) {
		this.sections = sections;
	}
}
