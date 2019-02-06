package de.hsrm.mi.swtpro03.server.game.model;

public enum Item {
	EMPTY, CREDITPOINTS_5, CREDITPOINTS_10, CREDITPOINTS_15, PIZZA, ENERGYDRINK;


	public boolean isCreditpoints() {
		return this == CREDITPOINTS_5 || this == CREDITPOINTS_10 || this == CREDITPOINTS_15;
	}

}
