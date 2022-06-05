using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : MonoBehaviour
{
    public static Lobby Script;

    void Awake()
    {
        Script = GetComponent<Lobby>();
    }

    public RectTransform Hourglass;

    public Text Online;
    public Text InGame;
    public Text AllLobbies;
    public Text InLobby;

    public string CurrentGame = "X";

    void Start()
    {
        StartCoroutine(Rotate());
    }

    public async void Exit()
    {
        Alert.Script.CreateAlert("Question", "Leave Lobby?", true);
        await Task.Run(() => Alert.Script.WaitForAnswer());

        if (Alert.Script.Answer)
        {
            GuiChanger.Script.SetGuiLoading("Lobby MainMenu");
            ClientServer.Script.Send(new string[] { "Games", "Lobby", "Leave" } );
            await Task.Run(() => Task.Delay(1000));
            GuiChanger.Script.DoneLoading = true;
        }
    }

    IEnumerator Rotate()
    {
        while (true)
        {
            for (int i = 0; i < 72; i++)
            {
                Hourglass.localEulerAngles -= new Vector3(0, 0, 2.5f);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(2.5f);
        }
    }
}
