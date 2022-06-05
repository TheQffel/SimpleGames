using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Games : MonoBehaviour
{
    public static Games Script;

    void Awake()
    {
        Script = GetComponent<Games>();
    }

    public RectTransform Content;
    public RectTransform EmptyGame;
    public RectTransform NewGame;
    public RectTransform ContentFrame;

    RectTransform[] CurrentGames = new RectTransform[256];

    public string[] GamesData = new string[0];
    public bool Refresh = false;

    float MinSize = -1;
    bool ChangeSize = false;

    async void Start()
    {
        while (!Account.Script.LoggedIn)
        {
            await Task.Run(() => Task.Delay(1000));
        }
        RefreshGames();
    }

    void Update()
    {
        bool Change = ChangeSize;
        if (ContentFrame.sizeDelta.y != MinSize)
        {
            MinSize = ContentFrame.sizeDelta.y;
            ChangeSize = true;
        }
        if (Change)
        {
            NewSize(GamesData.Length);
            ChangeSize = false;
        }
        if (Refresh)
        {
            NewGamesList();
            Refresh = false;
        }
    }

    void RefreshGames()
    {
        ClientServer.Script.Send(new string[] { "Games", "Update", "X" });
    }

    void NewGamesList()
    {
        for (int i = 0; i < CurrentGames.Length; i++)
        {
            if (CurrentGames[i] == null)
            {
                break;
            }
            Destroy(CurrentGames[i].gameObject);
            CurrentGames[i] = null;
        }

        if (GamesData[0].Length > 1)
        {
            for (int i = 0; i < GamesData.Length; i++)
            {
                CurrentGames[i] = Instantiate(EmptyGame, Content);
                Text[] Texts = CurrentGames[i].GetComponentsInChildren<Text>();
                CurrentGames[i].anchoredPosition = new Vector2(CurrentGames[i].anchoredPosition.x, (-100 * i) - 50);
                string[] Data = GamesData[i].Split(' ');
                CurrentGames[i].name = Data[0];

                Texts[0].text = Data[1];
                Texts[1].text = Data[2] + " " + Data[3];
                int PlayersNumber = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (Data[j + 4].Length > 1)
                    {
                        PlayersNumber++;
                    }
                }
                Texts[2].text = PlayersNumber + " Players";
                Texts[3].text = "Draw";
                if (int.Parse(Data[12]) > 0)
                {
                    string CurrentPlayer = Data[int.Parse(Data[12]) + 3];
                    if (CurrentPlayer == "X")
                    {
                        CurrentPlayer = "Computer";
                    }
                    Texts[3].text = CurrentPlayer + "'s Turn";
                }
                if (int.Parse(Data[12]) < 0)
                {
                    string CurrentPlayer = Data[(int.Parse(Data[12]) * (-1)) + 3];
                    if (CurrentPlayer == "X")
                    {
                        CurrentPlayer = "Computer";
                    }
                    Texts[3].text = CurrentPlayer + " Won";
                }
            }
            NewSize(GamesData.Length);
        }
        else
        {
            NewSize(0);
        }
    }

    void NewSize(int GamesCount)
    {
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, (GamesCount + 1) * 100);
        Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, -1000);

        NewGame.anchoredPosition = new Vector2(NewGame.anchoredPosition.x, (-1 * Content.sizeDelta.y) + 50);

        if (Content.sizeDelta.y < MinSize)
        {
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, MinSize);
        }
    }

    public void NetworkAction(string[] Message)
    {
        switch (Message[1])
        {
            case ("Update"):
            {
                if(Message[2] == "X")
                {
                    GamesData = Message[3].Split(new string[] { " | " }, StringSplitOptions.None);
                    Refresh = true;
                }
                else
                {
                    long GameId = long.Parse(Message[2]);
                    if (GameId == Game.Script.Id)
                    {
                        if(Game.Script.Name == "")
                        {
                            Game.Script.StartGame();
                        }

                        Game.Script.Name = Message[3];
                        //Game.Date = Message[4];

                        List<string> Players = new List<string>();

                        for (int i = 5; i < 13; i++)
                        {
                            if(Message[i].Length > 0)
                            {
                                Players.Add(Message[i]);
                                if(Message[i] == Account.Script.Nickname)
                                {
                                    Game.Script.Player = i - 4;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        
                        Debug.Log(Message[10]);
                        Debug.Log(Message[11]);
                        Debug.Log(Message[12]);
                        Debug.Log(Message[13]);
                        Debug.Log(Message[14]);
                        Debug.Log(Message[15]);

                        Game.Script.Players = Players.ToArray();
                        Game.Script.Turn = int.Parse(Message[13]);
                        Game.Script.Data = Message[14].Split(' ');
                        //Game.Script.Backup = Message[15].Split(' ');
                        //Game.Script.Move = Message[15].Split(' ');
                        Game.Script.ShowMove = false;//bool.Parse(Message[17]);
                        Game.Script.Refreshed = false;
                        Game.Script.UpdateNow = true;
                    }
                }
                break;
            }
            case ("Move"):
            {
                if (Message[2] == "Failed")
                {
                    Alert.Script.CreateAlert("Error", "You moved wrong. If you cheat - stop doing that. If not try again or contact support.", false);
                }
                break;
            }
            case ("New"):
            {
                if (Message[2] == "Success")
                {
                    Game.Script.Id = long.Parse(Message[3]);

                    ClientServer.Script.Send(new string[] { "Games", "Update", Message[3] });
                }
                if (Message[2] == "Failed")
                {
                    Alert.Script.CreateAlert("Error", "Failed to create game!", false);
                }
                break;
            }
            case ("Lobby"):
            {
                if (Message[2] == "Success")
                {
                    if (Message[3] == "X")
                    {
                        Lobby.Script.CurrentGame = Message[4];

                        GuiChanger.Script.DoneLoading = true;
                    }
                    else
                    {
                        Game.Script.Id = long.Parse(Message[3]);

                        GuiChanger.Script.SetGuiLoading("Lobby Game");

                        ClientServer.Script.Send(new string[] { "Games", "Update", Message[3] });
                    }
                }
                if (Message[2] == "Failed")
                {
                    if (Message[3] == "LobbyFull")
                    {
                        Alert.Script.CreateAlert("Error", "Lobby is currently full, try again in few seconds. Currently we have low computing power on our servers, becouse game is still in testing stage. Donate to help us.", false);
                    }
                    GuiChanger.Script.CancelLoading = true;
                }
                if (Message[2] == "Update")
                {
                    string[] PlayersCount = Message[4].Split(new string[] { " | " }, StringSplitOptions.None);

                    Lobby.Script.Online.text = PlayersCount[0];
                    Lobby.Script.InGame.text = PlayersCount[1];
                    Lobby.Script.AllLobbies.text = PlayersCount[2];

                    if (Message[3] == Lobby.Script.CurrentGame)
                    {
                        Lobby.Script.InLobby.text = PlayersCount[3];
                    }
                }
                break;
            }
        }
    }
}
