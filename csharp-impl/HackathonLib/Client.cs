using System;
using System.Collections.Generic;

namespace Hackathonlib
{
    public abstract class Client : IGameManager
    {
        public void ConnectionStarted(Connection connection)
        {
            // Do nothing
        }

        public void ConnectionReady(Connection connection)
        {
            // Do nothing
        }

        public void ConnectionLost(Connection connection)
        {
            // Do nothing
        }

        public void ExecuteCommand(Connection connection, String p)
        {
            // Do nothing
        }

        public void ChangeSpeed(Connection connection, double p)
        {
            // Do nothing
        }

        public void ChangeRotation(Connection connection, double p)
        {
            // Do nothing
        }

        public List<GameObject> GetScene()
        {
            // Do nothing
            return null;
        }


        public abstract void UpdateScene(List<GameObject> scene);

        public abstract string GetCommand();

        public abstract string GetName();

        public abstract double? GetSpeed();

        public abstract double? GetRotation();

        public abstract bool DoPing();
    }
}
