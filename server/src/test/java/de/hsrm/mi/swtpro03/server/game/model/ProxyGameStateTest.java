package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.session.GameSession;
import de.hsrm.mi.swtpro03.server.game.session.User;
import org.junit.jupiter.api.BeforeAll;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.util.Collection;
import java.util.HashMap;

import static org.junit.jupiter.api.Assertions.assertNotNull;
import static org.mockito.Mockito.mock;
import static org.mockito.Mockito.when;

public class ProxyGameStateTest {

	static ProxyGameState proxy;

	@BeforeAll
	static void setUp() {

		// setup dummy map
		Map map = mock(Map.class);
		int range = 1 + EnemyGameCharacter.RANGE_OF_SIGHT * 2;
		Tile[][] tiles = new Tile[range][range];
		when(map.getTiles()).thenReturn(tiles);

		// setup dummy session
		GameSession session = mock(GameSession.class);
		java.util.Map<User, PlayerGameCharacter> players = new HashMap<>();
		when(session.getUserGameCharacterHashMap()).thenReturn(players);

		// setup dummy AI character
		EnemyGameCharacter character = mock(EnemyGameCharacter.class);
		when(character.getPosition()).thenReturn(new Vector2(15f, 14f));

		// create proxy object and pass in the mockito dummys
		proxy = new ProxyGameState(session, character, map);
	}

	@Test
	@DisplayName("Test the getTilesInRange() Method of the proxy object")
	void testGetTilesInRange() {
		Tile[][] tiles = proxy.getTilesInRange();
		assertNotNull(tiles);
	}

	@Test
	@DisplayName("Test the getPlayersInRange() Method of the proxy object")
	void testGetPlayersInRange() {
		Collection<Vector2> players = proxy.getPlayersInRange();
		assertNotNull(players);
	}
}
