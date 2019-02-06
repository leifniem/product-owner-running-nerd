package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;
import de.hsrm.mi.swtpro03.server.game.session.*;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.SessionNotFoundException;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import org.junit.jupiter.api.AfterAll;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.io.IOException;
import java.util.List;
import java.util.Set;
import java.util.UUID;

import static org.junit.jupiter.api.Assertions.*;

class SessionManagerTest {
	private static final String TEST_NAME_SESSION = "testSession";
	private static final String TEST_MAP_NAME = "dummy";
	private static SessionManager sessionManager;
	private static User testUser;
	private static Session testSession;

	@BeforeAll
	static void prepare() throws IOException, MapBlockedException {
		NetworkService.start(1717);
		createTestMap();
		MapManager.loadAllMaps();

		sessionManager = SessionManager.getInstance();
		testUser = new User("zero", "TestUser");
		testSession = createTestSession();
	}

	private static void createTestMap() throws IOException {
		MapManager.saveMap(new Map(1, TEST_MAP_NAME, new Vector2Int(0, 0)));
	}

	@AfterAll
	static void quitProperly() {
		NetworkService.shutdown();
	}

	@Test
	@DisplayName("Singleton exists")
	void getInstance_notNull() {
		assertNotNull(sessionManager);
	}

	@Test
	@DisplayName("Create a Session successfully")
	void createSession_bestCaseScenario() {
		assertDoesNotThrow(SessionManagerTest::createTestSession);
	}

	@Test
	@DisplayName("Create a Session fails by blocked map")
	void createSession_failsByBlockedMap() {
		MapManager.block(TEST_MAP_NAME);

		assertThrows(
				MapBlockedException.class,
				SessionManagerTest::createEditorTestSession
		);

		MapManager.release(TEST_MAP_NAME);
	}

	@Test
	@DisplayName("Create a Session with correct name")
	void createSession_newSessionHasCorrectName() {
		assertEquals(TEST_NAME_SESSION, testSession.getName());
	}

	@Test
	@DisplayName("Create a Session with correct map")
	void createSession_newSessionHasCorrectMap() {
		assertEquals(TEST_MAP_NAME, testSession.getMapMeta().getName());
	}

	@Test
	@DisplayName("Create a Session with correct user")
	void createSession_newSessionHasCorrectUser() {
		Set usersInSession = testSession.getUsersInSession().keySet();

		assertTrue(usersInSession.contains(testUser));
	}

	@Test
	@DisplayName("Create password protected Session")
	void createPadProtectedSession_bestCaseScenario() {
		assertDoesNotThrow(SessionManagerTest::createProtectedTestSession);
	}

	@Test
	@DisplayName("Protected Session has password")
	void createPadProtectedSession_hasPassword() throws MapBlockedException {
		Session protectedSession = createProtectedTestSession();

		assertNotNull(protectedSession.getPassword());
	}

	@Test
	@DisplayName("Password protected Session has correct Password")
	void createPwdProtectedSession_hasCorrectPassword() throws MapBlockedException {
		Session protectedSession = createProtectedTestSession();

		assertEquals("test", protectedSession.getPassword());
	}

	@Test
	@DisplayName("Session has been successfully removed")
	void removeSession_sessionIsNotInActiveSessions() {
		UUID testSessionID = testSession.getSessionID();

		sessionManager.removeSession(testSessionID);

		assertThrows(
				SessionNotFoundException.class,
				() -> sessionManager.getSessionByID(testSessionID)
		);
	}

	@Test
	@DisplayName("Does not remove valid Sessions by invalid ID")
	void removeSession_ignoresInvalidID() {
		List activeSessions = sessionManager.getActiveSessions();
		UUID invalidID = UUID.randomUUID();

		sessionManager.removeSession(invalidID);

		assertIterableEquals(activeSessions, sessionManager.getActiveSessions());
	}

	@Test
	@DisplayName("Terminated Session has no more users")
	void removeSession_endsSessionProperly() {
		Session sessionToEnd = new GameSession(testUser, "End me Baby one more time", MapManager.getMapForGameSession(TEST_MAP_NAME));
		sessionToEnd.addUser(new User("my_ID", "me_iz_nobody"), SessionRole.SESSION_GUEST);
		sessionToEnd.addUser(new User("my_ID1", "Me_iz_anonymous"), SessionRole.SESSION_GUEST);

		sessionManager.addSession(sessionToEnd);
		sessionManager.removeSession(sessionToEnd);
		Set usersInSession = sessionToEnd.getUsersInSession().keySet();

		assertEquals(0, usersInSession.size());
	}

	@Test
	@DisplayName("All active Sessions are dead")
	void endAllSessions_noMoreActiveSessions() {
		for (int i = 0; i < 4; i++) {
			String sessionName = "I\'ll be dead soon [" + i + "]";
			sessionManager.addSession(new GameSession(testUser, sessionName, MapManager.getMapForGameSession(TEST_MAP_NAME)));
		}

		sessionManager.endAllSessions();
		List activeSessions = sessionManager.getActiveSessions();

		assertEquals(0, activeSessions.size());
	}

	@Test
	@DisplayName("Session-ID is known to SessionManager")
	void getSessionByID_sessionManagerKnowsTheSession() {
		assertDoesNotThrow(
				() -> sessionManager.getSessionByID(testSession.getSessionID())
		);
	}

	@Test
	@DisplayName("Session-ID is not known to SessionManager")
	void getSessionByID_sessionManagerYieldsExceptionForUnknownSession() {
		assertThrows(
				SessionNotFoundException.class,
				() -> sessionManager.getSessionByID(UUID.randomUUID())
		);
	}

	@Test
	@DisplayName("Session-Object is known to SessionManager")
	void getActiveSessions_SessionListContainsCorrectObjects() {
		List activeSessions = sessionManager.getActiveSessions();
		assertTrue(activeSessions.contains(testSession));
	}

	@Test
	@DisplayName("Add successfully a new Session")
	void addSession_bestCase() {
		Session newSession = new GameSession(testUser, "Rush B, blien!", MapManager.getMapForGameSession(TEST_MAP_NAME));
		sessionManager.addSession(newSession);

		List activeSessions = sessionManager.getActiveSessions();
		assertTrue(activeSessions.contains(newSession));
	}

	private static Session createTestSession() throws MapBlockedException {
		return sessionManager.createSession(testUser, TEST_NAME_SESSION, SessionType.GAME_SESSION, TEST_MAP_NAME);
	}

	private static void createEditorTestSession() throws MapBlockedException {
		sessionManager.createSession(testUser, TEST_NAME_SESSION, SessionType.EDITOR_SESSION, TEST_MAP_NAME);
	}

	private static Session createProtectedTestSession() throws MapBlockedException {
		return sessionManager.createPwdProtectedSession(testUser, TEST_NAME_SESSION, SessionType.GAME_SESSION, TEST_MAP_NAME, "test");
	}
}
