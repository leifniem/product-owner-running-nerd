package de.hsrm.mi.swtpro03.server.network.messages.game;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonProperty;

public class PlayerCommandMessage {

	public static final String TYPE = "PlayerCommandMessage";

	private PressedKey pressedKey;

	public PlayerCommandMessage() {
	}

	public PlayerCommandMessage(@JsonProperty("pressedKey") String pressedKey) {
		this.pressedKey = PressedKey.valueOf(pressedKey);
	}

	public PressedKey getPressedKey() {
		return pressedKey;
	}

	@JsonFormat(shape = JsonFormat.Shape.OBJECT)
	public enum PressedKey {
		LEFT,
		RIGHT,
		UP,
		DOWN,
		JUMP,
		DIG_LEFT,
		DIG_RIGHT,
		ENERGY
	}
}
