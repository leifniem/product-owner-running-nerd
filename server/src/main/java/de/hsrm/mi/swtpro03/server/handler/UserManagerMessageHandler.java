package de.hsrm.mi.swtpro03.server.handler;

import de.hsrm.mi.swtpro03.server.game.session.management.UserManager;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.ClientAlreadyHasUserException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNameTakenException;
import de.hsrm.mi.swtpro03.server.network.ComChannel;
import de.hsrm.mi.swtpro03.server.network.IConnectionListener;
import de.hsrm.mi.swtpro03.server.network.IMessageListener;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import de.hsrm.mi.swtpro03.server.network.messages.user.*;
import de.hsrm.mi.swtpro03.server.utils.Serializer;

public class UserManagerMessageHandler implements IConnectionListener, IMessageListener {

	private static final String USER_MANAGER_CHANNEL = "Server.Users";
	private ComChannel userLoginChannel;

	private UserManager userManager;

	public UserManagerMessageHandler(UserManager userManager) {
		this.userManager = userManager;
		this.userLoginChannel = NetworkService.createComChannel(USER_MANAGER_CHANNEL);
		this.userLoginChannel.addMessageListener(this);
		NetworkService.addConnectionListener(this);
	}

	private void sendAcceptLoginMessage(String clientID) {
		AcceptUserLoginMessage message = new AcceptUserLoginMessage();
		userLoginChannel.sendToClient(clientID, AcceptUserLoginMessage.TYPE, Serializer.serialize(message, AcceptUserLoginMessage.class));
	}

	private void sendDenyLoginMessage(String clientID, String reason) {
		DenyUserLoginMessage message = new DenyUserLoginMessage(reason);
		userLoginChannel.sendToClient(clientID, DenyUserLoginMessage.TYPE, Serializer.serialize(message, DenyUserLoginMessage.class));
	}

	@Override
	public void onMessageReceived(String clientID, String messageType, String message) {

		switch (messageType) {
			case UserLoginMessage.TYPE:
				handleMessage(clientID, (UserLoginMessage) Serializer.deserialize(message, UserLoginMessage.class));
				break;
		}
	}

	private void handleMessage(String clientID, UserLoginMessage userLoginMessage) {
		try {
			userManager.addUser(clientID, userLoginMessage.getUsername());
			sendAcceptLoginMessage(clientID);

		} catch (ClientAlreadyHasUserException e) {

			sendDenyLoginMessage(clientID, e.getMessage());
			e.printStackTrace();

		} catch (UserNameTakenException e) {

			sendDenyLoginMessage(clientID, e.getMessage());
			e.printStackTrace();
		}
	}

	@Override
	public void onClientConnected(String clientID) {
		// not necessary at the moment
	}

	@Override
	public void onClientDisconnected(String clientID) {
		userManager.removeUserByClientID(clientID);
	}

	@Override
	public void onClientTimeout(String clientID) {
		userManager.removeUserByClientID(clientID);
	}
}
