using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HackathonLib
{
    public class Connection
    {
        public string Name { get; private set; }

        // Stop-Flag
        private bool _stop;
        // The tcp connection to the client.
        private readonly TcpClient _connection;

        // In and output streams
        private readonly Stream _outStream;
        private readonly StreamReader _inStream;
        // The gamemanager callback
        private readonly IGameManager _gameManager;
        // The worker thread
        private readonly Thread _thread;

        /// <summary>
        /// Manage a connection with a client.
        /// </summary>
        /// <param name="connection">The connection with the client.</param>
        /// <param name="gameManager">The game manager callback.</param>
        /// <param name="receiveTimeout">The receive timeout. (0 or smaller means default)</param>
        /// <param name="isClient">Clients have to create connections differently then servers.</param>
        public Connection(TcpClient connection, IGameManager gameManager, int receiveTimeout, bool isClient)
        {
            Name = "none";
            _gameManager = gameManager;
            _connection = connection;

            // Set the receive timeout. (Important for gameservers)
            if (receiveTimeout > 0)
            {
                connection.ReceiveTimeout = receiveTimeout;
            }

            // Initialize streams
            _outStream = connection.GetStream();
            _inStream = new StreamReader(connection.GetStream());

            if (isClient)
            {
                StartClientLoop();
            }

            // Startup the network loop in separate thread.
            _thread = new Thread(Run);
            _thread.Start();
        }

        /// <summary>
        /// Start the client loop. Only call this client side.
        /// </summary>
        private void StartClientLoop()
        {
            var ping = new HackathonPacket {ping = "ping"};
            Write(ping);
        }

        private void Write(HackathonPacket message)
        {
            // Transform the packet into a ASCII string json-encoded.
            var sendBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message) + "\r\n");
            // Send the bytes to the client.
            _outStream.Write(sendBytes, 0, sendBytes.Length);
        }

        private HackathonPacket Read()
        {
            // Read the input line by line and convert it to a json-object.
            var str = _inStream.ReadLine();
            try
            {
                return JsonConvert.DeserializeObject<HackathonPacket>(str);
            }
            catch (JsonException e)
            {
                Console.WriteLine(str + ": " + e);
                throw;
            }
        }

        private void Run()
        {
            var loop = true;
            _gameManager.ConnectionStarted(this);

            while (loop)
            {
                try
                {
                    Thread.Sleep(16);
                    var packet = Read();
                    Write(Handle(packet));

                    // Repeat until a stop is requested.
                    loop = !_stop;
                }
                catch (Exception)
                {
                    // Or any exception occurs.
                    loop = false;
                }
            }
            _gameManager.ConnectionLost(this);

            // Close the connection.
            _connection.Close();
        }

        /// <summary>
        /// Quit the connection loop.
        /// </summary>
        public void Quit()
        {
            _stop = true;
        }

        /// <summary>
        /// Join the network loop.
        /// </summary>
        public void Join()
        {
            _thread.Join();
        }

        private HackathonPacket Handle(HackathonPacket packet)
        {
            var result = new HackathonPacket();

            if (packet.name != null)
            {
                if (Name == "none" && packet.name != "none")
                {
                    Name = packet.name;
                    _gameManager.ConnectionReady(this);
                }
            }
            if (packet.command != null)
            {
                _gameManager.ExecuteCommand(this, packet.command);
            }
            if (packet.speed != null)
            {
                _gameManager.ChangeSpeed(this, packet.speed.Value);
            }
            if (packet.rotation != null)
            {
                _gameManager.ChangeRotation(this, packet.rotation.Value);
            }
            if (packet.scene != null)
            {
                _gameManager.UpdateScene(packet.scene);
            }

            // Prepare output...
            if (packet.ping != null && packet.ping == "ping")
            {
                result.ping = "pong";
            }

            if (packet.ping == null && _gameManager.DoPing())
            {
                result.ping = "ping";
            }

            if (Name == "none" && "none" != _gameManager.GetName() && _gameManager.GetName() != null)
            {
                result.name = _gameManager.GetName();
                Name = result.name;
            }

            result.command = _gameManager.GetCommand();
            result.speed = _gameManager.GetSpeed();
            result.rotation = _gameManager.GetRotation();
            result.scene = _gameManager.GetScene();

            return result;
        }
    }
}
