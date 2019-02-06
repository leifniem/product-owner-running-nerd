package de.hsrm.mi.swtpro03.server.game.model;

public enum Tile {
	EMPTY, SOLID, DESTROYABLE_SOLID, DESTROYED_SOLID, LADDER, ENEMY;

	public static float SIZE = 1.0f;

	public boolean isDestroyed() {
		return this == DESTROYED_SOLID;
	}

	public boolean isSolid() {
		return this == SOLID || this == DESTROYABLE_SOLID;
	}

	public boolean isLadder() {
		return this == LADDER;
	}

}
