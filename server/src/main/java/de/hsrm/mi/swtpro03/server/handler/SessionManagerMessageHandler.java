package de.hsrm.mi.swtpro03.server.handler;

import de.hsrm.mi.swtpro03.server.game.dtos.MapMetaDTO;
import de.hsrm.mi.swtpro03.server.game.dtos.SessionDTO;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.session.*;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.SessionNotFoundException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserIsAlreadyInSessionException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.game.session.management.MapManager;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.game.session.management.UserManager;
import de.hsrm.mi.swtpro03.server.network.ComChannel;
import de.hsrm.mi.swtpro03.server.network.IMessageListener;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import de.hsrm.mi.swtpro03.server.network.messages.session.*;
import de.hsrm.mi.swtpro03.server.utils.Serializer;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.UUID;

public class SessionManagerMessageHandler implements IMessageListener {

	private static final String SESSION_MANAGER_CHANNEL = "Server.Sessions";
	private ComChannel sessionChannel;

	private SessionManager sessionManager;
	private UserManager userManager;


	public SessionManagerMessageHandler(SessionManager sessionManager) {
		this.sessionManager = sessionManager;
		this.userManager = UserManager.getInstance();
		this.sessionChannel = NetworkService.createComChannel(SESSION_MANAGER_CHANNEL);
		this.sessionChannel.addMessageListener(this);
	}

	private SessionListMessage createSessionListMessage() {
		List<Session> sessions = SessionManager.getInstance().getActiveSessions();
		ArrayList<SessionDTO> sessionDTOS = new ArrayList<>();
		for (Session session : sessions) {
			sessionDTOS.add(session.toSessionDTO());
		}
		return new SessionListMessage(sessionDTOS);
	}

	private MapListMessage createMapListMessage() {
		ArrayList<Map> list = new ArrayList<>(MapManager.getMapDictionary().values());
		ArrayList<MapMetaDTO> mapList = new ArrayList<>();
		list.forEach((item) -> mapList.add(item.toMapDTO().getMeta()));
		return new MapListMessage(mapList, MapManager.getBlockedMaps());
	}

	private void sendAcceptJoinMessage(String clientID, String color) {
		AcceptJoinSessionMessage message = new AcceptJoinSessionMessage(color);
		sessionChannel.sendToClient(clientID, AcceptJoinSessionMessage.TYPE, Serializer.serialize(message, AcceptJoinSessionMessage.class));
	}

	private void sendDenyJoinMessage(String clientID, String reason) {
		DenyJoinSessionMessage message = new DenyJoinSessionMessage(reason);
		sessionChannel.sendToClient(clientID, DenyJoinSessionMessage.TYPE, Serializer.serialize(message, DenyJoinSessionMessage.class));
	}

	private void sendAcceptCreateMessage(String clientID, UUID sessionID, String color) {
		AcceptCreateSessionMessage message = new AcceptCreateSessionMessage(sessionID.toString(), color);
		sessionChannel.sendToClient(clientID, AcceptCreateSessionMessage.TYPE, Serializer.serialize(message, AcceptCreateSessionMessage.class));
	}

	private void sendDenyCreateMessage(String clientID, String reason) {
		DenyCreateSessionMessage message = new DenyCreateSessionMessage(reason);
		sessionChannel.sendToClient(clientID, DenyCreateSessionMessage.TYPE, Serializer.serialize(message, DenyCreateSessionMessage.class));
	}

	private void sendSessionListMessage(String clientID) {
		SessionListMessage message = createSessionListMessage();
		sessionChannel.sendToClient(clientID, SessionListMessage.TYPE, Serializer.serialize(message, SessionListMessage.class));
	}

	private void sendMapListMessage(String clientID, MapListMessage mapListMessage) {
		sessionChannel.sendToClient(clientID, MapListMessage.TYPE, Serializer.serialize(mapListMessage, MapListMessage.class));
	}

	@Override
	public void onMessageReceived(String clientID, String messageType, String message) {
		switch (messageType) {
			case GetSessionListMessage.TYPE:
				sendSessionListMessage(clientID);
				break;
			case MapListMessage.TYPE:
				sendMapListMessage(clientID, createMapListMessage());
				break;
			case CreateSessionMessage.TYPE:
				createSession(clientID, (CreateSessionMessage) Serializer.deserialize(message, CreateSessionMessage.class));
				break;
			case JoinSessionMessage.TYPE:
				joinSession(clientID, (JoinSessionMessage) Serializer.deserialize(message, JoinSessionMessage.class));
				break;
			case CreateMapMessage.TYPE:
				createMap(clientID, (CreateMapMessage) Serializer.deserialize(message, CreateMapMessage.class));
		}
	}

	private void createMap(String clientID, CreateMapMessage message) {
		if (MapManager.isNewMap(message.getSessionDTO().getMapMetaDTO().getName())) {
			createSession(clientID, new CreateSessionMessage(message.getSessionDTO()));
		} else {
			sendDenyCreateMessage(clientID, String.format("Map '%s' already exists.", message.getSessionDTO().getMapMetaDTO().getName()));
		}
	}

	private void createSession(String clientID, CreateSessionMessage createSessionMessage) {
		try {
			User user = UserManager.getInstance().getUserByClientID(clientID);
			Session session = user.createSession(createSessionMessage.getSessionDTO());
			if (session instanceof GameSession) {
				user.setColor(((GameSession) session).getColor());
			}
			sendAcceptCreateMessage(clientID, session.getSessionID(), user.getColor());

		} catch (UserNotFoundException | UserIsAlreadyInSessionException e) {
			sendDenyCreateMessage(clientID, e.getMessage());
			e.printStackTrace();
		} catch (MapBlockedException e) {
			sendDenyCreateMessage(clientID, String.format("Map '%s' is already in use in an other EditorSession", e.getMessage()));
		}
	}

	private void joinSession(String clientID, JoinSessionMessage joinSessionMessage) {
		SessionRole sessionRole = joinSessionMessage.getSessionRole();
		String sessionID = joinSessionMessage.getSessionID();

		Session session;
		try {
			session = sessionManager.getSessionByID(UUID.fromString(sessionID));
			User user = userManager.getUserByClientID(clientID);
			user.joinSession(session, sessionRole);
			sendAcceptJoinMessage(clientID, user.getColor());

			if (session instanceof GameSession) {
				user.setColor(((GameSession) session).getColor());
				HashMap<Integer, List<String>> sections = new HashMap<>();
				((GameSession) session).getMapSectionUserHashMap().forEach((k, v) -> {
					List<String> list = new ArrayList<>();
					list.add(v.getUsername());
					list.add(v.getColor());
					sections.put(k, list);
				});
				sessionChannel.sendToClient(clientID, SelectSectionMessage.TYPE,
						Serializer.serialize(
								new SelectSectionMessage(sections),
								SelectSectionMessage.class
						)
				);
			}

			if (session instanceof EditorSession) ((EditorSession) session).startEditing(user);

		} catch (SessionNotFoundException | UserNotFoundException | UserIsAlreadyInSessionException e) {
			sendDenyJoinMessage(clientID, e.getMessage());
			e.printStackTrace();
		}
	}

}
