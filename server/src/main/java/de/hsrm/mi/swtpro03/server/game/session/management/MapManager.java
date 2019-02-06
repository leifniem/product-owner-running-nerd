package de.hsrm.mi.swtpro03.server.game.session.management;

import de.hsrm.mi.swtpro03.server.game.dtos.MapDTO;
import de.hsrm.mi.swtpro03.server.game.model.Map;
import de.hsrm.mi.swtpro03.server.game.model.Vector2;
import de.hsrm.mi.swtpro03.server.game.model.Vector2Int;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapBlockedException;
import de.hsrm.mi.swtpro03.server.game.session.exceptions.MapNotAvailableException;
import de.hsrm.mi.swtpro03.server.utils.Serializer;
import org.springframework.core.serializer.support.SerializationFailedException;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.OutputStreamWriter;
import java.nio.charset.Charset;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.stream.Collectors;

public class MapManager {

	private final static String MAP_FILES_FOLDER_PATH = "maps";
	private static final boolean IS_FOR_JAR = false;

	private static final int DEFAULT_NUMBER_OF_SECTIONS = 1;
	private static final String ENCODING = "UTF-8";
	private static final String FILESYSTEM_SEPARATOR = File.separator;
	private static final String MAP_FILE_EXTENSION = ".json";

	private static List<String> blockedMaps = new ArrayList<>();
	private static java.util.Map<String, Map> mapDictionary = new HashMap<>();

	/**
	 * Loads a Map for a GameSession
	 *
	 * @param mapFileName The name of the Map-File you want to load. Not the internal Map name!
	 * @return Map
	 * @see de.hsrm.mi.swtpro03.server.game.session.SessionFactory
	 * @see de.hsrm.mi.swtpro03.server.game.session.GameSession
	 */
	public static Map getMapForGameSession(String mapFileName) {
		return mapDictionary.get(mapFileName).deepCopy();
	}

	/**
	 * Loads a Map for a EditorSession. Loading a Map for EditorSessions also blocks the Map
	 * which means you can't load it for an EditorSession again.
	 *
	 * @param mapFileName The name of the Map-File you want to load. Not the internal Map name!
	 * @return Map
	 * @throws MapBlockedException Thrown if the map is already in blocked status (by another EditorSession).
	 * @see de.hsrm.mi.swtpro03.server.game.session.SessionFactory
	 * @see de.hsrm.mi.swtpro03.server.game.session.EditorSession
	 */
	public static Map getMapForEditorSession(String mapFileName) throws MapBlockedException {
		if (isBlocked(mapFileName)) throw new MapBlockedException(mapFileName);
		if (isNewMap(mapFileName)) createNewTempMap(mapFileName);

		block(mapFileName);
		return getMapForGameSession(mapFileName);
	}

	private static void createNewTempMap(String mapFileName) {
		Map newTempMap = new Map(DEFAULT_NUMBER_OF_SECTIONS, mapFileName, new Vector2Int(0, 0));
		mapDictionary.put(mapFileName, newTempMap);
	}

	public static boolean isNewMap(String mapFileName) {
		return mapDictionary.get(mapFileName) == null;
	}

	private static Map loadMapFile(File file) throws IOException {
		String mapAsString = new String(Files.readAllBytes(file.toPath()), Charset.forName(ENCODING));
		MapDTO mapDTO = (MapDTO) Serializer.deserialize(mapAsString, MapDTO.class);
		return mapDTO.toMap();
	}

	/**
	 * A method to load every map that is saved on the server into a HashMap
	 * this method is called at the start of the server and whenever we save a map
	 *
	 * @throws IOException
	 */
	public static void loadAllMaps() throws IOException {
		File mapDirectory = new File(MAP_FILES_FOLDER_PATH);
		File[] listOfMaps = mapDirectory.listFiles();
		if (listOfMaps != null) {
			for (File file : listOfMaps) {
				if (file.isFile()) {
					Map map = loadMapFile(file);
					mapDictionary.put(map.getName(), map);
				}
			}
		}
	}

	private static String buildMapFilePath(String filename) {
		StringBuilder builder = new StringBuilder();

		if (IS_FOR_JAR) {
			builder.append(FILESYSTEM_SEPARATOR);
		}
		builder.append(MAP_FILES_FOLDER_PATH);
		builder.append(FILESYSTEM_SEPARATOR);
		builder.append(filename);
		builder.append(MAP_FILE_EXTENSION);
		return builder.toString();
	}

	public static void saveMap(Map map) throws IOException {

		mapDictionary.put(map.getName(), map);

		String serializedMap;
		MapDTO mapAsDTO = map.toMapDTO();

		serializedMap = Serializer.serialize(mapAsDTO, MapDTO.class);

		if (serializedMap == null) {
			throw new SerializationFailedException("[MapManager] Failed to serialize map " + map.getName());
		}

		File mapsFolder = new File(MAP_FILES_FOLDER_PATH);

		if (!mapsFolder.exists() || !mapsFolder.isDirectory()) {
			if (!mapsFolder.mkdir()) {
				throw new IOException("[MapManager] Failed to create map folder " + MAP_FILES_FOLDER_PATH);
			}
		}

		File outputFile = new File(buildMapFilePath(map.getName()));
		FileOutputStream fileOutputStream;

		fileOutputStream = new FileOutputStream(outputFile);
		OutputStreamWriter outputStreamWriter = new OutputStreamWriter(fileOutputStream);

		outputStreamWriter.write(serializedMap);
		outputStreamWriter.close();
	}

	/**
	 * Releases a Map from its blocked status thus it can be used in an EditorSession once again.
	 *
	 * @param mapName The internal Name of the Map. Not the Map-File-Name!
	 */
	public synchronized static void release(String mapName) {
		blockedMaps.removeAll(filterBlockedMaps(mapName));
	}

	/**
	 * If not already blocked this will include the given MapName into the list of blocked Maps.
	 * Trying to load this Map for an EditorSession again will result in an Exception
	 *
	 * @param mapName The internal Name of the Map. Not the Map-File-Name!
	 * @see MapNotAvailableException
	 */
	public synchronized static void block(String mapName) {
		if (!blockedMaps.contains(mapName)) blockedMaps.add(mapName);
	}

	/**
	 * You may want to know if the Map your're trying to load might already be blocked.
	 * This method will tell you.
	 *
	 * @param mapName The internal Name of the Map. Not the Map-File-Name!
	 * @return boolean is the given Map blocked true/false. Duh.
	 */
	public synchronized static boolean isBlocked(String mapName) {
		return blockedMaps.stream().anyMatch(m -> m.equals(mapName));
	}

	/**
	 * Returns a List of internal Map-Names which are marked as blocked and can't be used for EditorSessions until release.
	 *
	 * @return List of Strings
	 */
	public synchronized static List<String> getBlockedMaps() {
		return new ArrayList<>(blockedMaps);
	}

	private static List<String> filterBlockedMaps(String mapName) {
		return blockedMaps.parallelStream()
				.filter(m -> m.equals(mapName))
				.collect(Collectors.toList());
	}

	/**
	 * We're holding all maps cached and you can of course have them.
	 *
	 * @return HashMap of Internal Map-Names and the actual Map-Object
	 */
	public static java.util.Map<String, Map> getMapDictionary() {
		return mapDictionary;
	}
}
