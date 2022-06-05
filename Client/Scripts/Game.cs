using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static Game Script;

    void Awake()
    {
        Script = GetComponent<Game>();
    }

    public long Id = -1;
    public string Name = "";
    public string[] Data = { };
    public string[] Backup = { };
    public string[] Move = { };
    public string[] Players = new string[8];
    public int Player = 0; // My Id (1-8)
    public int Turn = 0; // Who playing now, -1 = P1 win, 0 = draw, 1 = P1 turn
    
    public bool UpdateNow = false;
    public bool Refreshed = true;
    public bool ShowMove = true;

    string Selected = "X";
    public Camera MainCamera;

    public Canvas OpponentMove;
    public GameObject[] MoveTexts;

    public void SelectGame(string GameName)
    {
        Selected = GameName;
    }

    public async void New(string Mode)
    {
        UpdateNow = false;
        Name = "";
        Id = -1;

        if (Mode == "Multi")
        {
            GuiChanger.Script.SetGuiLoading("MainMenu Lobby");
        }
        else
        {
            GuiChanger.Script.SetGuiLoading("MainMenu Game");
        }

        await Task.Run(() => Task.Delay(1000));
        if (Mode == "Single")
        {
            ClientServer.Script.Send(new string[] { "Games", "New", "Single", Selected });
        }
        if (Mode == "Computer")
        {
            ClientServer.Script.Send(new string[] { "Games", "New", "Computer", Selected });
        }
        if (Mode == "Friend")
        {
            ClientServer.Script.Send(new string[] { "Games", "New", "Friend", Selected });
        }
        if (Mode == "Multi")
        {
            ClientServer.Script.Send(new string[] { "Games", "New", "Multi", Selected });
            GuiChanger.Script.DoneLoading = true;
        }
    }

    public async void Continue(string GameId)
    {
        GuiChanger.Script.SetGuiLoading(GuiChanger.Script.CurrentGui + " Game");

        UpdateNow = false;
        Name = "";
        Id = long.Parse(GameId);

        await Task.Run(() => Task.Delay(1000));
        ClientServer.Script.Send(new string[] { "Games", "Update", GameId });
    }

    public async void StartGame()
    {
        Console.Log("Game Started");
        await Task.Run(() => Task.Delay(100));
        SceneManager.LoadScene(Name);
        MainCamera.transform.position = new Vector3(0, 75, 0);
        GuiChanger.Script.DoneLoading = true;
    }

    public async void StopGame()
    {
        Alert.Script.CreateAlert("Question", "Leave Game?", true);
        await Task.Run(() => Alert.Script.WaitForAnswer());

        if (Alert.Script.Answer)
        {
            Id = -1;
            Name = "";
            UpdateNow = false;

            GuiChanger.Script.SetGuiLoading(GuiChanger.Script.CurrentGui + " MainMenu");
            await Task.Run(() => Task.Delay(1000));
            SceneManager.UnloadScene(SceneManager.GetActiveScene().name);
            MainCamera.transform.position = new Vector3(0, -25, 0);
            GuiChanger.Script.DoneLoading = true;
        }
    }

    public bool PlayerTurn()
    {
        return (Player == Turn && Turn > 0 && Id > 0);
    }

    public bool ComputerTurn()
    {
        if (Turn > 0)
        {
            return (Players[Turn - 1] == "X");
        }
        else
        {
            return false;
        }
    }

    public void WhoMovesNow()
    {
        if(PlayerTurn())
        {
            ShowToastMessage.Send("Your turn!", false);

            if(Settings.Get("Vibrations") == "true")
            {
                ShowToastMessage.Send(Settings.Get("Vibrations"), true);

                Handheld.Vibrate();
            }
        }
        else
        {
            if(Turn > 0)
            {
                ShowToastMessage.Send("Opponent's turn!", true);
            }
        }

        if (Turn < 1)
        {
            if (Turn < 0)
            {
                string Nickname = Players[(Turn * (-1)) - 1];
                if (Nickname.Length > 1)
                {
                    ShowToastMessage.Send(Nickname + " win!", false);
                }
                else
                {
                    ShowToastMessage.Send("You lose!", false);
                }
            }
            else
            {
                ShowToastMessage.Send("Draw!", false);
            }
        }


        /*if (Players.Length > 2)
        {
            MoveTexts[0].SetActive(false);
            MoveTexts[1].SetActive(false);
            MoveTexts[2].SetActive(true);
            MoveTexts[3].SetActive(true);
        }
        else
        {
            MoveTexts[0].SetActive(true);
            MoveTexts[1].SetActive(true);
            MoveTexts[2].SetActive(false);
            MoveTexts[3].SetActive(false);
        }
        StartCoroutine(Moving());*/
    }

    IEnumerator Moving()
    {
        OpponentMove.enabled = true;
        for (int i = 0; i <= 50; i++)
        {
            OpponentMove.GetComponent<CanvasGroup>().alpha = i * 0.02f;
            yield return new WaitForFixedUpdate();
        }

        while(Player != Turn && Turn <= 0)
        {
            for (int i = 50; i >= 0; i--)
            {
                OpponentMove.GetComponent<CanvasGroup>().alpha = i * 0.01f + 0.5f;
                yield return new WaitForFixedUpdate();
            }
            for (int i = 0; i <= 50; i++)
            {
                OpponentMove.GetComponent<CanvasGroup>().alpha = i * 0.01f + 0.5f;
                yield return new WaitForFixedUpdate();
            }
        }

        for (int i = 50; i >= 0; i--)
        {
            OpponentMove.GetComponent<CanvasGroup>().alpha = i * 0.02f;
            yield return new WaitForFixedUpdate();
        }
        OpponentMove.enabled = false;
    }
}
