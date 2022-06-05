using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Inputbox : MonoBehaviour
{
    public GameObject Title;
    public GameObject Input;
    public GameObject Done;
    public GameObject Cancel;
    public GameObject Background;

    static GameObject ThisGameObject;

    void Awake()
    {
        ThisGameObject = gameObject;
    }

    public static bool Answered = false;
    public static string Answer;

    public void CreateInput(string Name)
    {
        Answered = false;
        Title.GetComponent<Text>().text = Languages.Translate(Name, Settings.Get("Language")) + ":";
        StartCoroutine(Appear());
    }

    public static void AskForInput(string Name)
    {
        ThisGameObject.GetComponent<Inputbox>().CreateInput(Languages.Translate(Name, Settings.Get("Language")));
    }

    public void AnswerDone()
    {
        Answered = true;
        Answer = Input.GetComponent<InputField>().text;
        Input.GetComponent<InputField>().text = "";
        StartCoroutine(Disappear());
    }

    public void AnswerCancel()
    {
        Answered = true;
        Answer = null;
        Input.GetComponent<InputField>().text = "";
        StartCoroutine(Disappear());
    }

    public static void WaitForAnswer()
    {
        while(!Answered)
        {
            Thread.Sleep(10);
        }
    }

    IEnumerator Disappear()
    {
        GetComponent<CanvasGroup>().interactable = false;
        for (int i = 50; i >= 0; i--)
        {
            GetComponent<CanvasGroup>().alpha = (float)(i * 0.02);
            Background.transform.localPosition = new Vector3((50 - i) * (50 - i) * (0.5f), 0, 0);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<Canvas>().enabled = false;
    }

    IEnumerator Appear()
    {
        GetComponent<Canvas>().enabled = true;
        for (int i = 0; i <= 50; i++)
        {
            GetComponent<CanvasGroup>().alpha = (float)(i * 0.02);
            Background.transform.localPosition = new Vector3((50 - i) * (50 - i) * (-0.5f), 0, 0);
            yield return new WaitForFixedUpdate();
        }
        GetComponent<CanvasGroup>().interactable = true;
    }
}
