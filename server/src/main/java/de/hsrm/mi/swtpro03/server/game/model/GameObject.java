package de.hsrm.mi.swtpro03.server.game.model;

import java.util.Observable;
import java.util.UUID;

public abstract class GameObject extends Observable {
	private final UUID uuid;
	private final Vector2 position;
	private final float width;
	private final float height;


	/**
	 * @param gridX  x-coordinate of the tile the GameObject should start on
	 * @param gridY  y-coordinate of the tile the GameObject should start on
	 * @param width  width of the GameObject
	 * @param height height of the GameObject
	 */
	GameObject(int gridX, int gridY, float width, float height) {
		this.uuid = UUID.randomUUID();
		float posX = gridX + (1.0f - width) / 2;
		float posY = gridY + (1.0f - width) / 2;
		this.position = new Vector2(posX, posY);
		this.width = width;
		this.height = height;
	}


	/**
	 * Checks if the axis aligned bounding boxes of this and other overlap.
	 *
	 * @param other GameObject to check collision with
	 * @return true if a collision occurred
	 */
	boolean collides(GameObject other) {
		return collidesOnX(other) && collidesOnY(other);
	}


	private boolean collidesOnX(GameObject other) {
		return Math.abs(this.position.x - other.position.x) < (this.width / 2 + other.width / 2);
	}


	private boolean collidesOnY(GameObject other) {
		return Math.abs(this.position.y - other.position.y) < (this.height / 2 + other.height / 2);
	}


	public void setPosition(Vector2 newPosition) {
		this.position.x = newPosition.x;
		this.position.y = newPosition.y;
	}

	
	public Vector2 getPosition() {
		return position;
	}


	public Vector2 getCenter() {
		return new Vector2(position.x + width / 2, position.y + height / 2);
	}


	public float getWidth() {
		return width;
	}


	public float getHeight() {
		return height;
	}


	public UUID getUuid() {
		return uuid;
	}

}
