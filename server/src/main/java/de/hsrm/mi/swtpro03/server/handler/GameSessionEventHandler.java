package de.hsrm.mi.swtpro03.server.handler;

import de.hsrm.mi.swtpro03.server.game.dtos.*;
import de.hsrm.mi.swtpro03.server.game.model.GameCharacter;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;
import de.hsrm.mi.swtpro03.server.network.messages.game.*;

import java.util.HashMap;
import java.util.List;
import java.util.Observable;
import java.util.Observer;
import java.util.stream.Collectors;

public class GameSessionEventHandler implements Observer {

	private GameSession owner;
	private GameSessionMessageHandler messageHandler;
	private HashMap<String, List<Object>> gameChanges;

	public GameSessionEventHandler(GameSession owner, GameSessionMessageHandler messageHandler, HashMap<String, List<Object>> gameChanges) {
		this.owner = owner;
		this.messageHandler = messageHandler;
		this.gameChanges = gameChanges;
	}

	@Override
	public void update(Observable observable, Object object) {
		if (object instanceof GameCharacterDTO) {
			gameCharacterChanged((GameCharacterDTO) object);
		} else if (object instanceof LifePointsDTO) {
			lifePointsChanged((LifePointsDTO) object);
		} else if (object instanceof TileChangedMessage) {
			tileChanged((TileChangedMessage) object);
		} else if (object instanceof CreditPointsDTO) {
			creditPointsChanged((CreditPointsDTO) object);
		} else if (object instanceof ItemPickedUpDTO) {
			itemPickedUp((ItemPickedUpDTO) object);
		} else if (object instanceof ItemUsedDTO) {
			itemUsed((ItemUsedDTO) object);
		}
	}

	/**
	 * Event handling for when an item has been used by a player
	 *
	 * @param itemUsedDTO
	 */
	private void itemUsed(ItemUsedDTO itemUsedDTO) {
		ItemUsedMessage itemUsedMessage = new ItemUsedMessage(
				itemUsedDTO.getUuid(),
				itemUsedDTO.getType());

		gameChanges.get("ItemUsedMessages").add(itemUsedMessage);
	}

	/**
	 * Event handling for when an item has been picked up by a player
	 *
	 * @param itemPickedUpDTO
	 */
	private void itemPickedUp(ItemPickedUpDTO itemPickedUpDTO) {
		ItemPickedUpMessage itemPickedUpMessage = new ItemPickedUpMessage(
				itemPickedUpDTO.getUuid(),
				itemPickedUpDTO.getItemDTO());

		gameChanges.get("ItemPickedUpMessages").add(itemPickedUpMessage);
	}

	/**
	 * Event handling for when state of a tile has changed
	 *
	 * @param tileChangedMessage
	 */
	private void tileChanged(TileChangedMessage tileChangedMessage) {
		gameChanges.get("TileChangedMessages").add(tileChangedMessage);
		if (tileChangedMessage.getState().equals(TileChangedMessage.RESTORED_STATE)) {
			List<GameCharacter> trappedCharacters = owner.getUserGameCharacterHashMap()
					.values()
					.stream()
					.filter(gameCharacter -> (int) gameCharacter.getCenter().x == tileChangedMessage.getGridX()
							&& (int) gameCharacter.getCenter().y == tileChangedMessage.getGridY())
					.collect(Collectors.toList());
			trappedCharacters.addAll(owner.getEnemies()
					.stream()
					.filter(gameCharacter -> (int) gameCharacter.getCenter().x == tileChangedMessage.getGridX()
							&& (int) gameCharacter.getCenter().y == tileChangedMessage.getGridY())
					.collect(Collectors.toList()));
			for (GameCharacter trappedCharacter : trappedCharacters) {
				//trappedCharacter.handleLifeLoss();
				trappedCharacter.setKilledByTile(true);
			}
		}
	}

	/**
	 * Event handling for when credit points amount of a player has changed
	 *
	 * @param creditPointsDTO
	 */
	private void creditPointsChanged(CreditPointsDTO creditPointsDTO) {

		owner.checkIfGameEnded();

		CreditPointsUpdateMessage creditPointsUpdateMessage = new CreditPointsUpdateMessage(
				creditPointsDTO.getUuid(),
				creditPointsDTO.getCreditPoints());

		gameChanges.get("CreditPointsUpdateMessages").add(creditPointsUpdateMessage);
	}

	/**
	 * Event handling for when life points amount of a player has changed
	 *
	 * @param lifePointsDTO
	 */
	public void lifePointsChanged(LifePointsDTO lifePointsDTO) {
		if (lifePointsDTO.getLifePoints() == 0) {
			owner.checkIfGameEnded();
		}

		LifePointsUpdateMessage lifePointsUpdateMessage = new LifePointsUpdateMessage(
				lifePointsDTO.getUuid(),
				lifePointsDTO.getLifePoints());

		gameChanges.get("LifePointsUpdateMessages").add(lifePointsUpdateMessage);
	}

	/**
	 * Event handling for when game character state or/and position have changed
	 *
	 * @param gameCharacterDTO
	 */
	private synchronized void gameCharacterChanged(GameCharacterDTO gameCharacterDTO) {
		MoveActorMessage moveActorMessage = new MoveActorMessage(
				gameCharacterDTO.getUuid(),
				gameCharacterDTO.getPosition(),
				gameCharacterDTO.getGameCharacterState());

		gameChanges.get("MoveActorMessages").add(moveActorMessage);
	}

	public void startGameEnding(GameEndMessage gameEndMessage) {
		gameChanges.get("GameEndMessages").add(gameEndMessage);
	}

	public HashMap<String, List<Object>> getChanges() {
		return gameChanges;
	}
}
