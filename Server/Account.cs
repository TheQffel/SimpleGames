using System;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace SgServer
{
    class Account
    {
        static string Hash(string Value)
        {
            Value = "A" + Value + "z";
            StringBuilder StringBuilder = new StringBuilder();
            using (SHA256 Hash = SHA256.Create())
            {
                byte[] Result = Hash.ComputeHash(Encoding.UTF8.GetBytes(Value));
                foreach (byte Char in Result)
                {
                    StringBuilder.Append(Char.ToString("x2"));
                }
            }
            string Hashed = StringBuilder.ToString();
            return Hashed.Substring(48, 16) + "q" + Hashed.Substring(32, 16) + "v" + Hashed.Substring(16, 16) + "x" + Hashed.Substring(0, 16);
        }

        static void Register(int PlayerId, string Nickname, string Password, string Email)
        {
            if (IsNicknameValid(Nickname))
            {
                if (IsPasswordValid(Password))
                {
                    if (IsEmailValid(Email) || Email == "")
                    {
                        if (Database.Get("Users", "Password", "Username = '" + Nickname + "'")[0][0].Length > 1)
                        {
                            ClientServer.Send(PlayerId, new string[] { "Account", "Register", "Failed", "UserAlreadyExists" });
                        }
                        else
                        {
                            DateTime RegisterDate = DateTime.Now;
                            Database.Add("Users", "Username, Password, Email, RegisterDate, LastSeen, Status, Coins, Xp", "'" + Nickname + "', '" + Hash(Password) + "', '" + Email + "', '" + RegisterDate.Year + "-" + RegisterDate.Month + "-" + RegisterDate.Day + "', '2000-01-01 1:00:00', '1', '1000', '0'");
                            ClientServer.PlayersNicknames[PlayerId] = Nickname;
                            ClientServer.Send(PlayerId, new string[] { "Account", "Register", "Success" });
                        }
                    }
                    else
                    {
                        ClientServer.Send(PlayerId, new string[] { "Account", "Register", "Failed", "WrongEmail" });
                    }
                }
                else
                {
                    ClientServer.Send(PlayerId, new string[] { "Account", "Register", "Failed", "WrongPassword" });
                }
            }
            else
            {
                ClientServer.Send(PlayerId, new string[] { "Account", "Register", "Failed", "WrongNickname" });
            }
        }

        static void Login(int PlayerId, string Nickname, string Password)
        {
            if (Database.Get("Users", "Password", "Username = '" + Nickname + "'")[0][0] == Hash(Password))
            {
                ClientServer.PlayersNicknames[PlayerId] = Nickname;
                ClientServer.Send(PlayerId, new string[] { "Account", "Login", "Success" });

                DateTime LastSeen = DateTime.Now;
                Database.Set("Users", "LastSeen = '" + LastSeen.Year + "-" + LastSeen.Month + "-" + LastSeen.Day + " " + LastSeen.Hour + ":" + LastSeen.Minute + ":" + LastSeen.Second + "', Status = '1'", "'Username' = '" + Nickname + "'");
            }
            else
            {
                ClientServer.Send(PlayerId, new string[] { "Account", "Login", "Failed", "WrongUsernameOrPassword" });
            }
        }

        static void UpdatePlayerInfo(int PlayerId, string Username = "")
        {
            if (Username.Length > 1)
            {
                string[][] UserInfo = Database.Get("Users", "Username, LastSeen, Coins, Xp", "Username = '" + Username + "'");
                ClientServer.Send(PlayerId, new string[] { "Account", "Update", UserInfo[0][0], UserInfo[0][1], UserInfo[0][2], UserInfo[0][3] });
            }
            else
            {
                string[][] UserInfo = Database.Get("Users", "Username, Email, Coins, Xp", "Username = '" + ClientServer.PlayersNicknames[PlayerId] + "'");
                ClientServer.Send(PlayerId, new string[] { "Account", "Update", "X", UserInfo[0][0], UserInfo[0][1], UserInfo[0][2], UserInfo[0][3] });
            }
        }

        static int[] Levels =
        {
            // 10k = 10 Lvl, 100k = 35 Lvl, 1M = 75 Lvl, 10M = 120 Lvl, 100M = 185 Lvl
            100, 500, 1000, 2000, 3000, 4000, 5500, 7000, 8500, 10000, // 1 - 10
            12000, 14000, 16000, 18000, 20000, 22500, 25000, 27500, 30000, 32500, // 11 - 20
            35000, 37500, 40000, 45000, 50000, 55000, 60000, 65000, 70000, 75000, // 21 - 30
            80000, 85000, 90000, 95000, 100000, 110000, 120000, 130000, 140000, 150000, // 31 - 40
            160000, 170000, 180000, 190000, 200000, 220000, 240000, 260000, 280000, 300000, // 41 - 50
            320000, 340000, 360000, 380000, 400000, 420000, 440000, 460000, 480000, 500000, // 51 - 60
            530000, 560000, 590000, 620000, 650000, 680000, 710000, 740000, 770000, 800000, // 61 - 70
            840000, 880000, 920000, 960000, 1000000, 1050000, 1100000, 1150000, 1200000, 1250000,  // 71 - 80
            1300000, 1350000, 1400000, 1450000, 1500000, 1600000, 1700000, 1800000, 1900000, 2000000, // 81 - 90
            2200000, 2400000, 2600000, 2800000, 3000000, 3200000, 3400000, 3600000, 3800000, 4000000, // 91 - 100
            4250000, 4500000, 4750000, 5000000, 5250000, 5500000, 5750000, 6000000, 6350000, 6650000, // 101 - 110
            7000000, 7350000, 7650000, 8000000, 8350000, 8650000, 9000000, 9350000, 9650000, 10000000, // 111 - 120
            10500000, 11000000, 11500000, 12000000, 12500000, 13000000, 13500000, 14000000, 14500000, 15000000, // 121 - 130
            15500000, 16000000, 16500000, 17000000, 17500000, 18000000, 18500000, 19000000, 19500000, 20000000, // 131 - 140
            21000000, 22000000, 23000000, 24000000, 25000000, 26000000, 27000000, 28000000, 29000000, 30000000, // 141 - 150
            31000000, 32000000, 33000000, 34000000, 35000000, 36000000, 37000000, 38000000, 39000000, 40000000, // 151 - 160
            42000000, 44000000, 46000000, 48000000, 50000000, 52000000, 54000000, 56000000, 58000000, 60000000, // 161 - 170
            62000000, 64000000, 66000000, 68000000, 70000000, 73000000, 76000000, 79000000, 82000000, 85000000, // 171 - 180
            88000000, 91000000, 94000000, 97000000, 100000000, // 181 - 185
            // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // pattern to copy
            999999999, // blockade
        };

        public static int XpToLevel(int Xp)
        {
            int Level;
            for (Level = 0; Level < Levels.Length; Level++)
            {
                if (Levels[Level] > Xp)
                {
                    break;
                }
            }
            return Level;
        }

        public static int LevelToXp(int Level)
        {
            return Levels[Level];
        }

        static bool IsNicknameValid(string Value)
        {
            Regex Pattern = new Regex(@"^[\w.-]+$");
            return (Pattern.IsMatch(Value) && Value.Length >= 4 && Value.Length <= 24);
        }

        static bool IsPasswordValid(string Value)
        {
            Regex PatternA = new Regex(@"^.*[\d]+.*$");
            Regex PatternB = new Regex(@"^.*[\p{Lu}]+.*$");
            Regex PatternC = new Regex(@"^.*[\p{Ll}]+.*$");
            return (PatternA.IsMatch(Value) && PatternB.IsMatch(Value) && PatternC.IsMatch(Value) && Value.Length >= 5 && Value.Length <= 48);
        }

        static bool IsEmailValid(string Value)
        {
            Regex Pattern = new Regex(@"^[\w.-]+\@[\w.-]+\.[\w.-]+$");
            bool MailValid(string Mail) { try { MailAddress x = new MailAddress(Mail); return true; } catch { return false; } }
            return (Pattern.IsMatch(Value) && MailValid(Value) && Value.Length >= 6 && Value.Length <= 96);
        }

        public static void NetworkAction(int PlayerId, string[] Message)
        {
            switch (Message[1])
            {
                case ("Login"):
                {
                    if (Message.Length > 3)
                    {
                        Login(PlayerId, Message[2], Message[3]);
                    }
                    break;
                }
                case ("Register"):
                {
                    if (Message.Length > 4)
                    {
                        Register(PlayerId, Message[2], Message[3], Message[4]);
                    }
                    break;
                }
                case ("Update"):
                {
                    if (Message.Length > 2)
                    {
                        if (Message[2] == "X")
                        {
                            UpdatePlayerInfo(PlayerId);
                        }
                        else
                        {
                            UpdatePlayerInfo(PlayerId, Message[2]);
                        }
                    }
                    break;
                }
            }
        }
    }
}
