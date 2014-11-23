using Hackathonlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace HackathonNetwork
{
    class Program : IGameManager
    {
        static void Main(string[] args)
        {
            // Define hostname and port.
            string hostname = "localhost";
            int port = 25555;

            // Open up a tcp connection.
            TcpClient client = new TcpClient(hostname, port);
            Connection connection = new Connection(client, new Program(), -1);

            // Start the client loop.
            connection.StartClientLoop();

            // Join the connection since it's callback based.
            connection.Join();
        }

        void IGameManager.ConnectionStarted(Connection connection)
        {
            throw new NotImplementedException();
        }

        void IGameManager.ConnectionReady(Connection connection)
        {
            throw new NotImplementedException();
        }

        void IGameManager.ConnectionLost(Connection connection)
        {
            throw new NotImplementedException();
        }

        void IGameManager.ExecuteCommand(Connection connection, string p)
        {
            throw new NotImplementedException();
        }

        void IGameManager.ChangeSpeed(Connection connection, double p)
        {
            throw new NotImplementedException();
        }

        void IGameManager.ChangeRotation(Connection connection, double p)
        {
            throw new NotImplementedException();
        }

        void IGameManager.UpdateScene(List<GameObject> scene)
        {
            throw new NotImplementedException();
        }

        List<GameObject> IGameManager.GetScene()
        {
            throw new NotImplementedException();
        }

        string IGameManager.GetCommand()
        {
            throw new NotImplementedException();
        }

        string IGameManager.GetName()
        {
            throw new NotImplementedException();
        }

        double? IGameManager.GetSpeed()
        {
            throw new NotImplementedException();
        }

        double? IGameManager.GetRotation()
        {
            throw new NotImplementedException();
        }

        bool IGameManager.DoPing()
        {
            throw new NotImplementedException();
        }
    }
}
