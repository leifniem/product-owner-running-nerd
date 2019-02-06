package de.hsrm.mi.swtpro03.server.utils;

import org.apache.logging.log4j.Level;

import java.util.Arrays;

public enum Environment {
    XTREME_DEBUG(Level.TRACE),
    DEBUG(Level.DEBUG),
    PRODUCTION(Level.INFO);

    private Level level;

    Environment(Level level) {
        this.level = level;
    }

    public Level getLevel() {
        return level;
    }

    public static String[] getAllEnvironments() {
        return Arrays.stream(Environment.class.getEnumConstants()).map(Enum::name).toArray(String[]::new);
    }
}
