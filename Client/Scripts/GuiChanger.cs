using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class GuiChanger : MonoBehaviour
{
    public static GuiChanger Script;

    void Awake()
    {
        Script = GetComponent<GuiChanger>();
    }

    public Canvas GuiBackground;
    public Canvas GuiLoading;
    public Canvas GuiMainMenu;
    public Canvas GuiLobby;
    public Canvas GuiGames;
    public Canvas GuiMode;
    public Canvas GuiDifficulty;
    public Canvas GuiGame;
    public Canvas GuiAccount;
    public Canvas GuiRegister;
    public Canvas GuiLogin;
    public Canvas GuiFriends;
    public Canvas GuiShop;
    public Canvas GuiAchievements;
    public Canvas GuiSettings;
    public Canvas GuiProfile;

    public string CurrentGui = "X";
    public bool GuiChanging = false;
    public bool DoneLoading = false;
    public bool CancelLoading = false;

    public void SetGui(string DisAppearGui)
    {
        string[] Guis = DisAppearGui.Split(' ');
        StartCoroutine(DisAppear(GuiNameToCanvas(Guis[0]), GuiNameToCanvas(Guis[1])));
        
        if (Guis[1].Length > 1)
        {
            CurrentGui = Guis[1];
        }
    }

    public void SetGuiLoading(string DisAppearGui)
    {
        string[] Guis = DisAppearGui.Split(' ');
        StartCoroutine(DisAppearLoading(GuiNameToCanvas(Guis[0]), GuiNameToCanvas(Guis[1])));

        if (Guis[1].Length > 1)
        {
            CurrentGui = Guis[1];
        }
    }

    Canvas GuiNameToCanvas(string Name)
    {
        switch (Name)
        {
            case ("Background"):
            {
                return GuiBackground;
            }
            case ("MainMenu"):
            {
                return GuiMainMenu;
            }
            case ("Lobby"):
            {
                return GuiLobby;
            }
            case ("Games"):
            {
                return GuiGames;
            }
            case ("Mode"):
            {
                return GuiMode;
            }
            case ("Difficulty"):
            {
                return GuiDifficulty;
            }
            case ("Game"):
            {
                return GuiGame;
            }
            case ("Account"):
            {
                return GuiAccount;
            }
            case ("Register"):
            {
                return GuiRegister;
            }
            case ("Login"):
            {
                return GuiLogin;
            }
            case ("Friends"):
            {
                return GuiFriends;
            }
            case ("Shop"):
            {
                return GuiShop;
            }
            case ("Achievements"):
            {
                return GuiAchievements;
            }
            case ("Settings"):
            {
                return GuiSettings;
            }
        }
        return null;
    }

    IEnumerator DisAppear(Canvas DisappearGui, Canvas AppearGui)
    {
        GuiChanging = true;

        if (DisappearGui != null)
        {
            DisappearGui.GetComponent<CanvasGroup>().interactable = false;
            for (int i = 50; i >= 0; i--)
            {
                DisappearGui.GetComponent<CanvasGroup>().alpha = i * 0.02f;
                yield return new WaitForFixedUpdate();
            }
            DisappearGui.enabled = false;
        }

        if (AppearGui != null)
        {
            AppearGui.enabled = true;
            for (int i = 0; i <= 50; i++)
            {
                AppearGui.GetComponent<CanvasGroup>().alpha = i * 0.02f;
                yield return new WaitForFixedUpdate();
            }
            AppearGui.GetComponent<CanvasGroup>().interactable = true;
        }

        GuiChanging = false;
    }

    IEnumerator DisAppearLoading(Canvas DisappearGui, Canvas AppearGui)
    {
        GuiChanging = true;
        DoneLoading = false;
        CancelLoading = false;

        GuiLoading.enabled = true;
        for (int i = 0; i <= 50; i++)
        {
            GuiLoading.GetComponent<CanvasGroup>().alpha = i * 0.02f;
            yield return new WaitForFixedUpdate();
        }
        GuiLoading.GetComponent<CanvasGroup>().interactable = true;

        if (DisappearGui != null)
        {
            DisappearGui.GetComponent<CanvasGroup>().interactable = false;
            DisappearGui.GetComponent<CanvasGroup>().alpha = 0;
            DisappearGui.enabled = false;
        }

        while (!DoneLoading && !CancelLoading)
        {
            yield return new WaitForFixedUpdate();
        }
        if(CancelLoading)
        {
            AppearGui = DisappearGui;
        }

        if (AppearGui != null)
        {
            AppearGui.enabled = true;
            AppearGui.GetComponent<CanvasGroup>().alpha = 1;
            AppearGui.GetComponent<CanvasGroup>().interactable = true;
        }

        GuiLoading.GetComponent<CanvasGroup>().interactable = false;
        for (int i = 50; i >= 0; i--)
        {
            GuiLoading.GetComponent<CanvasGroup>().alpha = i * 0.02f;
            yield return new WaitForFixedUpdate();
        }
        GuiLoading.enabled = false;

        GuiChanging = false;
        DoneLoading = false;
        CancelLoading = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!GuiChanging)
            {
                switch (CurrentGui)
                {
                    case ("MainMenu"):
                    {
                        ExitGame();
                        break;
                    }
                    case ("Lobby"):
                    {
                        Lobby.Script.Exit();
                        break;
                    }
                    case ("Game"):
                    {
                        Game.Script.StopGame();
                        break;
                    }
                    case ("Difficulty"):
                    {
                        SetGui(CurrentGui + " X");
                        break;
                    }
                    case ("Mode"):
                    {
                        SetGui(CurrentGui + " X");
                        break;
                    }
                    case ("Register"):
                    {
                        SetGui(CurrentGui + " Account");
                        break;
                    }
                    case ("Login"):
                    {
                        SetGui(CurrentGui + " Account");
                        break;
                    }
                    case ("Friends"):
                    {
                        SetGui(CurrentGui + " Account");
                        break;
                    }
                    case ("Achievements"):
                    {
                        SetGui(CurrentGui + " Account");
                        break;
                    }
                    default:
                    {
                        SetGui(CurrentGui + " MainMenu");
                        break;
                    }
                }
            }
        }
    }

    public async void ExitGame()
    {
        Alert.Script.CreateAlert("Question", "Exit Game?", true);
        await Task.Run(() => Alert.Script.WaitForAnswer());

        if (Alert.Script.Answer)
        {
            Application.Quit();
        }
    }
}
