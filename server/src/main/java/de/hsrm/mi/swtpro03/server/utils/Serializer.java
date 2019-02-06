package de.hsrm.mi.swtpro03.server.utils;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.fasterxml.jackson.databind.ObjectMapper;
import com.fasterxml.jackson.databind.ObjectReader;
import com.fasterxml.jackson.databind.ObjectWriter;

import java.io.IOException;

/**
 * Serializes and Deserializes Objects to and from a JSON-String
 * @author tschn002
 *
 */
public class Serializer {
	
	private static ObjectMapper jsonMapper = new ObjectMapper();

	/**
	 * Deserializes string to given Object-Type
	 * @param str Incoming string to be deserialized
	 * @param type Type of object that is deserialized
	 * @return The deserialized object
	 */
	public static Object deserialize(String str, Class<?> type){
		Object deserialized = null;
		try {
			ObjectReader objectReader = jsonMapper.readerFor(type);
			deserialized = objectReader.readValue(str);
		} catch (IOException e) {
			System.err.println("[Serializer] Failed to deserialize object of type \"" + type.getName() + "\"");
			e.printStackTrace();
		}
		return deserialized;
	}

	/**
	 * Serializes object to JSON-String
	 * @param object Object to be serialized
	 * @param type The type of Object to be serialized
	 * @return The object serialized to a string
	 */
	public static String serialize(Object object,Class<?> type) {
		String serialized = null;
		try {
			ObjectWriter objectWriter = jsonMapper.writerFor(type);
			serialized = objectWriter.writeValueAsString(object);
		} catch (JsonProcessingException e) {
			System.err.println("[Serializer] Failed to serialize object of type \"" + type.getName() + "\"");
			e.printStackTrace();
		}
		return serialized;
	}
}
