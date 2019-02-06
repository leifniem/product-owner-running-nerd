package de.hsrm.mi.swtpro03.server.game.dtos;

import java.util.Objects;

public class MapMetaDTO {

	public String name;
	public int numberOfSections;

	// Default constructor for Jackson
	public MapMetaDTO() {}

	public MapMetaDTO(String title, int numberOfSections) {
		this.name = title;
		this.numberOfSections = numberOfSections;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public int getNumberOfSections() {
		return numberOfSections;
	}

	public void setNumberOfSections(int numberOfSections) {
		this.numberOfSections = numberOfSections;
	}

	@Override
	public boolean equals(Object o) {
		if (this == o) return true;
		if (o == null || getClass() != o.getClass()) return false;
		MapMetaDTO that = (MapMetaDTO) o;
		return numberOfSections == that.numberOfSections &&
				Objects.equals(name, that.name);
	}

}
