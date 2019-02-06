package de.hsrm.mi.swtpro03.server.network.messages.session;

import de.hsrm.mi.swtpro03.server.game.dtos.MapMetaDTO;

import java.util.List;

public class MapListMessage {
	public static final String TYPE = "MapListMessage";
	private List<MapMetaDTO> listOfMaps;
	private List<String> blockedMaps;

	public MapListMessage(List<MapMetaDTO> mapList, List<String> blockedMaps) {
		this.listOfMaps = mapList;
		this.blockedMaps = blockedMaps;
	}

	public MapListMessage() {
	}

	public List<MapMetaDTO> getListOfMaps() {
		return listOfMaps;
	}

	public void setListOfMaps(List<MapMetaDTO> listOfMaps) {
		this.listOfMaps = listOfMaps;
	}

	public List<String> getBlockedMaps() {
		return blockedMaps;
	}

	public void setBlockedMaps(List<String> blockedMaps) {
		this.blockedMaps = blockedMaps;
	}
}
