using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class Alert : MonoBehaviour
{
    public static Alert Script;

    void Awake()
    {
        Script = GetComponent<Alert>();
    }

    public GameObject Title;
    public GameObject Text;
    public GameObject Ok;
    public GameObject Yes;
    public GameObject No;
    public GameObject Background;

    public bool Answered = false;
    public bool Answer;

    public void CreateAlert(string Name, string Content, bool CanChoose)
    {
        Answered = false;
        if(CanChoose)
        {
            Ok.SetActive(false);
            Yes.SetActive(true);
            No.SetActive(true);
        }
        else
        {
            Ok.SetActive(true);
            Yes.SetActive(false);
            No.SetActive(false);
        }
        Title.GetComponent<Text>().text = Languages.Translate(Name, Settings.Get("Language")) + ":";
        Text.GetComponent<Text>().text = Languages.Translate(Content, Settings.Get("Language"));
        StartCoroutine(Appear());
    }

    public void AnswerOk()
    {
        Answered = true;
        StartCoroutine(Disappear());
    }

    public void AnswerYes()
    {
        Answered = true;
        Answer = true;
        StartCoroutine(Disappear());
    }

    public void AnswerNo()
    {
        Answered = true;
        Answer = false;
        StartCoroutine(Disappear());
    }

    public void WaitForAnswer()
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
