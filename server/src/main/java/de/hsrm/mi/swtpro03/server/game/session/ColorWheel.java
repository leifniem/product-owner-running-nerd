package de.hsrm.mi.swtpro03.server.game.session;

import java.util.Stack;

/**
 * A ring data structure for rotative picking of colors from SpriteColor
 */
public class ColorWheel {

	/**
	 * A stack containing values of SpriteColor
	 */
	private Stack<SpriteColor> colors;

	/**
	 * Constructor for ColorWheel
	 */
	public ColorWheel() {
		reset();
	}

	/**
	 * Resets the ColorWheel by creating a new stack with all values of SpriteColor
	 */
	private void reset() {
		colors = new Stack<>();
		for (int i = SpriteColor.values().length - 1; i >= 0; i--) {
			colors.push(SpriteColor.values()[i]);
		}
	}

	/**
	 * Returns the top color from the colors-Stack
	 *
	 * @return one of the colors from SpriteColor
	 */
	public SpriteColor pickColor() {
		if (colors.isEmpty()) reset();
		return colors.pop();
	}
}
