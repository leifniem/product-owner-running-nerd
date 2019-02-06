package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.dtos.*;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;

import java.util.HashMap;

public class PlayerGameCharacter extends GameCharacter {

	private final int ENERGY_EFFECT_DURATION = 7000;
	private final float ENERGY_SPEED_BOOST = 1.5f;
	private final int DIGGING_COOLDOWN = 2000;
	private final int STARTING_LIFE_POINTS = 3;

	private final HashMap<Item, Integer> inventory;
	private int energyDrinkTimer;
	private int diggingCooldownTimer;
	private int lifePoints;
	private boolean isDead;

	private int oldLifePoints;
	private int oldCreditPoints;


	/**
	 * @param session
	 * @param gridX      x-coordinate of the tile the character should start on
	 * @param gridY      y-coordinate of the tile the character should start on
	 * @param map        reference to the map the character is playing on
	 * @param inputState InputState object belonging to the user which is controlling this character
	 */
	public PlayerGameCharacter(GameSession session, int gridX, int gridY, Map map, InputState inputState) {
		super(session, gridX, gridY, map, inputState);
		this.inventory = new HashMap<>();
		this.inventory.put(Item.CREDITPOINTS_5, 0);
		this.inventory.put(Item.CREDITPOINTS_10, 0);
		this.inventory.put(Item.CREDITPOINTS_15, 0);
		this.inventory.put(Item.ENERGYDRINK, 0);
		this.lifePoints = STARTING_LIFE_POINTS;
		this.isDead = false;
	}


	/**
	 * @param deltaTime time since last update in millis.
	 * @see GameCharacter#update(long)
	 */
	@Override
	public void update(long deltaTime) {
		super.rememberState();
		rememberState();

		super.update(deltaTime);
		updateTimers(deltaTime);
		collideWithEnemies();
		if (!isDead) {
			changes.add(pickUpItem());
			changes.add(processInputsForItemUse());
		}
		if (!isTrapped) {
			processInputsForDigging(deltaTime);
		}
		if (oldLifePoints != lifePoints) {
			changes.add(toLifePointsDTO());
		}
		if (oldCreditPoints != getScore()) {
			changes.add(toCreditPointsDTO());
		}

		super.notifyAboutChanges();
		notifyAboutChanges();
	}


	/**
	 * @see GameCharacter#rememberState()
	 */
	protected void rememberState() {
		oldCreditPoints = getScore();
		oldLifePoints = lifePoints;
	}


	/**
	 * @see GameCharacter#notifyAboutChanges()
	 */
	protected void notifyAboutChanges() {
		for (Object change : changes) {
			setChanged();
			notifyObservers(change);
		}
		changes.clear();
	}


	/**
	 * @return Amount of creditpoints the character has collected during the
	 * game to this point.
	 */
	public int getScore() {
		return (5 * inventory.get(Item.CREDITPOINTS_5))
				+ (10 * inventory.get(Item.CREDITPOINTS_10))
				+ (15 * inventory.get(Item.CREDITPOINTS_15));
	}


	/**
	 * Updates the timer for used energydring and sets the velocity multiplier to
	 * boost speed.
	 *
	 * @param deltaTime
	 */
	private void updateTimers(long deltaTime) {
		if (energyDrinkTimer > 0) {
			energyDrinkTimer -= deltaTime;
			setVelocityMultiplier(ENERGY_SPEED_BOOST);
		} else {
			setVelocityMultiplier(1.0f);
		}
	}


	/**
	 * Checks for collision between this character and enemy characters and
	 * removes one lifepoint if collision occured and moves this
	 * GameCharacter to a spawnpoint. If lifepoints reach 0 this GameCharacter dies.
	 */
	private void collideWithEnemies() {
		if (isDead) return;
		for (EnemyGameCharacter enemy : session.getEnemies()) {
			if (this.collides(enemy)) {
				handleDeath();
			}
		}
	}


