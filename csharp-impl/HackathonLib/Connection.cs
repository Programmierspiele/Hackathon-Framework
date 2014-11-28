using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hackathonlib
{
    public class Connection
    {
        public string name { get; private set; }

        // Stop-Flag
        private bool stop = false;
        // Die Verbindung zum Client
        private TcpClient connection = null;

        // Der Ausgabestream
        private Stream outStream;
        // Der Eingabestream
        private StreamReader inStream;
        // Der Spielkoordinator
        private IGameManager gameManager;
        // The worker thread.
        private Thread thread;

        // Speichert die Verbindung zum Client und startet den Thread
        public Connection(TcpClient connection, IGameManager gameManager, int receiveTimeout)
        {
            this.name = "none";
            this.gameManager = gameManager;
            this.connection = connection;

            // Setze das receive timeout. Für server wichtig...
            if (receiveTimeout > 0)
            {
                connection.ReceiveTimeout = receiveTimeout;
            }

            // Hole den Stream für's schreiben
            outStream = connection.GetStream();
            inStream = new StreamReader(connection.GetStream());

            // Initialisiert und startet den Thread
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        /// <summary>
        /// Start the client loop. Only call this client side.
        /// </summary>
        public void StartClientLoop()
        {
            var ping = new HackathonPacket();
            ping.ping = "ping";
            Write(ping);
        }

        private void Write(HackathonPacket message)
        {
            // Wandele den String in ein Byte-Array um
            // Es wird noch ein Carriage-Return-Linefeed angefügt
            // so daß das Lesen auf Client-Seite einfacher wird
            Byte[] sendBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message) + "\r\n");
            // Sende die Bytes zum Client
            outStream.Write(sendBytes, 0, sendBytes.Length);
        }

        private HackathonPacket Read()
        {
            // Zeilenweise lesen von der Eingabe
            return JsonConvert.DeserializeObject<HackathonPacket>(inStream.ReadLine());
        }

        // Der eigentliche Thread
        public void Run()
        {
            bool loop = true;
            gameManager.ConnectionStarted(this);

            while (loop)
            {
                try
                {
                    Thread.Sleep(16);
                    Write(Handle(Read()));

                    // Wiederhole die Schleife so lange bis von außen der Stopwunsch kommt
                    loop = !stop;
                }
                catch (Exception)
                {
                    // oder bis ein Fehler aufgetreten ist
                    loop = false;
                }
            }
            gameManager.ConnectionLost(this);

            // Schließe die Verbindung zum Client
            this.connection.Close();
        }

        public void Quit()
        {
            stop = true;
        }

        public void Join()
        {
            thread.Join();
        }

        private HackathonPacket Handle(HackathonPacket packet)
        {
            HackathonPacket result = new HackathonPacket();

            if (packet.name != null)
            {
                if (name == "none" && packet.name != "none")
                {
                    name = packet.name;
                    gameManager.ConnectionReady(this);
                }
            }
            if (packet.command != null)
            {
                gameManager.ExecuteCommand(this, packet.command);
            }
            if (packet.speed != null)
            {
                gameManager.ChangeSpeed(this, packet.speed.Value);
            }
            if (packet.rotation != null)
            {
                gameManager.ChangeRotation(this, packet.rotation.Value);
            }
            if (packet.scene != null)
            {
                gameManager.UpdateScene(packet.scene);
            }

            // Prepare output...
            if (packet.ping != null && packet.ping == "ping")
            {
                result.ping = "pong";
            }

            if (packet.ping == null && gameManager.DoPing())
            {
                result.ping = "ping";
            }

            if (name == "none" && "none" != gameManager.GetName())
            {
                result.name = gameManager.GetName();
            }

            result.command = gameManager.GetCommand();
            result.speed = gameManager.GetSpeed();
            result.rotation = gameManager.GetRotation();
            result.scene = gameManager.GetScene();

            return result;
        }
    }
}
