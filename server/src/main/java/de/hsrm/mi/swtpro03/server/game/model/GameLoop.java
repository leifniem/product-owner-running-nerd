package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.session.GameSession;
import de.hsrm.mi.swtpro03.server.game.session.User;

import java.util.Map;
import java.util.HashMap;
import java.util.List;
import java.util.concurrent.TimeUnit;


public class GameLoop extends Thread {
	private static final float FPS = 60f;
	private static final float FRAME_TIME_NANOS = TimeUnit.SECONDS.toNanos(1) / FPS;

	private static final boolean DEBUG_FRAMETIME = false;
	int samples = 0;
	long total = 0;

	private boolean isRunning = true;
	private GameSession ownerSession;
	private Map<User, PlayerGameCharacter> userGameCharacterHashMap;
	private de.hsrm.mi.swtpro03.server.game.model.Map map;
	private HashMap<String, List<Object>> gameChanges;


	/**
	 * Creates a new GameLoop instance which extends Thread.
	 *
	 * @param ownerSession GameSession to which this GameLoop belongs too
	 * @param map          Map which will be played on in this GameSession
	 */
	public GameLoop(GameSession ownerSession, de.hsrm.mi.swtpro03.server.game.model.Map map) {
		this.ownerSession = ownerSession;
		this.userGameCharacterHashMap = ownerSession.getUserGameCharacterHashMap();
		this.gameChanges = ownerSession.getGameChanges();
		this.map = map;
	}


	/**
	 * Calls update method of every GameObject or Map (Classes that implement
	 * the Updateable interface).
	 *
	 * @param deltaTime time since last update in millis
	 */
	private void updateGameObjects(long deltaTime) {
		userGameCharacterHashMap.values().forEach(gameCharacter -> gameCharacter.update(deltaTime));
		map.update(deltaTime);
		List<EnemyGameCharacter> enemies = ownerSession.getEnemies();
		enemies.forEach(enemy -> enemy.update(deltaTime));
	}


	/**
	 * Starts game loop and calls updateGameObjects every frame. Calculates the time
	 * needed for the update calls and sleeps the remaining frametime to prevent
	 * updates from occuring more than 60 times per second.
	 */
	private void startGameLoop() {
		long timeLastFrame = System.nanoTime();

		while (isRunning) {
			long timeBeforeUpdate = System.nanoTime();
			updateGameObjects(TimeUnit.NANOSECONDS.toMillis(timeBeforeUpdate - timeLastFrame));

			boolean atLeastOneListIsNotEmpty = gameChanges.values().stream().anyMatch(list -> !list.isEmpty());
			if (atLeastOneListIsNotEmpty) {
				ownerSession.onFrameFinished();
			}
			long timeAfterUpdate = System.nanoTime();

			timeLastFrame = timeBeforeUpdate;
			long timeElapsedForUpdate = timeAfterUpdate - timeBeforeUpdate;

			if (DEBUG_FRAMETIME) {
				samples++;
				total += timeElapsedForUpdate;
				if (samples > 300) {
					System.out.println(String.format("Average Frametime: %.2f ms", (float) (total / samples) / 1000000));
					samples = 0;
					total = 0;
				}
			}

			if (timeElapsedForUpdate < (FRAME_TIME_NANOS)) {
				try {
					Thread.sleep(TimeUnit.NANOSECONDS.toMillis((long) FRAME_TIME_NANOS - timeElapsedForUpdate));
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}
		}
	}


	@Override
	public void run() {
		startGameLoop();
	}


	@Override
	public void interrupt() {
		isRunning = false;
	}

}
