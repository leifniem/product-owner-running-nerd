package de.hsrm.mi.swtpro03.server.network.messages.game;


import java.util.List;
import java.util.Map;

public class GameEndMessage {

    public static final String TYPE = "GameEndMessage";

    public static final String WIN_STATUS = "WIN";
    public static final String LOSS_STATUS = "LOSS";
    public static final String QUIT_STATUS = "QUIT";

    private Map<String, Integer> scores;
    private String status;
    private String winnerColor;

    public GameEndMessage() {
    }

    public GameEndMessage(Map<String, Integer> scores, String status, String winnerColor) {
        this.scores = scores;
        this.status = status;
        this.winnerColor = winnerColor;
    }

    public Map<String, Integer> getScores() {
        return scores;
    }

    public void setScores(Map<String, Integer> scores) {
        this.scores = scores;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getWinnerColor() {
        return winnerColor;
    }

    public void setWinnerColor(String winnerColor) {
        this.winnerColor = winnerColor;
    }
}
