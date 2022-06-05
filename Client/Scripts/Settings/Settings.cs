using UnityEngine;

public class Settings : MonoBehaviour
{
    public static string Get(string Name)
    {
        if(PlayerPrefs.HasKey(Name))
        {
            return PlayerPrefs.GetString(Name);
        }
        else
        {
            return null;
        }
    }

    public static void Set(string Name, string Value)
    {
        PlayerPrefs.SetString(Name, Value);
        PlayerPrefs.Save();

        Console.Log("Setting " + Name + " to " + Value);
    }

    public static void Del(string Name)
    {
        PlayerPrefs.DeleteKey(Name);
        PlayerPrefs.Save();
    }
}
