package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.dtos.GameCharacterDTO;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;

import java.util.concurrent.TimeUnit;

import org.python.core.PyCode;
import org.python.util.PythonInterpreter;

/**
 * A GameCharacter controlled by a python script, that runs within a jython interpreter.
 * The python script is only allowed to set input-states (like "left button pressed").
 * The GameCharacter behaves in the exact same way like a PlayerGameCharacter,
 * when it comes to things like physics and movement.
 *
 * @author tschn002, lvonk001
 */
public class EnemyGameCharacter extends GameCharacter {

	public static final boolean LIMIT_KI_FPS = true;
	public static final float KI_FPS = 10;
	public static final float KI_FRAMETIME_NANOS = TimeUnit.SECONDS.toNanos(1) / KI_FPS;
	public static final int RANGE_OF_SIGHT = 5;
	private static final String RUNCODE = "update()";

	private PythonInterpreter py;
	private PyCode defcode;
	private PyCode runcode;

	private float lastAIUpdate;

	private ProxyGameState proxy;


	/**
	 * Create a new EnemyGameCharacter
	 *
	 * @param gridX      Start location X-Coordinate
	 * @param gridY      Start location >-Coordinate
	 * @param map        The map on which the EnemyGameCharacter exists
	 * @param inputState An input state object, that is used by the python script, to control the character
	 * @param session    The GameSession that owns this character
	 * @param defcode    The python code, that defines an update() function for this EnemyGameCharacter
	 */
	public EnemyGameCharacter(int gridX, int gridY, Map map, InputState inputState, GameSession session, String defcode) {
		super(session, gridX, gridY, map, inputState);

		this.proxy = new ProxyGameState(session, this, map);

		py = new PythonInterpreter();
		this.defcode = py.compile(defcode);
		this.runcode = py.compile(RUNCODE);

		py.set("InputState", inputState);
		py.set("ProxyGameState", proxy);

		py.exec(this.defcode);
		lastAIUpdate = System.nanoTime();
	}


	/**
	 * Executes the python script to set input states and calls the parent classes udapte() method,
	 * to perform movement and physics calculations.
	 */
	@Override
	public void update(long deltaTime) {
		super.rememberState();

		if (!LIMIT_KI_FPS || (float) (System.nanoTime() - lastAIUpdate) > KI_FRAMETIME_NANOS) {
			lastAIUpdate = System.nanoTime();
			py.exec(runcode);
		}

		super.update(deltaTime);
		super.notifyAboutChanges();
	}


	/**
	 * Traps enemy inside hole if he falls inside one.
	 */
	protected void trapInsideHole() {
		Tile t1 = map.getTile((int) getCenter().x, (int) getCenter().y);
		if (t1 == Tile.DESTROYED_SOLID) {
			moveToTile((int) getCenter().x, (int) getCenter().y);
			isTrapped = true;
		}
	}


	/**
	 * Create a GameCharacterDTO object to transfer state-changes to the clients.
	 */
	@Override
	public GameCharacterDTO toGameCharacterDTO() {
		GameCharacterDTO gameCharacterDTO = super.toGameCharacterDTO();
		gameCharacterDTO.setEnemy(true);
		return gameCharacterDTO;
	}

}
