using Hackathonlib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace HackathonNetwork
{
    class Program : Client
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

        override public void UpdateScene(List<GameObject> scene)
        {
            // TODO put code here
        }

        override public string GetCommand()
        {
            // TODO put code here
            return null;
        }

        override public string GetName()
        {
            // TODO put code here
            return null;
        }

        override public double? GetSpeed()
        {
            // TODO put code here
            return null;
        }

        override public double? GetRotation()
        {
            // TODO put code here
            return null;
        }

        override public bool DoPing()
        {
            // TODO put code here
            return false;
        }
    }
}
