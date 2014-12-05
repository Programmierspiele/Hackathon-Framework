using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hackathonlib
{
    /// <summary>
    /// A class that encapsulates a connection listener.
    /// </summary>
    public class Network
    {
        private int port;
        private IGameManager gameManager;
        private int receiveTimeout;

        private TcpListener listener;
        private List<Connection> threads = new List<Connection>();
        private bool running = true;
        private Thread th;

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
                catch (Exception)
                {
                }
            }
            return client;
        }

        /// <summary>
        /// Start to listen for incoming connections.
        /// </summary>
        /// <param name="port">Listen on this port.</param>
        public void StartListen(int port, IGameManager gameManager, int receiveTimeout)
        {
            this.port = port;
            this.gameManager = gameManager;
            this.receiveTimeout = receiveTimeout;

            th = new Thread(new ThreadStart(Run));
            th.Start();
        }

        private void Run()
        {
            try
            {
                listener = new TcpListener(port);
                listener.Start();
            }
            catch (Exception)
            {
                Console.Error.WriteLine("Cannot open port...");
            }

            while (running)
            {
                try
                {
                    // Wartet auf eingehenden Verbindungswunsch
                    TcpClient c = listener.AcceptTcpClient();
                    if (c == null) continue;
                    // Initialisiert und startet einen Server-Thread
                    // und fügt ihn zur Liste der Server-Threads hinzu
                    threads.Add(new Connection(c, gameManager, receiveTimeout, false));
                }
                catch (Exception)
                {
                    // Do nothing..
                }
            }

            listener.Stop();
            listener = null;

            // Alle Server-Threads stoppen
            foreach (var t in threads)
            {
                t.Quit();
            }
        }

        /// <summary>
        /// Stop listening on the network.
        /// </summary>
        public void EndListen()
        {
            running = false;
            if (listener != null)
            {
                var conn = new TcpClient("localhost", port);
                conn.Close();
            }
        }
    }
}
