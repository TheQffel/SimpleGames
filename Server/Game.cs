using System.Reflection.Metadata;

namespace SgServer
{
    class Game
    {
        public static string NameToData(string Name)
        {
            string Data = "";
            switch (Name)
            {
                case ("Billard"): { Data = ""; break; }
                case ("Cards"): { Data = ""; break; }
                case ("Chess"): { Data = ""; break; }
                case ("CoinToss"): { Data = "4 4 4 4 4 4 4 4 4 4"; break; }
                case ("Dice"): { Data = "8 8 8 8 8 8 8 8 8 8"; break; }
                case ("Domino"): { Data = ""; break; }
                case ("Maze"): { Data = ""; break; }
                case ("Puzzle"): { Data = ""; break; }
                case ("Quiz"): { Data = ""; break; }
                case ("Snake"): { Data = ""; break; }
                case ("TicTacToe"): { Data = "0 0 0 0 0 0 0 0 0"; break; }
            }
            return Data;
        }

        public static string MoveToData(string GameName, int PlayerId, string Data, string Move)
        {
            string DataMove = "";

            string[] Datas = Data.Split(' ');
            string[] Moves = Move.Split(' ');

            switch (GameName)
            {
                case ("CoinToss"):
                {
                    bool EmptySlot = false;
                    for (int i = 0; i < Datas.Length; i++)
                    {
                        if (int.Parse(Datas[i]) == 4)
                        {
                            Datas[i] = Move;
                            EmptySlot = true;
                            break;
                        }
                    }

                    if (!EmptySlot)
                    {
                        return null;
                    }
                    break;
                }
                case ("Dice"):
                {
                    bool EmptySlot = false;
                    for (int i = 0; i < Datas.Length; i++)
                    {
                        if (int.Parse(Datas[i]) == 8)
                        {
                            Datas[i] = Move;
                            EmptySlot = true;
                            break;
                        }
                    }

                    if (!EmptySlot)
                    {
                        return null;
                    }
                    break;
                }
                case ("TicTacToe"):
                {
                    int Value = int.Parse(Moves[0]);
                    if (int.Parse(Datas[Value]) > 0)
                    {
                        return null;
                    }
                    else
                    {
                        Datas[Value] = PlayerId.ToString();
                    }
                    break;
                }
            }

            for (int i = 0; i < Datas.Length; i++)
            {
                DataMove += Datas[i] + " ";
            }
            return DataMove.Substring(0, DataMove.Length - 1);
        }

        public static string[] Score(string GameId)
        {
            string[] GameData = Database.Get("Games", "Game, Player1, Player2, Player3, Player4, Player5, Player6, Player7, Player8, State, Data", "Id = '" + GameId + "'")[0];
            string[] Results = new string[8];

            for (int i = 0; i < 8; i++)
            {
                Results[i] = GameData[i + 1] + " ";
            }

            /*switch (GameData[0])
            {
                case ("CoinToss"):
                {
                    break;
                }
                case ("Dice"):
                {
                    break;
                }
                case ("TicTacToe"):
                {
                    break;
                }
            }*/

            for (int i = 0; i < 8; i++)
            {
                if (Results[i].Length > 1)
                {
                    if (i != 0)
                    {
                        if (i == int.Parse(GameData[9]) * -1)
                        {
                            Results[i] += "WIN";
                        }
                        else
                        {
                            if (i < 0)
                            {
                                Results[i] += "LOSE";
                            }
                        }
                    }
                    else
                    {
                        Results[i] += "DRAW";
                    }
                }
            }

            return Results;
        }

        public static string Reward(string GameId, string Username)
        {
            return "101 | 99";
        }

