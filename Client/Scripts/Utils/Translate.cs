using UnityEngine;
using UnityEngine.UI;

public class Translate : MonoBehaviour
{
    string EnglishText;

    public string OptionalPrefix = "";
    public string OptionalSuffix = "";

    void Start()
    {
        EnglishText = GetComponent<Text>().text;
        Languages.LanguageChange.AddListener(TranslateMe);
        TranslateMe();
    }

    void TranslateMe()
    {
        GetComponent<Text>().text = OptionalPrefix + Languages.Translate(EnglishText, Settings.Get("Language")) + OptionalSuffix;
    }
}
