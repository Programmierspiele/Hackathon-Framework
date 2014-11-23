using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hackathonlib
{
    public abstract class Server : IGameManager
    {
        public void UpdateScene(List<GameObject> scene)
        {
        }

        public string GetCommand()
        {
            return null;
        }

        public string GetName()
        {
            return null;
        }

        public double? GetSpeed()
        {
            return null;
        }

        public double? GetRotation()
        {
            return null;
        }

        public bool DoPing()
        {
            return false;
        }

        public abstract void ConnectionStarted(Connection connection);

        public abstract void ConnectionReady(Connection connection);

        public abstract void ConnectionLost(Connection connection);

        public abstract void ExecuteCommand(Connection connection, string p);

        public abstract void ChangeSpeed(Connection connection, double p);

        public abstract void ChangeRotation(Connection connection, double p);

        public abstract List<GameObject> GetScene();
    }
}
