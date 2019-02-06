package de.hsrm.mi.swtpro03.server.game.session.exceptions;

public class UserAlreadyInSessionException extends Exception {
	public UserAlreadyInSessionException(String message) {
		super(message);
	}
}
