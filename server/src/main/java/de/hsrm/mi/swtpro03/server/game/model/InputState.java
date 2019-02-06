package de.hsrm.mi.swtpro03.server.game.model;


public class InputState {
	private boolean upKeyPressed;
	private boolean downKeyPressed;
	private boolean leftKeyPressed;
	private boolean rightKeyPressed;
	private boolean jumpKeyPressed;
	private boolean energyDrinkKeyPressed;
	private boolean digLeftKeyPressed;
	private boolean digRightKeyPressed;

	public boolean isUpKeyPressed() {
		return upKeyPressed;
	}

	public void setUpKeyPressed(boolean upKeyPressed) {
		this.upKeyPressed = upKeyPressed;
	}

	public boolean isDownKeyPressed() {
		return downKeyPressed;
	}

	public void setDownKeyPressed(boolean downKeyPressed) {
		this.downKeyPressed = downKeyPressed;
	}

	public boolean isLeftKeyPressed() {
		return leftKeyPressed;
	}

	public void setLeftKeyPressed(boolean leftKeyPressed) {
		this.leftKeyPressed = leftKeyPressed;
	}

	public boolean isRightKeyPressed() {
		return rightKeyPressed;
	}

	public void setRightKeyPressed(boolean rightKeyPressed) {
		this.rightKeyPressed = rightKeyPressed;
	}

	public boolean isJumpKeyPressed() {
		return jumpKeyPressed;
	}

	public void setJumpKeyPressed(boolean jumpKeyPressed) {
		this.jumpKeyPressed = jumpKeyPressed;
	}

	public boolean isEnergyDrinkKeyPressed() {
		return energyDrinkKeyPressed;
	}

	public void setEnergyDrinkKeyPressed(boolean energyDrinkKeyPressed) {
		this.energyDrinkKeyPressed = energyDrinkKeyPressed;
	}

	public boolean isDigLeftKeyPressed() {
		return digLeftKeyPressed;
	}

	public void setDigLeftKeyPressed(boolean digLeftKeyPressed) {
		this.digLeftKeyPressed = digLeftKeyPressed;
	}

	public boolean isDigRightKeyPressed() {
		return digRightKeyPressed;
	}

	public void setDigRightKeyPressed(boolean digRightKeyPressed) {
		this.digRightKeyPressed = digRightKeyPressed;
	}

	public String toString() {
		return "|UP: " + upKeyPressed + " DOWN: " + downKeyPressed + "; LEFT: " + leftKeyPressed + "; RIGHT: " + rightKeyPressed + "; JUMP: " + jumpKeyPressed + "|";
	}

	@Override
	public boolean equals(Object o) {
		if (this == o) return true;
		if (o == null || getClass() != o.getClass()) return false;
		InputState that = (InputState) o;
		return upKeyPressed == that.upKeyPressed &&
				downKeyPressed == that.downKeyPressed &&
				leftKeyPressed == that.leftKeyPressed &&
				rightKeyPressed == that.rightKeyPressed &&
				jumpKeyPressed == that.jumpKeyPressed &&
				energyDrinkKeyPressed == that.energyDrinkKeyPressed &&
				digLeftKeyPressed == that.digLeftKeyPressed &&
				digRightKeyPressed == that.digRightKeyPressed;
	}

}
