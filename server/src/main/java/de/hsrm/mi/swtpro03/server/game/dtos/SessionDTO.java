package de.hsrm.mi.swtpro03.server.game.dtos;

/**
 * @Author Florian Ortmann
 * SessionDTO to send sessionmetadata to the client
 */
public class SessionDTO {

	private String id;
	private String name;
	private int users;
	private int minUser;
	private int ping;
	private boolean editorSession;
	private boolean gameSession;
	private MapMetaDTO mapMetaDTO;
	public SessionDTO(){

	}

	public SessionDTO(String id,String name,boolean gameSession,boolean editorSession,MapMetaDTO mapMetaDTO){
		this.id = id;
		this.name = name;
		this.editorSession = editorSession;
		this.gameSession = gameSession;
		this.mapMetaDTO = mapMetaDTO;
	}

	public String getName() {
		return name;
	}

	public void setName(String name) {
		this.name = name;
	}

	public String getId() {
		return id;
	}

	public void setId(String id) {
		this.id = id;
	}

	public int getUsers() {
		return users;
	}

	public void setUsers(int users) {
		this.users = users;
	}

	public int getPing() {
		return ping;
	}

	public void setPing(int ping) {
		this.ping = ping;
	}

	public boolean isEditorSession() {
		return editorSession;
	}

	public void setEditorSession(boolean editorSession) {
		this.editorSession = editorSession;
	}

	public boolean isGameSession() {
		return gameSession;
	}

	public int getMinUser() {
		return minUser;
	}

	public void setMinUser(int minUsers) {
		this.minUser = minUser;
	}

	public MapMetaDTO getMapMetaDTO() {
		return mapMetaDTO;
	}

	public void setMapMetaDTO(MapMetaDTO mapDTO) {
		this.mapMetaDTO = mapDTO;
	}

	public void setGameSession(boolean gameSession) {
		this.gameSession = gameSession;
	}
}
