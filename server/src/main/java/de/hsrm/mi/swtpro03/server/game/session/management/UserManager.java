package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.session.User;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.ClientAlreadyHasUserException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNameTakenException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.handler.UserManagerMessageHandler;

import java.util.*;

public class UserManager {

	private static UserManager instance;

	private UserManagerMessageHandler messageHandler;

	private Map<String, User> activeUsersByClientID = Collections.synchronizedMap(new HashMap<>());
	private Map<String, User> activeUsersByUsername = Collections.synchronizedMap(new HashMap<>());

	private UserManager() {
		messageHandler = new UserManagerMessageHandler(this);
	}

	/**
	 * @return the instance of UserManager
	 */
	public static synchronized UserManager getInstance() {
		if (instance == null) {
			instance = new UserManager();
		}
		return instance;
	}

	/**
	 * @param clientID ID of the C#-Client-Window
	 * @param username Chosen name of the user in his C#-Client
	 * @throws ClientAlreadyHasUserException thrown if the clientID is already registered
	 * @throws UserNameTakenException        thrown if the userName is already registered
	 */
	public void addUser(String clientID, String username) throws ClientAlreadyHasUserException, UserNameTakenException {
		if (activeUsersByClientID.containsKey(clientID))
			throw new ClientAlreadyHasUserException(
					"Client(" + clientID + ") already has a User: " + activeUsersByClientID.get(clientID).getUsername());
		if (activeUsersByUsername.containsKey(username))
			throw new UserNameTakenException("Username already taken: " + username);

		User newUser = new User(clientID, username);
		activeUsersByClientID.put(clientID, newUser);
		activeUsersByUsername.put(username, newUser);
	}

	/**
	 * @param clientID ID of the C#-Client-Window
	 */
	public void removeUserByClientID(String clientID) {
		if (!activeUsersByClientID.containsKey(clientID))
			return;

		removeUser(activeUsersByClientID.get(clientID));
	}

	/**
	 * @param username Chosen name of the user in his C#-Client
	 */
	void removeUserByName(String username) {
		if (!activeUsersByUsername.containsKey(username))
			return;

		removeUser(activeUsersByUsername.get(username));
	}

	/**
	 * @param user Our internal User-Object for the existing connection
	 */
	void removeUser(User user) {
		user.leaveActiveSession();
		activeUsersByClientID.remove(user.getClientID());
		activeUsersByUsername.remove(user.getUsername());
	}

	/**
	 * @param username Chosen name of the user in his C#-Client
	 * @return Our internal User-Object for the existing connection
	 * @throws UserNotFoundException thrown if the requested User is not registered
	 */
	User getUserByName(String username) throws UserNotFoundException {
		if (!activeUsersByUsername.containsKey(username))
			throw new UserNotFoundException("User with this name (" + username + ") doesn't exist");

		return activeUsersByUsername.get(username);
	}

	/**
	 * @param clientID ID of the C#-Client-Window
	 * @return Our internal User-Object for the existing connection
	 * @throws UserNotFoundException thrown if the requested User is not registered
	 */
	public User getUserByClientID(String clientID) throws UserNotFoundException {
		if (!activeUsersByClientID.containsKey(clientID))
			throw new UserNotFoundException("Client(" + clientID + ") has no User");

		return activeUsersByClientID.get(clientID);
	}

	/**
	 * @return An ArrayList of User-Objects. Represents a List of successfully connected Client-Windows.
	 */
	List<User> getActiveUsers() {
		return new ArrayList<>(activeUsersByClientID.values());
	}
}