        public static int GameOver(string GameName, int State, string Data)
        {
            string[] Datas = Data.Split(' ');

            switch (GameName)
            {
                case ("CoinToss"):
                {
                    if (int.Parse(Datas[Datas.Length - 1]) < 4)
                    {
                        State = 0;
                        int PlayerA = int.Parse(Datas[0]) + int.Parse(Datas[2]) + int.Parse(Datas[4]) + int.Parse(Datas[6]) + int.Parse(Datas[8]);
                        int PlayerB = int.Parse(Datas[1]) + int.Parse(Datas[3]) + int.Parse(Datas[5]) + int.Parse(Datas[7]) + int.Parse(Datas[9]);

                        if (PlayerA > PlayerB)
                        {
                            State = -1;
                        }
                        if (PlayerA < PlayerB)
                        {
                            State = -2;
                        }
                    }
                    break;
                }
                case ("Dice"):
                {
                    if (int.Parse(Datas[Datas.Length - 1]) < 8)
                    {
                        State = 0;
                        int PlayerA = int.Parse(Datas[0]) + int.Parse(Datas[2]) + int.Parse(Datas[4]) + int.Parse(Datas[6]) + int.Parse(Datas[8]);
                        int PlayerB = int.Parse(Datas[1]) + int.Parse(Datas[3]) + int.Parse(Datas[5]) + int.Parse(Datas[7]) + int.Parse(Datas[9]);

                        if (PlayerA > PlayerB)
                        {
                            State = -1;
                        }
                        if (PlayerA < PlayerB)
                        {
                            State = -2;
                        }
                    }
                    break;
                }
                case ("TicTacToe"):
                {
                    bool Draw = true;
                    for (int i = 0; i < 9; i++)
                    {
                        if (int.Parse(Datas[i]) < 1)
                        {
                            Draw = false;
                        }
                    }
                    if (Draw)
                    {
                        State = 0;
                    }
                    if (int.Parse(Datas[0]) == int.Parse(Datas[1]) && int.Parse(Datas[1]) == int.Parse(Datas[2]) && int.Parse(Datas[1]) > 0)
                    {
                        State = int.Parse(Datas[1]) * (-1);
                    }
                    if (int.Parse(Datas[3]) == int.Parse(Datas[4]) && int.Parse(Datas[4]) == int.Parse(Datas[5]) && int.Parse(Datas[4]) > 0)
                    {
                        State = int.Parse(Datas[4]) * (-1);
                    }
                    if (int.Parse(Datas[6]) == int.Parse(Datas[7]) && int.Parse(Datas[7]) == int.Parse(Datas[8]) && int.Parse(Datas[7]) > 0)
                    {
                        State = int.Parse(Datas[7]) * (-1);
                    }
                    if (int.Parse(Datas[0]) == int.Parse(Datas[3]) && int.Parse(Datas[3]) == int.Parse(Datas[6]) && int.Parse(Datas[3]) > 0)
                    {
                        State = int.Parse(Datas[3]) * (-1);
                    }
                    if (int.Parse(Datas[1]) == int.Parse(Datas[4]) && int.Parse(Datas[4]) == int.Parse(Datas[7]) && int.Parse(Datas[4]) > 0)
                    {
                        State = int.Parse(Datas[4]) * (-1);
                    }
                    if (int.Parse(Datas[2]) == int.Parse(Datas[5]) && int.Parse(Datas[5]) == int.Parse(Datas[8]) && int.Parse(Datas[5]) > 0)
                    {
                        State = int.Parse(Datas[5]) * (-1);
                    }
                    if (int.Parse(Datas[0]) == int.Parse(Datas[4]) && int.Parse(Datas[4]) == int.Parse(Datas[8]) && int.Parse(Datas[4]) > 0)
                    {
                        State = int.Parse(Datas[4]) * (-1);
                    }
                    if (int.Parse(Datas[2]) == int.Parse(Datas[4]) && int.Parse(Datas[4]) == int.Parse(Datas[6]) && int.Parse(Datas[4]) > 0)
                    {
                        State = int.Parse(Datas[4]) * (-1);
                    }
                    break;
                }
            }
            return State;
        }
    }
}
