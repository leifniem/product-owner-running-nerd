package de.hsrm.mi.swtpro03.server.session;

import de.hsrm.mi.swtpro03.server.game.session.ColorWheel;
import de.hsrm.mi.swtpro03.server.game.session.SpriteColor;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import static org.junit.jupiter.api.Assertions.assertEquals;

public class ColorWheelTest {

	private ColorWheel colorWheel;

	@BeforeEach
	void setup() {
		colorWheel = new ColorWheel();
	}

	@Test
	@DisplayName("Having 8 colors, 1st and 9th color should be the same color")
	void pickColor_ColorIsSameAsTheFirst() {
		SpriteColor firstColor = colorWheel.pickColor();
		SpriteColor lastColor = null;
		for (int i = SpriteColor.values().length; i >= 1; i--) {
			lastColor = colorWheel.pickColor();
		}
		assertEquals(firstColor, lastColor);
	}
}
