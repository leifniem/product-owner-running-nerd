package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.dtos.GameCharacterDTO;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;

import java.util.LinkedList;
import java.util.List;

public abstract class GameCharacter extends GameObject implements Updateable {
	private static final float WIDTH = 0.7f;
	private static final float HEIGHT = 1.0f;
	private static final float NO_LADDER_JUMPING = 0.01f;

	private final float MAX_VELOCITY = 0.005f;
	private final float JUMP_VELOCITY = 0.015f;
	private final float TERMINAL_VELOCITY = 0.015f;
	private final float CLIMBING_VELOCITY = 0.003f;
	private final float ACCELERATION = 0.0002f;
	private final float FRICTION = 0.99f;
	private final float GRAVITY = 0.000075f;

	private final Vector2 velocity = new Vector2();
	private float velocityMultiplier = 1.0f;
	private final Vector2 respawnLocation;

	final GameSession session;
	final InputState inputState;
	final Map map;
	final List<Object> changes = new LinkedList<>();
	boolean isTrapped;

	private GameCharacterState gameCharacterState;
	private boolean isOnGround;
	private boolean isOnLadder;
	private boolean isKilledByTile;

	private Vector2 oldPosition;
	private GameCharacterState oldState;


	/**
	 * @param gridX      x-coordinate of the tile the character should start on
	 * @param gridY      y-coordinate of the tile the character should start on
	 * @param map        reference to the map the character is playing on
	 * @param inputState InputState object belonging to the user which is controlling this character
	 */
	GameCharacter(GameSession session, int gridX, int gridY, Map map, InputState inputState) {
		super(gridX, gridY, WIDTH, HEIGHT);
		this.respawnLocation = getPosition().deepCopy();
		this.session = session;
		this.map = map;
		this.inputState = inputState;
		this.gameCharacterState = GameCharacterState.STANDING;
	}


	/**
	 * Calls all internal update methods and notifies GameSessionEventHandler
	 * about every change that should get transmitted to the Client.
	 *
	 * @param deltaTime time since last update in millis.
	 * @see GameObject
	 * @see de.hsrm.mi.swtpro03.server.handler.GameSessionEventHandler
	 */
	@Override
	public void update(long deltaTime) {
		if (isKilledByTile) {
			handleDeath();
		}
		if (!isTrapped) {
			processInputsForMovement(deltaTime);
		}
		updatePhysics(deltaTime);
		updatePosition(deltaTime);
		trapInsideHole();
		updateCharacterState();
	}


	/**
	 * Creates copies of current position and state to compare
	 * them later too see if anything changed.
	 */
	protected void rememberState() {
		oldPosition = getPosition().deepCopy();
		oldState = gameCharacterState;
	}


	/**
	 * Notifies the GameSessionEventHandler about changes that happened during
	 * update.
	 */
	protected void notifyAboutChanges() {
		if (!oldPosition.equals(getPosition()) || oldState != gameCharacterState) {
			setChanged();
			notifyObservers(this.toGameCharacterDTO());
		}
	}


	/**
	 * Moves GameCharacter back to spawnpoint and resets trapped inside hole flags.
	 */
	protected void handleDeath() {
		setPosition(this.respawnLocation);
		isTrapped = false;
		isKilledByTile = false;
	}


