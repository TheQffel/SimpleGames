using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Language : MonoBehaviour
{
    public GameObject LanguagesList;
    public GameObject LanguagesListArrow;
    public GameObject SelectedLanguage;
    public GameObject SelectedLanguageFlag;
    public Texture[] Flags;

    bool Expanded = false;
    bool IsMoving = false;

    void Start()
    {
        RefreshLanguageButton();
    }

    void RefreshLanguageButton()
    {
        SelectedLanguage.GetComponent<Text>().text = Languages.Translate("English", Get());
        SelectedLanguageFlag.GetComponent<RawImage>().texture = LanguageToFlagImage(Get());
        Languages.LanguageChange.Invoke();
    }

    public string Get()
    {
        string CurrentLanguage = Settings.Get("Language");
        if(CurrentLanguage == null)
        {
            CurrentLanguage = "English";
            Set(CurrentLanguage);
        }
        return CurrentLanguage;
    }

    public void Set(string Value)
    {
        Settings.Set("Language", Value);
        RefreshLanguageButton();
    }

    public void Change(string Language)
    {
        if (!IsMoving)
        {
            Set(Language);
            if (Language != "Polish" && Language != "English")
            {
                Alert.Script.CreateAlert("Warning", "Currently there are only 2 supported languages: English and Polsih. If you know any other language you can help us translating the app. Contact me on discord: TheQffel#8408.", false);
            }
        }
    }

    public Texture LanguageToFlagImage(string Language)
    {
        if (Language == "Chinese") { return Flags[0]; }
        if (Language == "English") { return Flags[1]; }
        if (Language == "French") { return Flags[2]; }
        if (Language == "German") { return Flags[3]; }
        if (Language == "Greek") { return Flags[4]; }
        if (Language == "Italian") { return Flags[5]; }
        if (Language == "Japanese") { return Flags[6]; }
        if (Language == "Korean") { return Flags[7]; }
        if (Language == "Polish") { return Flags[8]; }
        if (Language == "Portuguese") { return Flags[9]; }
        if (Language == "Russian") { return Flags[10]; }
        if (Language == "Spanish") { return Flags[11]; }
        return null;
    }

    public void ExpandOrCollapse()
    {
        if (!IsMoving)
        {
            if (!Expanded)
            {
                StartCoroutine(ExpandList());
                Expanded = true;
            }
            else 
            {
                StartCoroutine(CollapseList());
                Expanded = false;
            }
        }
    }

    IEnumerator ExpandList()
    {
        IsMoving = true;
        LanguagesList.SetActive(true);
        for (int i = 0; i <= 40; i++)
        {
            LanguagesList.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, (i * -8) - 30, 0);
            LanguagesList.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(450, i * 16);

            LanguagesListArrow.transform.localEulerAngles = new Vector3(0, 0, i * -4.5f);

            yield return new WaitForFixedUpdate();
        }
        IsMoving = false;
    }

    IEnumerator CollapseList()
    {
        IsMoving = true;
        for (int i = 40; i >= 0; i--)
        {
            LanguagesList.transform.GetComponent<RectTransform>().localPosition = new Vector3(0, (i * -8) - 30, 0);
            LanguagesList.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(450, i * 16);

            LanguagesListArrow.transform.localEulerAngles = new Vector3(0, 0, i * -4.5f);

            yield return new WaitForFixedUpdate();
        }
        LanguagesList.SetActive(false);
        IsMoving = false;
    }
}
