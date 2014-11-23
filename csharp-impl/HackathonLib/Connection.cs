using SimpleJSON;
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
        // Flag für "Thread läuft"
        private bool running = false;
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
            Write("{\"ping\":\"ping\"}");
        }

        private void Write(string message)
        {
            // Wandele den String in ein Byte-Array um
            // Es wird noch ein Carriage-Return-Linefeed angefügt
            // so daß das Lesen auf Client-Seite einfacher wird
            Byte[] sendBytes = Encoding.ASCII.GetBytes(message + "\r\n");
            // Sende die Bytes zum Client
            outStream.Write(sendBytes, 0, sendBytes.Length);
        }

        private string Read()
        {
            // Zeilenweise lesen von der Eingabe
            return inStream.ReadLine();
        }

        // Der eigentliche Thread
        public void Run()
        {
            // Setze Flag für "Thread läuft"
            this.running = true;
            bool loop = true;
            gameManager.ConnectionStarted(this);

            while (loop)
            {
                try
                {
                    Thread.Sleep(16);
                    var result = Handle(JSON.Parse(Read()));
                    Write(result);

                    // Wiederhole die Schleife so lange bis von außen der Stopwunsch kommt
                    loop = !stop;
                }
                catch (Exception e)
                {
                    // oder bis ein Fehler aufgetreten ist
                    loop = false;
                }
            }
            gameManager.ConnectionLost(this);

            // Schließe die Verbindung zum Client
            this.connection.Close();
            // Setze das Flag "Thread läuft" zurück
            this.running = false;
        }

        public void Quit()
        {
            stop = true;
        }

        public void Join()
        {
            thread.Join();
        }

        private string Handle(JSONNode jsonObject)
        {
            if (jsonObject["name"] != null)
            {
                var tmp = jsonObject["name"].Value;
                if (name == "none" && tmp != "none")
                {
                    name = tmp;
                    gameManager.ConnectionReady(this);
                }
            }
            if (jsonObject["command"] != null)
            {
                gameManager.ExecuteCommand(this, jsonObject["command"].Value);
            }
            if (jsonObject["speed"] != null)
            {
                gameManager.ChangeSpeed(this, jsonObject["speed"].AsDouble);
            }
            if (jsonObject["rotation"] != null)
            {
                gameManager.ChangeRotation(this, jsonObject["rotation"].AsDouble);
            }
            if (jsonObject["scene"] != null)
            {
                // Calculate a scene object.
                List<GameObject> scene = new List<GameObject>();
                foreach (var tmp in jsonObject["scene"].AsArray)
                {
                    JSONNode obj = tmp as JSONNode;
                    // Not sure if this works...
                    var extra = new List<object>();
                    foreach (var t in obj["extra"].AsArray)
                    {
                        extra.Add(t);
                    }
                    // Add the object to the scene.
                    scene.Add(new GameObject(obj["name"].Value, obj["x"].AsDouble, obj["y"].AsDouble, obj["z"].AsDouble, obj["rotation"].AsDouble, extra));
                }
                gameManager.UpdateScene(scene);
            }

            string result = "{";

            bool topSeparatorRequired = false;

            if (jsonObject["ping"] != null && jsonObject["ping"].Value == "ping")
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"ping\":\"pong\"";
            }

            if (jsonObject["ping"] == null && gameManager.DoPing())
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"ping\":\"ping\"";
            }

            var command = gameManager.GetCommand();
            if (command != null)
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"command\":\"" + command + "\"";
            }

            if (gameManager.GetName() != null && name == "none" && "none" != gameManager.GetName())
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                name = gameManager.GetName();
                result += "\"name\":\"" + gameManager.GetName() + "\"";
            }

            var speed = gameManager.GetSpeed();
            if (speed != null)
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"speed\":\"" + speed + "\"";
            }

            var rotation = gameManager.GetRotation();
            if (rotation != null)
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"rotation\":\"" + rotation + "\"";
            }

            List<GameObject> outScene = gameManager.GetScene();
            if (outScene != null)
            {
                if (topSeparatorRequired)
                {
                    result += ",";
                }
                else
                {
                    topSeparatorRequired = true;
                }
                result += "\"scene\":[";
                bool separatorRequired = false;

                foreach (var obj in outScene)
                {
                    if (separatorRequired) {
                        result += ",";
                    } else {
                        separatorRequired = true;
                    }
                    result += "{";
                    result += "\"name\":\"" + obj.name + "\",";
                    result += "\"x\":\"" + obj.x + "\",";
                    result += "\"y\":\"" + obj.y + "\",";
                    result += "\"z\":\"" + obj.z + "\",";
                    result += "\"rotation\":\"" + obj.rotation + "\",";
                    result += "\"extra\":[";

                    bool subSeparatorRequired = false;
                    foreach (var ext in obj.extra)
                    {
                        if (subSeparatorRequired)
                        {
                            result += ",";
                        }
                        else
                        {
                            subSeparatorRequired = true;
                        }
                        result += ext.ToString();
                    }

                    result += "]";
                    result += "}";
                }

                result += "]";
            }

            return result + "}";
        }
    }
}
