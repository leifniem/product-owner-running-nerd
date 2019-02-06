package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;
import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import org.junit.jupiter.api.*;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.*;

class MapManagerTest {

	private static final String TEST_MAP_NAME = "dummy";
	private static Map map;

	@BeforeAll
	static void loadTestMap() throws IOException {
		createTestMap();
		MapManager.loadAllMaps();
		map = MapManager.getMapForGameSession(TEST_MAP_NAME);
		assertNotNull(map);
	}

	private static void createTestMap() throws IOException {
		MapManager.saveMap(new Map(1, TEST_MAP_NAME, new Vector2Int(0, 0)));
	}

	@AfterAll
	static void removeTestMap(){
		File file = new File("maps" + File.separator + TEST_MAP_NAME +".json");
		file.delete();
	}

	@AfterEach
	void tearDown() {
		MapManager.release(map.getName());
	}

	@Test
	@DisplayName("Map saves successfully")
	void saveMap_saveMapSuccessfully() throws IOException {
		Map newMap = new Map(1, "newTestMap", new Vector2Int(1, 1));

		MapManager.saveMap(newMap);
		File file = new File("maps" + File.separator + "newTestMap.json");

		assertTrue(file.exists());
		file.delete();
	}

	@Test
	@DisplayName("Map overwrites existing successfully")
	void saveMap_overwriteMapSuccessfully() throws IOException {
		MapManager.saveMap(map);
		File file = new File("maps" + File.separator + TEST_MAP_NAME + ".json");

		assertTrue(file.exists());
	}

	@Test
	@DisplayName("Map is marked as unblocked")
	void releaseMap_mapIsNoLongerMarkedAsBlocked() {
		MapManager.block(map.getName());

		MapManager.release(map.getName());

		assertFalse(MapManager.isBlocked(map.getName()));
	}

	@Test
	@DisplayName("Multiple unblocking causes no problems")
	void releaseMap_unblockMapMultipleTimes() {
		MapManager.block(map.getName());

		MapManager.release(map.getName());
		MapManager.release(map.getName());

		assertFalse(MapManager.isBlocked(map.getName()));
	}

	@Test
	@DisplayName("Map is marked as blocked")
	void blockMap_mapIsMarkedAsBlocked() {
		MapManager.block(map.getName());

		assertTrue(MapManager.isBlocked(map.getName()));
	}

	@Test
	@DisplayName("Multiple blocking causes no problems")
	void blockMap_blockMapMultipleTimes() {
		MapManager.block(map.getName());

		MapManager.block(map.getName());

		assertTrue(MapManager.isBlocked(map.getName()));
	}

	@Test
	@DisplayName("isBlocked by String")
	void isBlocked_InformationAvailableViaString() {
		MapManager.block(map.getName());

		assertTrue(MapManager.isBlocked(TEST_MAP_NAME));
	}

	@Test
	@DisplayName("Blocked-Map-List contains correct Elements")
	void getBlockedMaps_hasCorrectElements() {
		String mapName1 = "TestMap1";
		String mapName2 = "TestMap2";

		List<String> mapList = new ArrayList<>();
		mapList.add(mapName1);
		mapList.add(mapName2);

		MapManager.block(mapName1);
		MapManager.block(mapName2);

		assertIterableEquals(mapList, MapManager.getBlockedMaps());
	}

	@Test
	@DisplayName("Get a blocked Map for a GameSession")
	void getMapForGameSession_getABlockedMap() {
		MapManager.block(map.getName());

		assertDoesNotThrow(() -> MapManager.getMapForGameSession(map.getName()));
	}

	@Test
	@DisplayName("Get independent Map instead of reference")
	void getMapForGameSession_getDeepCopyInsteadOfReference() {
		Map original = MapManager.getMapDictionary().get(TEST_MAP_NAME);
		Map copy = MapManager.getMapForGameSession(TEST_MAP_NAME);

		assertNotSame(original, copy);
	}

	@Test
	@DisplayName("Obtain an Exception instead the blocked map")
	void getMapForEditorSession_getExceptionInsteadOfMap() {
		MapManager.block(map.getName());

		assertThrows(
				MapBlockedException.class,
				() -> MapManager.getMapForEditorSession(map.getName())
		);
	}

	@Test
	@DisplayName("New Map is not blocked")
	void getMapForEditorSession_createNewMapDoesNotThrowBlockedException() {
		String newMapName = Double.toString(Math.random());

		assertDoesNotThrow(() -> MapManager.getMapForEditorSession(newMapName));
	}

	@Test
	@DisplayName("Create successfully a new TempMap")
	void getMapForEditorSession_createNewMap() throws MapBlockedException {
		String newMapName = Double.toString(Math.random());

		Map newMap = MapManager.getMapForEditorSession(newMapName);

		assertEquals(newMapName, newMap.getName());
		MapManager.getMapDictionary().remove(newMapName, newMap);
	}

	@Test
	@DisplayName("New TempMap is in MapDictionary")
	void getMapDictionary_newMapIsInMapDict() throws MapBlockedException {
		String newMapName = Double.toString(Math.random());

		MapManager.getMapForEditorSession(newMapName);
		Map newMap = MapManager.getMapDictionary().get(newMapName);

		assertNotNull(newMap);
	}
}