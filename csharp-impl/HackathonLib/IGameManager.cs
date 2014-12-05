using System.Collections.Generic;

namespace HackathonLib
{
    public interface IGameManager
    {
        /// <summary>
        /// Called when a connection is successfully established.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        void ConnectionStarted(Connection connection);

        /// <summary>
        /// Called on servers when a connection is ready to join a game. Means that the connection has a valid playername.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        void ConnectionReady(Connection connection);

        /// <summary>
        /// Called when the connection is lost.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        void ConnectionLost(Connection connection);

        /// <summary>
        /// Called when the other side of the network wishes your implementation to execute the given command.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="command">The command that should be executed.</param>
        void ExecuteCommand(Connection connection, string command);

        /// <summary>
        /// Called when the other side of the network wishes to change the movement speed.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="speed"></param>
        void ChangeSpeed(Connection connection, double speed);

        /// <summary>
        /// Called when the other side of the network wishes to change the rotation.
        /// (Recommended for servers)
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="rotation"></param>
        void ChangeRotation(Connection connection, double rotation);

        /// <summary>
        /// This contains the current scene.
        /// (Recommended for servers)
        /// </summary>
        /// <returns>The current scene or null.</returns>
        List<GameObject> GetScene();

        /// <summary>
        /// Update the scene to fit to the network.
        /// </summary>
        /// <param name="scene">The current scene or null.</param>
        void UpdateScene(List<GameObject> scene);

        /// <summary>
        /// Return a command if one should be transmitted.
        /// </summary>
        /// <returns>The command or null.</returns>
        string GetCommand();

        /// <summary>
        /// Return the name of the player/connection.
        /// </summary>
        /// <returns>The name or null.</returns>
        string GetName();

        /// <summary>
        /// Return the speed if one should be transmitted.
        /// </summary>
        /// <returns>The speed or null.</returns>
        double? GetSpeed();

        /// <summary>
        /// Return the rotation if one should be transmitted.
        /// </summary>
        /// <returns>The rotation or null.</returns>
        double? GetRotation();

        /// <summary>
        /// Return if the ping flag should be set.
        /// </summary>
        /// <returns>If a ping should be executed.</returns>
        bool DoPing();
    }
}
