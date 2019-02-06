package de.hsrm.mi.swtpro03.server;

import de.hsrm.mi.swtpro03.server.game.session.management.MapManager;
import de.hsrm.mi.swtpro03.server.game.session.management.SessionManager;
import de.hsrm.mi.swtpro03.server.game.session.management.UserManager;
import de.hsrm.mi.swtpro03.server.network.NetworkService;
import de.hsrm.mi.swtpro03.server.utils.Config;
import de.hsrm.mi.swtpro03.server.utils.Environment;
import org.apache.logging.log4j.LogManager;
import org.apache.logging.log4j.Logger;
import org.apache.logging.log4j.core.config.Configurator;

import java.io.IOException;

public class Server {

	private static UserManager userManager;
	private static SessionManager sessionManager;
	private static final String GAME_NAME = "Product Owner: Running Nerd";

	private static final Logger LOGGER = LogManager.getLogger(Server.class);

	public static void main(String[] args) {

		LOGGER.info(GAME_NAME + " is starting...");

		Environment environment = Config.getEnvironment();
		LOGGER.info("Environment set to {}", environment.name());

		Configurator.setRootLevel(environment.getLevel());

		int port = Config.getPort();
		LOGGER.info("Port set to {}", port);

		NetworkService.start(port);

		userManager = UserManager.getInstance();
		sessionManager = SessionManager.getInstance();

		try {
			MapManager.loadAllMaps();
		} catch (IOException e) {
			System.err.println("Failed to load existing maps, starting without any maps..");
			e.printStackTrace();
		}
	}
}

