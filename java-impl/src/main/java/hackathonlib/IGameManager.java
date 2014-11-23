package hackathonlib;

import java.util.List;

/**
 * This interface must be implemented by servers and clients.
 * It specifies how to react on different packets.
 * Some are only relevant for clients some only for servers.
 *
 * @author Michael Fürst
 * @version 1.0
 * @since 23.11.2014
 */
public interface IGameManager {
    /**
     * Notifies a server when a connection is started. (Clients can ignore this.)
     * @param connection The connection.
     */
    void connectionStarted(Connection connection);

    /**
     * Notifies a server when a connection is ready. (Clients can ignore this.)
     * @param connection The connection.
     */
    void connectionReady(Connection connection);

    /**
     * Notifies a server when a connection is lost. (Clients can ignore this.)
     * @param connection The connection.
     */
    void connectionLost(Connection connection);

    /**
     * Notifies a server when a connection wants to execute a command. (Clients can ignore this.)
     * @param connection The connection.
     * @param p The command.
     */
    void executeCommand(Connection connection, String p);

    /**
     * Notifies a server when a connection wants to change it's speed. (Clients can ignore this.)
     * @param connection The connection.
     * @param p The speed.
     */
    void changeSpeed(Connection connection, double p);

    /**
     * Notifies a server when a connection wants to change it's rotation. (Clients can ignore this.)
     * @param connection The connection.
     * @param p The rotation.
     */
    void changeRotation(Connection connection, double p);

    /**
     * Update the scene. (For clients. Servers should just ignore this.)
     * @param scene The scene.
     */
    void updateScene(List<GameObject> scene);

    /**
     * The scene. (Only for servers.)
     * @return The scene. (Clients should return null.)
     */
    List<GameObject> getScene();

    /**
     * The command to execute.
     * @return The current command.
     */
    String getCommand();

    /**
     * Your name.
     * @return Your name.
     */
    String getName();

    /**
     * Return null if you do not want to set a speed, otherwise return the speed.
     * @return The speed or null.
     */
    Double getSpeed();

    /**
     * Return null if you do not want to set a rotation, otherwise return the rotation.
     * @return The rotation or null.
     */
    Double getRotation();

    /**
     * Do you want to ping the server this frame.
     * Set this true if all other outputs are null, otherwise you will loose your connection.
     * @return True for a ping.
     */
    boolean doPing();
}
