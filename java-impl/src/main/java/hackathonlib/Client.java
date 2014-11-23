package hackathonlib;

import java.util.List;

/**
 * @author PenguinMenace
 * @version 1.0
 * @since 23.11.2014
 */
public abstract class Client implements IGameManager {

    @Override
    public final void connectionStarted(Connection connection) {
        // Do nothing
    }

    @Override
    public final void connectionReady(Connection connection) {
        // Do nothing
    }

    @Override
    public final void connectionLost(Connection connection) {
        // Do nothing
    }

    @Override
    public final void executeCommand(Connection connection, String p) {
        // Do nothing
    }

    @Override
    public final void changeSpeed(Connection connection, double p) {
        // Do nothing
    }

    @Override
    public final void changeRotation(Connection connection, double p) {
        // Do nothing
    }

    @Override
    public final List<GameObject> getScene() {
        // Do nothing
        return null;
    }
}
