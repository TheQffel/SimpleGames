using UnityEngine;

public class FirstRun : MonoBehaviour
{
    public static FirstRun Script;

    void Awake()
    {
        Script = GetComponent<FirstRun>();
    }

    public string CurrentVersion;

    void Start()
    {
        string LastVersion = Settings.Get("LastVersion");
        if(LastVersion != CurrentVersion)
        {
            Settings.Set("LastVersion", CurrentVersion);

            if(LastVersion.Length < 1)
            {
                Alert.Script.CreateAlert("Information", "We recommend playing this game in portrait mode, but landscape mode is also available. You must login or register to save your progress. You can read 'How To Play?' instructions in pause menu after game started. Have fun!", false);
            }
        }

        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }
}
