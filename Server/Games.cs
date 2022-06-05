using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SgServer
{
    class Games
    {
        public static string PlayersInGame = "?";

        static string[] GamesList = { "Billard", "Cards", "Chess", "CoinToss", "Dice", "Domino", "Maze", "Puzzle", "Quiz", "Snake", "TicTacToe" };
        static Dictionary<string, int> GamesLobby = new Dictionary<string, int>();
        static Random Random = new Random();

        public static void FillLobby()
        {
            for (int i = 0; i < GamesList.Length; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    GamesLobby.Add(GamesList[i] + j, -1);
                }
            }
        }

        public static void CheckLobbies()
        {
            while (true)
            {
                for (int i = 0; i < GamesList.Length; i++)
                {
                    int PlayerId = -1;
                    for (int j = 0; j < 10; j++)
                    {
                        if(GamesLobby[GamesList[i] + j] > -1)
                        {
                            if (ClientServer.PlayersConnections[GamesLobby[GamesList[i] + j]] == null)
                            {
                                RemoveFromLobby(GamesList[i], GamesLobby[GamesList[i] + j]);
                            }
                            else
                            {
                                if (PlayerId > -1)
                                {
                                    string Id = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                                    Database.Add("Games", "Id, Game, LastPlayed, Player1, Player2, State, Data", "'" + Id + "', '" + GamesList[i] + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ClientServer.PlayersNicknames[PlayerId] + "', '" + ClientServer.PlayersNicknames[GamesLobby[GamesList[i] + j]] + "', '1', '" + Game.NameToData(GamesList[i]) + "'");

                                    ClientServer.Send(PlayerId, new string[] { "Games", "Lobby", "Success", Id });
                                    ClientServer.Send(GamesLobby[GamesList[i] + j], new string[] { "Games", "Lobby", "Success", Id });

                                    RemoveFromLobby(GamesList[i], PlayerId);
                                    RemoveFromLobby(GamesList[i], GamesLobby[GamesList[i] + j]);

                                    Console.Log("New multiplayer game created!");
                                }
                                else
                                {
                                    PlayerId = GamesLobby[GamesList[i] + j];
                                }
                            }
                        }
                    }
                    Thread.Sleep(100);
                }
            }
        }

        static bool AddToLobby(string GameName, int PlayerId)
        {
            for (int i = 0; i < 10; i++)
            {
                if(GamesLobby[GameName + i] < 0)
                {
                    GamesLobby[GameName + i] = PlayerId;
                    Console.Log("Player with ID: " + PlayerId + " joined lobby " + GameName + " at position " + i + ".");
                    Task.Run(() => Update(GameName));
                    return true;
                }
            }
            return false;
        }

        static void RemoveFromLobby(string GameName, int PlayerId)
        {
            for (int i = 0; i < 10; i++)
            {
                if (GamesLobby[GameName + i] == PlayerId)
                {
                    GamesLobby[GameName + i] = -1;
                    Console.Log("Player with ID: " + PlayerId + " was removed from lobby " + GameName + " from position " + i + ".");
                    Task.Run(() => Update(GameName));
                }
            }
        }

        static void RemoveFromLobbies(int PlayerId)
        {
            for (int i = 0; i < GamesList.Length; i++)
            {
                RemoveFromLobby(GamesList[i], PlayerId);
            }
        }

        static void Update(string GameName)
        {
            string PlayersCount = ClientServer.PlayersConnected + " | " + "?" + " | " + PlayersInLobbies() + " | " + PlayersInLobby(GameName);

            for (int i = 0; i < GamesList.Length; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (GamesLobby[GamesList[i] + j] > -1)
                    {
                        ClientServer.Send(GamesLobby[GamesList[i] + j], new string[] { "Games", "Lobby", "Update", GameName, PlayersCount } );
                    }
                }
            }
        }

        public static int PlayersInLobby(string GameName)
        {
            int Count = 0;
            for (int i = 0; i < 10; i++)
            {
                if (GamesLobby[GameName + i] > -1)
                {
                    Count++;
                }
            }
            return Count;
        }

        public static int PlayersInLobbies()
        {
            int Count = 0;
            for (int i = 0; i < GamesList.Length; i++)
            {
                Count += PlayersInLobby(GamesList[i]);
            }
            return Count;
        }

        public static void RemoveOldGames()
        {
            while(true)
            {
                Thread.Sleep(1000000);
                Database.Del("Games", "State < 1 AND LastPlayed < NOW() - INTERVAL 100 HOUR");
                Thread.Sleep(1000000);
                Database.Del("Games", "State > 0 AND LastPlayed < NOW() - INTERVAL 1000 HOUR");
            }
        }

        public static void NetworkAction(int PlayerId, string[] Message)
        {
            switch (Message[1])
            {
                case ("New"):
                {
                    if (Message.Length > 3)
                    {
                        if (Message[3] == "X")
                        {
                            Message[3] = GamesList[Random.Next(0, GamesList.Length)];
                        }

                        if (Message[2] == "Single")
                        {
                            string Id = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                            Database.Add("Games", "Id, Game, LastPlayed, Player1, State, Data", "'" + Id + "', '" + Message[3] + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ClientServer.PlayersNicknames[PlayerId] + "', '1', '" + Game.NameToData(Message[3]) + "'");
                            ClientServer.Send(PlayerId, new string[] { "Games", "New", "Success", Id });
                        }
                        if (Message[2] == "Computer")
                        {
                            string Id = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                            Database.Add("Games", "Id, Game, LastPlayed, Player1, Player2, State, Data", "'" + Id + "', '" + Message[3] + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ClientServer.PlayersNicknames[PlayerId] + "', 'X', '1', '" + Game.NameToData(Message[3]) + "'");
                            ClientServer.Send(PlayerId, new string[] { "Games", "New", "Success", Id });
                        }
                        if (Message[2] == "Friend")
                        {
                            if (Message.Length > 4)
                            {
                                if (Database.Get("Users", "Username", "Username = '" + Message[4] + "'")[0][0].Length > 1)
                                {
                                    string Id = DateTime.Now.ToString("yyyyMMddHHmmssfffff");
                                    Database.Add("Games", "Id, Game, LastPlayed, Player1, Player2, State, Data", "'" + Id + "', '" + Message[3] + "', '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + ClientServer.PlayersNicknames[PlayerId] + "', '" + Message[4] + "', '1', '" + Game.NameToData(Message[3]) + "'");
                                    ClientServer.Send(PlayerId, new string[] { "Games", "New", "Success", Id });
                                }
                                else
                                {
                                    ClientServer.Send(PlayerId, new string[] { "Games", "New", "Failed", "FriendNotExist" });
                                }
                            }
                        }
                        if (Message[2] == "Multi")
                        {
                            if (AddToLobby(Message[3], PlayerId))
                            {
                                ClientServer.Send(PlayerId, new string[] { "Games", "Lobby", "Success", "X", Message[3] });
                            }
                            else
                            {
                                ClientServer.Send(PlayerId, new string[] { "Games", "Lobby", "Failed", "LobbyFull" });
                            }
                        }
                    }
                    break;
                }
                case ("Update"):
                {
                    if (Message.Length > 2)
                    {
                        if (Message[2] == "X")
                        {
                            string Data = ClientServer.PlayersNicknames[PlayerId];
                            string[][] Games = Database.Get("Games", "Id, Game, LastPlayed, Player1, Player2, Player3, Player4, Player5, Player6, Player7, Player8, State", "Player1 = '" + Data + "' OR Player2 = '" + Data + "' OR Player3 = '" + Data + "' OR Player4 = '" + Data + "' OR Player5 = '" + Data + "' OR Player6 = '" + Data + "' OR Player7 = '" + Data + "' OR Player8 = '" + Data + "'");
                            Data = "";
                            for (int i = 0; i < Games.Length; i++)
                            {
                                for (int j = 0; j < Games[i].Length; j++)
                                {
                                    Data += Games[i][j] + " ";
                                }
                                Data += "| ";
                            }
                            ClientServer.Send(PlayerId, new string[] { "Games", "Update", "X", Data.Substring(0, Data.Length - 3) });
                        }
                        else
                        {
                            string[] Games = Database.Get("Games", "*", "Id = '" + Message[2] + "'")[0];
                            if (Games[0].Length > 1)
                            {
                                ClientServer.Send(PlayerId, new string[] { "Games", "Update" }.Concat(Games).ToArray().Concat(new string[] { "True" }).ToArray());
                            }
                        }
                    }
                    break;
                }
                case ("Lobby"):
                {
                    if (Message.Length > 2)
                    {
                        if (Message[2] == "Leave")
                        {
                            RemoveFromLobbies(PlayerId);
                        }
                    }
                    break;
                }
                case ("Move"):
                {
                    if (Message.Length > 4)
                    {
                        string[] Games = Database.Get("Games", "*", "Id = '" + Message[2] + "'")[0];

                        try
                        {

                            if (Games[0].Length > 1)
                            {
                                int State = int.Parse(Games[11]);
                                int LastPlayer = State;
                                if (State > 0)
                                {
                                    if (Games[State + 2] == ClientServer.PlayersNicknames[PlayerId] || Games[State + 2] == "X") // Player turn
                                    {
                                        string OldData = Games[12];
                                        string Move = Message[3];
                                        string NewData = Game.MoveToData(Games[1], State, OldData, Move);

                                        if (NewData != null)
                                        {
                                            State++;
                                            if (State > 8)
                                            {
                                                State = 1;
                                            }
                                            else
                                            {
                                                if (Games[State + 2].Length < 1)
                                                {
                                                    State = 1;
                                                }
                                            }
                                            State = Game.GameOver(Games[1], State, NewData);

                                            string Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                            Games[2] = Now;
                                            Games[11] = State.ToString();
                                            Games[12] = NewData;
                                            //Games[13] = OldData;
                                            Games[13] = Message[4];

                                            Database.Set("Games", "LastPlayed = '" + Games[2] + "', State = '" + Games[11] + "', Data = '" + Games[12] + "', Backup = '" + Games[13] + "', Move = '" + Games[14] + "'", "Id = '" + Message[2] + "'");

                                            for (int i = 3; i < 11; i++)
                                            {
                                                if (Games[i].Length > 1)
                                                {
                                                    ClientServer.Send(Array.IndexOf(ClientServer.PlayersNicknames, Games[i]), new string[] { "Games", "Update" }.Concat(Games).ToArray().Concat(new string[] { (Games[LastPlayer + 2] != ClientServer.PlayersNicknames[PlayerId]).ToString() }).ToArray());
                                                }
                                            }
                                        }
                                        else
                                        {
                                            ClientServer.Send(PlayerId, new string[] { "Games", "Move", "Wrong" });
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception Ex)
                        {
                            Console.Log(Ex.ToString());
                        }
                    }
                    break;
                }
                case ("Results"):
                {
                    if(Message.Length > 2)
                    {
                        string[] Games = Database.Get("Games", "*", "Id = '" + Message[2] + "'")[0];
                        if (Games[0].Length > 1)
                        {
                            ClientServer.Send(PlayerId, new string[] { "Games", "Results" }.Concat(Games).ToArray().Concat(new string[] { "True" }).ToArray());
                        }
                    }
                    break;
                }
            }
        }
    }
}