	/**
	 * Processes the states of input keys from InputState and adjust the players
	 * velocity accordingly to create running or jumping movements.
	 *
	 * @param deltaTime time since last update in millis
	 * @see InputState
	 */
	private void processInputsForMovement(long deltaTime) {
		if (inputState.isRightKeyPressed()) {
			velocity.x += (isOnGround ? ACCELERATION : ACCELERATION / 2) * deltaTime;
			if (velocity.x > MAX_VELOCITY) velocity.x = MAX_VELOCITY;
		}
		if (inputState.isLeftKeyPressed()) {
			velocity.x -= (isOnGround ? ACCELERATION : ACCELERATION / 2) * deltaTime;
			if (velocity.x < -MAX_VELOCITY) velocity.x = -MAX_VELOCITY;
		}

		if (isOnLadder) {
			velocity.x = 0.0f;
			velocity.y = 0.0f;

			if (inputState.isUpKeyPressed() && !inputState.isDownKeyPressed())
				velocity.y = -CLIMBING_VELOCITY;
			if (inputState.isRightKeyPressed() && !inputState.isLeftKeyPressed())
				velocity.x = CLIMBING_VELOCITY;
			if (inputState.isLeftKeyPressed() && !inputState.isRightKeyPressed())
				velocity.x = -CLIMBING_VELOCITY;
			if (inputState.isDownKeyPressed() && !inputState.isUpKeyPressed())
				velocity.y = CLIMBING_VELOCITY;
		} else if (isOnGround && inputState.isJumpKeyPressed()) {
			velocity.y = -JUMP_VELOCITY * velocityMultiplier;
		}
		inputState.setJumpKeyPressed(false);
	}


	/**
	 * Applies friction to velocity in x-direction and gravitational acceleration
	 * to velocity in y-direction.
	 *
	 * @param deltaTime time since last update in millis
	 */
	private void updatePhysics(long deltaTime) {
		if (isOnGround) {
			velocity.x *= Math.pow(FRICTION, deltaTime);
			if (Math.abs(velocity.x) < 0.001f) velocity.x = 0.0f;
		}
		if (!isOnLadder) {
			velocity.y += GRAVITY * deltaTime;
			if (velocity.y > TERMINAL_VELOCITY) velocity.y = TERMINAL_VELOCITY;
		}
	}


	private boolean isDestroyedAndSomeoneTrappedInside(Tile tile, int posX, int posY) {
		if (!tile.isDestroyed()) {
			return false;
		}

		return session.isSomeoneTrappedAtTile(posX, posY);
	}

	/**
	 * Calculates new position based on velocity. Checks for and handles collisions
	 * and moves character to new position.
	 *
	 * @param deltaTime time since last update in millis
	 */
	private void updatePosition(long deltaTime) {
		float newX = getPosition().x + velocity.x * deltaTime * velocityMultiplier;
		float newY = getPosition().y + velocity.y * deltaTime;
		Vector2 newPosition = new Vector2(newX, newY);

		if (fixNewPositionOutOfBounds(newPosition)) return;

		if (velocity.x < 0.0f) {
			Tile upperTile = map.getTile((int) newPosition.x, (int) getPosition().y);
			Tile lowerTile = map.getTile((int) newPosition.x, (int) (getPosition().y + HEIGHT - 0.1f));
			if (upperTile.isSolid() || lowerTile.isSolid()) {
				newPosition.x = (int) newPosition.x + 1.0f;
				velocity.x = 0.0f;
			}
		} else {
			Tile upperTile = map.getTile((int) (newPosition.x + WIDTH), (int) getPosition().y);
			Tile lowerTile = map.getTile((int) (newPosition.x + WIDTH), (int) (getPosition().y + HEIGHT - 0.1f));
			if (upperTile.isSolid() || lowerTile.isSolid()) {
				newPosition.x = (int) newPosition.x + (1.0f - WIDTH);
				velocity.x = 0.0f;
			}
		}

		isOnGround = false;
		if (velocity.y < 0.0f) {
			Tile leftTile = map.getTile((int) newPosition.x, (int) newPosition.y);
			Tile rightTile = map.getTile((int) (newPosition.x + WIDTH - 0.1f), (int) newPosition.y);
			if (leftTile.isSolid() || rightTile.isSolid()) {
				newPosition.y = (int) newPosition.y + 1.0f;
				velocity.y = 0.0f;
			}
		} else {
			int posXLeft = (int) newPosition.x;
			int posYLeft = (int) (newPosition.y + HEIGHT);
			int posXRight = (int) (newPosition.x + WIDTH - 0.1f);
			int posYRight = (int) (newPosition.y + HEIGHT);
			Tile leftTile = map.getTile(posXLeft, posYLeft);
			Tile rightTile = map.getTile(posXRight, posYRight);
			if (leftTile.isSolid() || rightTile.isSolid() || isDestroyedAndSomeoneTrappedInside(leftTile, posXLeft, posYLeft) || isDestroyedAndSomeoneTrappedInside(rightTile, posXRight, posYRight)) {
				newPosition.y = (int) newPosition.y + (1.0f - HEIGHT);
				velocity.y = 0.0f;
				isOnGround = true;
			}
		}

		getPosition().x = newPosition.x;
		getPosition().y = newPosition.y;
	}


