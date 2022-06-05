using System;
using System.IO;
using System.Threading.Tasks;

namespace SgServer
{
    class Console
    {
        public static void Log(string String, bool DisplayTime = true)
        {
            DateTime Time = DateTime.Now;
            if (DisplayTime)
            {
                String = "[" + Time.Hour.ToString("D2") + ":" + Time.Minute.ToString("D2") + ":" + Time.Second.ToString("D2") + "]: " + String;
            }
            using (StreamWriter File = new StreamWriter("./logs/" + Time.Day.ToString("D2") + "-" + Time.Month.ToString("D2") + "-" + Time.Year.ToString("D2") + ".log", true))
            {
                File.WriteLine(String);
            }
            System.Console.WriteLine(String);
        }

        public static void Command()
        {
            while (true)
            {
                string Message = System.Console.ReadLine();
                System.Console.SetCursorPosition(0, System.Console.CursorTop - 1);
                Log("Server issued command: /" + Message);
                string[] Command = Message.Split(' ');

                switch (Command[0].ToLower())
                {
                    case ("help"):
                    {
                        Log("Available Commands:");
                        Log("/help /stop /stats");
                        break;
                    }
                    case ("stop"):
                    {
                        ClientServer.Esc();
                        break;
                    }
                    case ("stats"):
                    {
                        Log("Current Players: " + ClientServer.PlayersConnected + "/" + ClientServer.PlayersLimit + ".");
                        break;
                    }
                    case ("ping"):
                    {
                        if(Command.Length > 1)
                        {
                            if (int.TryParse(Command[1], out int PlayerId))
                            {
                                if (PlayerId < 0 || PlayerId > ClientServer.BufferLimit - 1)
                                {
                                    Log("Error: PlayerId must be between 0 and " + (ClientServer.BufferLimit - 1));
                                }
                                else
                                {
                                    Task.Run(() => ClientServer.Send(PlayerId, new string[] { "Ping!" }));
                                }
                            }
                            else
                            {
                                Log("Error: PlayerId must be a number.");
                            }
                        }
                        else
                        {
                            Log("Error: Not enough parameters. Use: /ping <PlayerId>");
                        }
                        break;
                    }
                    default:
                    {
                        Log("Error: Command not found. Use: /help");
                        break;
                    }
                }
            }
        }
    }
}
