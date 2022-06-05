using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SgServer
{
    class ClientServer
    {
        // - - - - - Settings - - - - - // 

        public static string Domain = "localhost";
        public static string IpAddress = "192.168.1.8";
        public static int Port = 12347;
        public static TcpListener Server;
        public static string[] Splitter = { " | [Tq] | " };

        // - - - - - Clients - - - - - //

        public static int PlayersLimit = 1000;
        public static int BufferLimit = 1024;
        public static TcpClient[] PlayersConnections = new TcpClient[BufferLimit];
        public static string[] PlayersNicknames = new string[BufferLimit];
        public static int LastPlayerId = 0;
        public static int PlayersConnected = 0;

        // - - - - - Server - - - - - //

        public static void Main()
        {
            try
            {

                if (!Directory.Exists("./logs/"))
                {
                    Directory.CreateDirectory("./logs/");
                }

                Console.Log(@"                                                                ", false);
                Console.Log(@"      _____                  _____                              ", false);
                Console.Log(@"     / ____|                / ____|                             ", false);
                Console.Log(@"    | (___   __ _   _____  | (___   ___ _ ____   _____ _ __     ", false);
                Console.Log(@"     \___ \ / _' | |_____|  \___ \ / _ \ '__\ \ / / _ \ '__|    ", false);
                Console.Log(@"     ____) | (_| |           ____) |  __/ |   \ V /  __/ |      ", false);
                Console.Log(@"    |_____/ \__, |          |_____/ \___|_|    \_/ \___|_|      ", false);
                Console.Log(@"             __/ |                                              ", false);
                Console.Log(@"            |___/                                               ", false);
                Console.Log(@"                                                                ", false);
                Console.Log(@"                                                                ", false);

                Task.Run(() => Console.Command());

                Console.Log("Launching Simple Games Server.");

                for (int i = 0; i < BufferLimit; i++)
                {
                    PlayersConnections[i] = null;
                    PlayersNicknames[i] = "";
                }

                Games.FillLobby();

                Task.Run(() => Games.CheckLobbies());

                Console.Log("Connecting to database...");

                Database.OpenConnections();

                Console.Log("Waiting for connections...");

                try
                {
                    Server = new TcpListener(IPAddress.Parse(IpAddress), Port);
                    Server.Start();
                }
                catch (Exception Ex)
                {
                    Console.Log(Ex.ToString());
                }

                while (true)
                {
                    if (PlayersConnected < PlayersLimit)
                    {
                        while (PlayersConnections[LastPlayerId] != null)
                        {
                            LastPlayerId++;
                            if (LastPlayerId >= BufferLimit)
                            {
                                LastPlayerId = 0;
                            }
                        }
                        Task<TcpClient> Client = Server.AcceptTcpClientAsync();
                        while (!Client.IsCompleted)
                        {
                            Thread.Sleep(10);
                        }
                        PlayersConnections[LastPlayerId] = Client.Result;

                        Task Handle = Task.Run(() => Listen(LastPlayerId));
                        while (Handle.Status < TaskStatus.Running)
                        {
                            Thread.Sleep(10);
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch(Exception Ex)
            {
                Console.Log(Ex.ToString());
            }
        }
        
        public static void Esc()
        {
            Console.Log("Stopping connections...");
            for (int i = 0; i < PlayersConnections.Length; i++)
            {
                if(PlayersConnections[i] != null)
                {
                    PlayersConnections[i].Close();
                }
            }
            Server.Stop();
            Console.Log("Disconnecting from database...");
            Database.CloseConnections();
            Console.Log("Stopping server...");
            Environment.Exit(PlayersConnected);
        }

        static void Listen(int PlayerId)
        {
            try
            {
                Console.Log("Player with id " + PlayerId + " has connected.");
                PlayersConnected++;

                PlayersConnections[PlayerId].ReceiveBufferSize = 25600;
                PlayersConnections[PlayerId].SendBufferSize = 25600;

                PlayersNicknames[PlayerId] = "";
                byte[] Buffer = new byte[25600];
                Stream Stream = PlayersConnections[PlayerId].GetStream();


                int Length = 0;
                while ((Length = Stream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    Receive(PlayerId, Encryption.Decrypt(Buffer.Take(Length).ToArray()));
                }

                if (PlayersNicknames[PlayerId].Length > 1)
                {
                    DateTime LastSeen = DateTime.Now;
                    Database.Set("Users", "LastSeen = '" + LastSeen.Year + "-" + LastSeen.Month + "-" + LastSeen.Day + " " + LastSeen.Hour + "-" + LastSeen.Minute + "-" + LastSeen.Second + "', Status = '0'", "Username = '" + PlayersNicknames[PlayerId] + "'");
                }

                PlayersConnections[PlayerId].Close();
                PlayersConnections[PlayerId] = null;
                PlayersNicknames[PlayerId] = null;

                Console.Log("Player with id " + PlayerId + " has disconnected.");
                PlayersConnected--;
            }
            catch(Exception Ex)
            {
                Console.Log(Ex.ToString());
            }
        }

        public static void Send(int PlayerId, string[] Messages)
        {
            if (PlayerId >= 0 && PlayerId <= BufferLimit)
            {
                if (PlayersConnections[PlayerId] != null)
                {
                    if (PlayersConnections[PlayerId].Connected)
                    {
                        string Message = "";
                        for (int i = 0; i < Messages.Length; i++)
                        {
                            Message += Messages[i] + Splitter[0];
                        }
                        Stream Stream = PlayersConnections[PlayerId].GetStream();
                        byte[] Buffer = Encryption.Encrypt(Message + "X");
                        Stream.Write(Buffer, 0, Buffer.Length);
                        Console.Log("Message send to player " + PlayerId + ": " + Message.Replace(Splitter[0], "|") + Messages.Length + ".");
                    }
                }
            }
        }

        public static void Receive(int PlayerId, string Message)
        {
            Console.Log("Message recieved from player " + PlayerId + ": " + Message.Replace(Splitter[0], "|") + ".");
            string[] Messages = Message.Split(Splitter, StringSplitOptions.None);
            Array.Resize(ref Messages, Messages.Length - 1);
            if(Messages.Length > 1)
            {
                switch (Messages[0])
                {
                    case ("Account"):
                    {
                        Account.NetworkAction(PlayerId, Messages);
                        break;
                    }
                    case ("Games"):
                    {
                        Games.NetworkAction(PlayerId, Messages);
                        break;
                    }
                    case ("Friends"):
                    {
                        Friends.NetworkAction(PlayerId, Messages);
                        break;
                    }
                }
            }
        }
    }
}