	abstract void trapInsideHole();

	/**
	 * Updates the current character state depending on movement and position and
	 * surrounding tiles.
	 *
	 * @see GameCharacterState
	 */
	private void updateCharacterState() {
		isOnLadder = onLadder();
		if (isOnLadder) {
			gameCharacterState = GameCharacterState.CLIMBING;
		} else if (isOnGround) {
			if (velocity.x == 0.0f) gameCharacterState = GameCharacterState.STANDING;
			else gameCharacterState = GameCharacterState.RUNNING;
		} else if (velocity.y < 0) {
			gameCharacterState = GameCharacterState.JUMPING;
		} else {
			gameCharacterState = GameCharacterState.FALLING;
		}
	}


	/**
	 * Generates GameCharacterDTO made from this character containing
	 * GameCharacterState and position.
	 *
	 * @return new GameCharacterDTO
	 */
	public GameCharacterDTO toGameCharacterDTO() {
		GameCharacterDTO gameCharacterDTO = new GameCharacterDTO();
		gameCharacterDTO.setUuid(getUuid().toString());
		gameCharacterDTO.setPosition(getPosition().deepCopy());
		gameCharacterDTO.setGameCharacterState(gameCharacterState);
		return gameCharacterDTO;
	}


	/**
	 * Moves character to a specific tile.
	 *
	 * @param gridX x-coordinate of the tile to move character on
	 * @param gridY y-coordinate of the tile to move character on
	 */
	public void moveToTile(int gridX, int gridY) {
		getPosition().x = gridX + (1.0f - getWidth()) / 2;
		getPosition().y = gridY + (1.0f - getHeight()) / 2;
	}


	/**
	 * Checks if the new calculated position lies outside map bounds and corrects
	 * the coordinates of newPosition to be inside Map bounds.
	 *
	 * @param newPosition calculated position that the character should move to
	 */
	private boolean fixNewPositionOutOfBounds(Vector2 newPosition) {
		if (newPosition.x < 0.0f) {
			newPosition.x = 0.0f;
			velocity.x = 0.0f;
		} else if (newPosition.x + WIDTH >= map.getNumberOfSections() * Map.SECTION_WIDTH) {
			newPosition.x = map.getNumberOfSections() * Map.SECTION_WIDTH - WIDTH;
			velocity.x = 0.0f;
		}

		if (newPosition.y < 0.0f) {
			newPosition.y = 0.0f;
			velocity.y = 0.0f;
		} else if (newPosition.y + HEIGHT >= Map.SECTION_HEIGHT) {
			newPosition.y = Map.SECTION_HEIGHT - HEIGHT;
			velocity.y = 0.0f;
			handleDeath();
			return true;
		}
		return false;
	}


	/**
	 * @return true if the center of this GameCharacter is on a ladder tile
	 */
	private boolean onLadder() {
		Tile tile = map.getTile((int) getCenter().x, (int) (getPosition().y + getHeight() - NO_LADDER_JUMPING));
		return tile.isLadder();
	}


	public InputState getInputState() {
		return inputState;
	}


	public GameCharacterState getGameCharacterState() {
		return gameCharacterState;
	}


	public void setKilledByTile(boolean killedByTile) {
		isKilledByTile = killedByTile;
	}


	public void setVelocityMultiplier(float velocityMultiplier) {
		this.velocityMultiplier = velocityMultiplier;
	}

	public boolean isTrapped() {
		return isTrapped;
	}


}