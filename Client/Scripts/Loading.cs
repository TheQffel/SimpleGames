using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public static Loading Script;

    void Awake()
    {
        Script = GetComponent<Loading>();
    }

    public RectTransform Arrows;
    public Text Hint;

    int Countdown = 0;
    string[] Tips =
    {
        "You can read tips during the loading screens.",
        "You can invite friends by clicking special button in friend list.",
        //"You will receive a reward for each invited friend when he reaches 10 level.",
        //"Join our official discord server: discord.gg/vy9uNxn or click discord icon in settings menu.",
        //"You like this app? Rate it on Google Play!",
        "We recommend playing this game in portait mode, but landscape mode is also available!",
        "Something not displaying or displaying wrong? Try switch to portait mode.",
        "Notice that this game is still in developement.",
        //"Help us translating this game! Join our discord to learn more.",
        "Did you know that this game is created by only one person?",
        //"Not enough people to play? Join our discord and invite someone to play!",
    };

    void FixedUpdate()
    {
        Arrows.localEulerAngles -= new Vector3(0, 0, 2.5f);
        Countdown++;

        if(Countdown > Hint.text.Length * 15)
        {
            Countdown = 0;
            Hint.text = Languages.Translate(Tips[Random.Range(0, Tips.Length)], Settings.Get("Language"));
        }
    }
}
