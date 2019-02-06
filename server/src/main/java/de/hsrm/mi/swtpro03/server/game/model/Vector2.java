package de.hsrm.mi.swtpro03.server.game.model;

public class Vector2 {
	public float x;
	public float y;

	// Default constructor for jackson
	public Vector2() {
	}


	/**
	 * 2 dimensional Vector which holds two float values. Used as representation
	 * of a point in 2D space.
	 *
	 * @param x x coordinate of a point represented by this vector.
	 * @param y y coordinate of a point represented by this vector.
	 */
	public Vector2(float x, float y) {
		this.x = x;
		this.y = y;
	}


	@Override
	public boolean equals(Object obj) {
		if (this == obj) return true;
		if (obj instanceof Vector2) {
			Vector2 vec = (Vector2) obj;
			return this.x == vec.x && this.y == vec.y;
		}
		return false;
	}


	public Vector2 deepCopy() {
		return new Vector2(x, y);
	}


	@Override
	public String toString() {
		return String.format("x: %f, y: %f", x, y);
	}

}
