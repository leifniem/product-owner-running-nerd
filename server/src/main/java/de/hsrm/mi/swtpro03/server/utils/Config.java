package de.hsrm.mi.swtpro03.server.utils;

import de.hsrm.mi.swtpro03.server.Server;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;

public class Config {

	private static final Environment DEFAULT_ENVIRONMENT = Environment.PRODUCTION;
	private static final int DEFAULT_PORT = 61616;
	private static final int FIRST_NOT_RESERVED_PORT = 1024;

	private static final Logger LOGGER = LogManager.getLogger(Server.class);

	public static int getPort() {
		String customPort = System.getProperty("port");

		if (customPort == null)
			return DEFAULT_PORT;

		Integer newPort = getIntegerFromString(customPort);

		if (newPort == null) {
			LOGGER.fatal("The custom port \"{}\" is not a number.", customPort);
			System.exit(1);
		}

		if (newPort < FIRST_NOT_RESERVED_PORT) {
			LOGGER.fatal("The custom port \"{}\" is below the reserved port. The first not reserved port is {}", newPort, FIRST_NOT_RESERVED_PORT);
			System.exit(1);
		}

		return newPort;
	}

	public static Environment getEnvironment() {
		String customEnvironment = System.getProperty("environment");

		if (customEnvironment == null)
			return DEFAULT_ENVIRONMENT;

		Environment newEnvironment = getEnvironmentFromString(customEnvironment);

		if (newEnvironment != null)
			return newEnvironment;

		LOGGER.fatal("The environment \"{}\" could not be found. List of available environments: {}", customEnvironment, Environment.getAllEnvironments());
		System.exit(1);

		// Unreachable
		return null;
	}

	private static Integer getIntegerFromString(String customPort) {
		try {
			return Integer.parseInt(customPort);
		} catch (NumberFormatException ignored) {
			return null;
		}
	}

	private static Environment getEnvironmentFromString(String customEnvironment) {
		try {
			return Environment.valueOf(customEnvironment);
		} catch (Exception ignored) {
			return null;
		}
	}

}
