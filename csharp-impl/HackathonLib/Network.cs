using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;

namespace HackathonLib
{
    /// <summary>
    /// A class that encapsulates a connection listener.
    /// </summary>
    public class Network
    {
        private int _port;
        private IGameManager _gameManager;
        private int _receiveTimeout;

        private TcpListener _listener;
        private readonly List<Connection> _threads = new List<Connection>();
        private bool _running = true;
        private Thread _th;

        /// <summary>
        /// Connect to the server. Automatically retries when not successfull.
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static TcpClient Connect(string hostname, int port)
        {
            TcpClient client = null;
            while (client == null)
            {
                try
                {
                    client = new TcpClient(hostname, port);
                }
                catch (SocketException)
                {
                    Console.WriteLine("Cannot reach host.");
                    Thread.Sleep(1000);
                }
            }
            return client;
        }

        /// <summary>
        /// Start to listen for incoming connections.
        /// </summary>
        /// <param name="port">Listen on this port.</param>
        /// <param name="gameManager">The gamemanager used for callbacks.</param>
        /// <param name="receiveTimeout">The timeout allowed.</param>
        public void StartListen(int port, IGameManager gameManager, int receiveTimeout)
        {
            _port = port;
            _gameManager = gameManager;
            _receiveTimeout = receiveTimeout;

            _th = new Thread(Run);
            _th.Start();
        }

        private void Run()
        {
            try
            {
// ReSharper disable once CSharpWarnings::CS0618
                _listener = new TcpListener(_port);
                _listener.Start();
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Cannot open port...");
            }

            while (_running)
            {
                try
                {
                    // Accept incoming connections.
                    _threads.Add(new Connection(_listener.AcceptTcpClient(), _gameManager, _receiveTimeout, false));
                }
                catch (SocketException)
                {
                    // Do nothing..
                }
            }

            _listener.Stop();
            _listener = null;

            // Stop all connections.
            foreach (var t in _threads)
            {
                t.Quit();
            }
        }

        /// <summary>
        /// Stop listening on the network.
        /// </summary>
        public void EndListen()
        {
            _running = false;
            if (_listener == null) return;
            var conn = new TcpClient("localhost", _port);
            conn.Close();
        }
    }
}
