package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.session.User;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.ClientAlreadyHasUserException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNameTakenException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.UserNotFoundException;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import org.junit.jupiter.api.*;

import java.util.List;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

class UserManagerTest {
	private static final String TEST_ID = "TEST-ID";
	private static final String TEST_NAME = "TEST-NAME";
	private static final int TEST_PORT = 1717;
	private static final String DIFFERENT_ID = "different-ID";
	private static final String DIFFERENT_NAME = "different-Name";

	private static UserManager userManager;

	@BeforeAll
	static void prepare() {
		NetworkService.start(TEST_PORT);
		userManager = UserManager.getInstance();
	}

	@BeforeEach
	void setUp() throws UserNameTakenException, ClientAlreadyHasUserException {
		userManager.addUser(TEST_ID, TEST_NAME);
	}

	@AfterEach
	void tearDown() {
		// needed because the manager is a static singleton
		userManager.removeUserByClientID(TEST_ID);
		userManager.removeUserByClientID(DIFFERENT_ID);
	}

	@AfterAll
	static void shutdown() {
		NetworkService.shutdown();
	}

	@Test
	@DisplayName("Instance exists")
	void getInstance_userManagerExists() {
		assertNotNull(userManager);
	}

	@Test
	@DisplayName("User successfully added")
	void addUser_bestCase() throws UserNotFoundException {
		User knownUser = userManager.getUserByClientID(TEST_ID);

		assertNotNull(knownUser);
	}

	@Test
	@DisplayName("Added User has correct ID")
	void addUser_addedUserHasCorrectID() throws UserNotFoundException {
		User knownUser = userManager.getUserByClientID(TEST_ID);

		assertEquals(TEST_ID, knownUser.getClientID());
	}

	@Test
	@DisplayName("Added User has correct name")
	void addUser_addedUserHasCorrectName() throws UserNotFoundException {
		User knownUser = userManager.getUserByClientID(TEST_ID);

		assertEquals(TEST_NAME, knownUser.getUsername());
	}

	@Test
	@DisplayName("Adding an additional different User does not throw Exceptions")
	void addUser_addingDifferentUserDoesNotThrow() {
		assertDoesNotThrow(
				() -> userManager.addUser(DIFFERENT_ID, DIFFERENT_NAME)
		);
	}

	@Test
	@DisplayName("Adding a user fails because of taken username")
	void addUser_throwsUserNameTakenException() {
		assertThrows(
				UserNameTakenException.class,
				() -> userManager.addUser(DIFFERENT_ID, TEST_NAME)
		);
	}

	@Test
	@DisplayName("Adding a user fails because client has already a user")
	void addUser_throwsClientAlreadyHasUserException() {
		assertThrows(
				ClientAlreadyHasUserException.class,
				() -> userManager.addUser(TEST_ID, DIFFERENT_NAME)
		);
	}

	@Test
	@DisplayName("Successfully remove a User by his ID")
	void removeUserByClientID_throwsUserNotFoundExceptionAfterwards() {
		userManager.removeUserByClientID(TEST_ID);

		assertThrows(
				UserNotFoundException.class,
				() -> userManager.getUserByClientID(TEST_ID)
		);
	}

	@Test
	@DisplayName("Removing an user by unknown ID does nothing")
	void removeUserByClientID_hasSameUsersAfterwards() {
		List activeUsers = userManager.getActiveUsers();

		userManager.removeUserByClientID(UUID.randomUUID().toString());

		assertIterableEquals(activeUsers, userManager.getActiveUsers());
	}

	@Test
	@DisplayName("Successfully remove a User by his name")
	void removeUserByName_throwsUserNotFoundExceptionAfterwards() {
		userManager.removeUserByName(TEST_NAME);

		assertThrows(
				UserNotFoundException.class,
				() -> userManager.getUserByName(TEST_NAME)
		);
	}

	@Test
	@DisplayName("Removing an user by unknown Name does nothing")
	void removeUserByName_hasSameUsersAfterwards() {
		List activeUsers = userManager.getActiveUsers();

		userManager.removeUserByName(UUID.randomUUID().toString());

		assertIterableEquals(activeUsers, userManager.getActiveUsers());
	}

	@Test
	@DisplayName("Successfully remove a User by his User-object")
	void removeUser_throwsUserNotFoundExceptionAfterwards() throws UserNotFoundException {
		User knownUser = userManager.getUserByClientID(TEST_ID);

		userManager.removeUser(knownUser);

		assertThrows(
				UserNotFoundException.class,
				() -> userManager.getUserByName(TEST_NAME)
		);
	}

	@Test
	@DisplayName("Successfully get User-object by his name")
	void getUserByName_bestCase() throws UserNotFoundException {
		User knownUser = userManager.getUserByName(TEST_NAME);

		assertEquals(TEST_NAME, knownUser.getUsername());
	}

	@Test
	@DisplayName("Getting a User by an unknown name throws Exception")
	void getUserByName_throwsUserNotFoundException() {
		assertThrows(
				UserNotFoundException.class,
				() -> userManager.getUserByName(UUID.randomUUID().toString())
		);
	}

	@Test
	@DisplayName("Successfully get User-object by his ID")
	void getUserByClientID_bestCase() throws UserNotFoundException {
		User knownUser = userManager.getUserByClientID(TEST_ID);

		assertEquals(TEST_NAME, knownUser.getUsername());
	}

	@Test
	@DisplayName("Getting a User by an unknown ID throws Exception")
	void getUserByClientID_throwsUserNotFoundException() {
		assertThrows(
				UserNotFoundException.class,
				() -> userManager.getUserByClientID(UUID.randomUUID().toString())
		);
	}
}