package de.hsrm.mi.swtpro03.server.game.model;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.*;

class MapTest {

	private Map map;

	@BeforeEach
	void setUp() {
		map = new Map(1, "test", new Vector2Int(0, 0));
	}

	@Test
	void addTile_getTile_equalTile() {
		map.addTile(0, 0, Tile.LADDER);
		assertEquals(Tile.LADDER, map.getTile(0, 0));
	}

	@Test
	void removeTile_getTileReturnsEmpty() {
		map.removeTile(0, 0);
		assertEquals(Tile.EMPTY, map.getTile(0, 0));
	}

	@Test
	void addItem_getItem_equalItem() {
		map.addItem(0, 0, Item.PIZZA);
		assertEquals(Item.PIZZA, map.getItem(0, 0));
	}

	@Test
	void removeItem_getItemReturnsEmpty() {
		map.addItem(0, 0, Item.PIZZA);
		assertEquals(Item.PIZZA, map.getItem(0, 0));
		map.removeItem(0, 0);
		assertEquals(Item.EMPTY, map.getItem(0, 0));
	}

	@Test
	void destroyTile_itemDestroyedAndRestored() {
		assertEquals(Tile.EMPTY, map.getTile(0, 1));
		map.addTile(0, 1, Tile.DESTROYABLE_SOLID);
		assertEquals(Tile.DESTROYABLE_SOLID, map.getTile(0, 1));
		map.destroyTile(0, 1);
		assertEquals(Tile.DESTROYED_SOLID, map.getTile(0, 1));
		map.update(5001);
		assertEquals(Tile.DESTROYABLE_SOLID, map.getTile(0, 1));
	}


}