	/**
	 * Processes the states of input keys from InputState and try to dig tiles in
	 * left or right direction.
	 *
	 * @see InputState
	 */
	private void processInputsForDigging(long deltaTime) {
		if (diggingCooldownTimer > 0) {
			diggingCooldownTimer -= deltaTime;
			return;
		}

		if (inputState.isDigLeftKeyPressed()) {
			map.destroyTile((int) getCenter().x - 1, (int) getCenter().y + 1);
			diggingCooldownTimer = DIGGING_COOLDOWN;
		} else if (inputState.isDigRightKeyPressed()) {
			map.destroyTile((int) getCenter().x + 1, (int) getCenter().y + 1);
			diggingCooldownTimer = DIGGING_COOLDOWN;
		}

		inputState.setDigLeftKeyPressed(false);
		inputState.setDigRightKeyPressed(false);
	}


	/**
	 * Processes the states of input keys from InputState and try to use items
	 * if available in the inventory.
	 *
	 * @see InputState
	 */
	private ItemUsedDTO processInputsForItemUse() {
		ItemUsedDTO itemUsedDTO = null;
		if (inputState.isEnergyDrinkKeyPressed()) {
			int count = inventory.get(Item.ENERGYDRINK);
			if (count > 0) {
				energyDrinkTimer = ENERGY_EFFECT_DURATION;
				inventory.put(Item.ENERGYDRINK, count - 1);
				itemUsedDTO = new ItemUsedDTO(getUuid().toString(), Item.ENERGYDRINK.toString());
			}
		}
		inputState.setEnergyDrinkKeyPressed(false);
		return itemUsedDTO;
	}


	/**
	 * Picks up item from Map at current position and puts it in inventory.
	 * If the item was of type PIZZA, one lifepoint will be added.
	 *
	 * @see Map#pickUpItem(int, int)
	 */
	private ItemPickedUpDTO pickUpItem() {
		int gridX = (int) getCenter().x;
		int gridY = (int) getCenter().y;
		Item item = map.pickUpItem(gridX, gridY);
		if (item == Item.EMPTY) {
			return null;
		}

		if (item == Item.PIZZA) {
			if (lifePoints < 5) {
				lifePoints++;
			}
		} else {
			inventory.put(item, inventory.get(item) + 1);
		}
		ItemDTO itemDTO = new ItemDTO(gridX, gridY, item.name());
		return new ItemPickedUpDTO(getUuid().toString(), itemDTO);
	}


	/**
	 * Traps gamecharacter inside hole if he falls inside one.
	 */
	protected void trapInsideHole() {
		Tile t1 = map.getTile((int) getCenter().x, (int) getCenter().y);
		Tile t2 = map.getTile((int) getCenter().x, (int) getCenter().y + 1);
		if (t1 == Tile.DESTROYED_SOLID && t2 != Tile.EMPTY) {
			moveToTile((int) getCenter().x, (int) getCenter().y);
			isTrapped = true;
		}
	}




	/**
	 * Generates LifePointsDTO made from this character containing
	 * current amount of lifepoints.
	 *
	 * @return new LifePointsDTO
	 */
	public LifePointsDTO toLifePointsDTO() {
		LifePointsDTO lifePointsDTO = new LifePointsDTO();
		lifePointsDTO.setUuid(getUuid().toString());
		lifePointsDTO.setLifePoints(lifePoints);
		return lifePointsDTO;
	}


	/**
	 * Generates CreditPointsDTO made from this character containing
	 * current amount of creditpoints.
	 *
	 * @return new CreditPointsDTO
	 */
	public CreditPointsDTO toCreditPointsDTO() {
		CreditPointsDTO creditPointsDTO = new CreditPointsDTO();
		creditPointsDTO.setUuid(getUuid().toString());
		creditPointsDTO.setCreditPoints(getScore());
		return creditPointsDTO;
	}


	/**
	 * Removes one lifepoint and call super method.
	 *
	 * @see GameCharacter#handleDeath()
	 */
	@Override
	protected void handleDeath() {

		if (lifePoints > 0)
			lifePoints--;

		if (lifePoints == 0 && !isDead)
			isDead = true;

		super.handleDeath();
	}


	public int getLifePoints() {
		return lifePoints;
	}


	public boolean isDead() {
		return isDead;
	}

}
