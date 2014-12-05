using HackathonLib;
using System.Collections.Generic;

namespace HackathonSample
{
    class Sample : Client
    {
        static void Main()
        {
            // Define hostname and port.
            const string hostname = "localhost";
            const int port = 25555;

            // Open up a tcp connection.
            var client = Network.Connect(hostname, port);
            var connection = new Connection(client, new Sample(), -1, true);

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
            return "C-Sharp Sample";
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
            return true;
        }
    }
}
