using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hackathonlib
{
    public interface IGameManager
    {
        void ConnectionStarted(Connection connection);

        void ConnectionReady(Connection connection);

        void ConnectionLost(Connection connection);

        void ExecuteCommand(Connection connection, string p);

        void ChangeSpeed(Connection connection, double p);

        void ChangeRotation(Connection connection, double p);

        void UpdateScene(List<GameObject> scene);

        List<GameObject> GetScene();

        string GetCommand();

        string GetName();

        double? GetSpeed();

        double? GetRotation();

        bool DoPing();
    }
}
