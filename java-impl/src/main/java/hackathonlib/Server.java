package hackathonlib;

import java.util.List;

/**
 * @author PenguinMenace
 * @version 1.0
 * @since 23.11.2014
 */
public abstract class Server implements IGameManager {
    @Override
    public final void updateScene(List<GameObject> scene) {
    }

    @Override
    public final String getCommand() {
        return null;
    }

    @Override
    public final String getName() {
        return null;
    }

    @Override
    public final Double getSpeed() {
        return null;
    }

    @Override
    public final Double getRotation() {
        return null;
    }

    @Override
    public final boolean doPing() {
        return false;
    }
}
