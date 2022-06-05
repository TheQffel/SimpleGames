using UnityEngine;
using UnityEngine.Events;

public class Languages : MonoBehaviour
{
    public static UnityEvent LanguageChange = new UnityEvent();

    public static string Translate(string EnglishText, string LocaleName)
    {
        string TranslatedText = Database.Get("Languages", LocaleName, "English = '" + EnglishText + "'")[0];

        if(TranslatedText != "")
        {
            return TranslatedText;
        }
        else
        {
            return EnglishText;
        }
    }
}
