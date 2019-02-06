package de.hsrm.mi.swtpro03.server.game.model;

import de.hsrm.mi.swtpro03.server.game.dtos.GameCharacterDTO;
import de.hsrm.mi.swtpro03.server.game.session.GameSession;
import de.hsrm.mi.swtpro03.server.game.session.User;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.mock;

class GameCharacterTest {
	private GameCharacter character;
	private GameCharacter other;
	private Map map;
	private InputState inputState;

	@BeforeEach
	void setUp(){
		map = new Map(1, "test", new Vector2Int(0, 0));

		GameSession session = mock(GameSession.class);
		map.addTile(1, 2, Tile.SOLID);
		map.addTile(2, 1, Tile.SOLID);
		map.addTile(0, 1, Tile.SOLID);
		map.addTile(5, 5, Tile.LADDER);
		map.addTile(5, 4, Tile.LADDER);
		map.addTile(5, 3, Tile.LADDER);
		map.addTile(15, 15, Tile.SOLID);
		inputState = new InputState();
		character = new PlayerGameCharacter(session, 1, 0, map, inputState);
		other = new PlayerGameCharacter(session, 1, 0, map, inputState);
	}


	@Test
	void toCharacterDTO_DTOEqualsCharacter() {
		GameCharacterDTO dto = character.toGameCharacterDTO();
		assertEquals(dto.getPosition(), character.getPosition());
		assertSame(dto.getGameCharacterState(), character.getGameCharacterState());
	}

	@Test
	void moveToTile_characterCenteredOnTile() {
		int gridX = 5;
		int gridY = 3;
		character.moveToTile(gridX, gridY);
		assertEquals(gridX + 1.0f / 2, character.getCenter().x);
		assertEquals(gridY + 1.0f / 2, character.getCenter().y);
	}

	@Test
	void update_fallingOnGround() {
		for (int i = 0; i < 60; i++) {
			character.update(16);
		}
		assertEquals(2, character.getPosition().y + character.getHeight());
	}

	@Test
	void update_walkingAgainstWallRight() {
		inputState.setRightKeyPressed(true);
		for (int i = 0; i < 60; i++) {
			character.update(16);
		}
		assertEquals(2, character.getPosition().x + character.getWidth());
	}

	@Test
	void update_walkingAgainstWallLeft() {
		inputState.setLeftKeyPressed(true);
		for (int i = 0; i < 60; i++) {
			character.update(16);
		}
		assertEquals(1, character.getPosition().x);
	}

	@Test
	void update_climbingLadder() {
		character.moveToTile(5, 5);
		inputState.setUpKeyPressed(true);
		for (int i = 0; i < 60; i++) {
			character.update(16);
		}
		assertTrue(character.getPosition().y < 3.5);
	}

	@Test
	void update_jumping() {
		character.moveToTile(15, 14);
		boolean jumped = false;
		for (int i = 0; i < 60; i++) {
			character.update(16);
			inputState.setJumpKeyPressed(true);
			if (character.getPosition().y < 13) jumped = true;
		}
		assertTrue(jumped);
	}

	@Test
	void collides() {
		character.getPosition().x = 0;
		character.getPosition().y = 0;
		other.getPosition().x = 0.699999f;
		other.getPosition().y = 0;
		assertTrue(character.collides(other));
		other.getPosition().x = 0.7f;
		assertFalse(character.collides(other));

		other.getPosition().x = 0;
		other.getPosition().y = 0.999999f;
		assertTrue(character.collides(other));
		other.getPosition().y = 1.0f;
		assertFalse(character.collides(other));
	}
}