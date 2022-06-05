using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Results : MonoBehaviour
{
    public static Results Script;

    void Awake()
    {
        Script = GetComponent<Results>();
    }

    public Text[] Players;
    public Text[] Points;
    public Text MoneyResult;
    public Text XpResult;

    public GameObject GameOver;
    public GameObject Table;
    public GameObject MoneyFrame;
    public GameObject XpFrame;
    public GameObject Button;

    int ButtonPosition = 0;

    public void ShowResults()
    {
        StartCoroutine(Show());
    }

    public void HideResults()
    {
        StartCoroutine(Hide());
    }

    public void SetResults(string[] Nicknames, string[] Results, int Money = -1, int Xp = -1)
    {
        for (int i = 0; i < 8; i++)
        {
            Players[i].text = "- - - - -";
            Points[i].text = "-";
        }

        for (int i = 0; i < Nicknames.Length; i++)
        {
            Players[i].text = Nicknames[i];
            Points[i].text = Results[i];
        }

        if(Money < 0 && Xp < 0)
        {
            GameOver.SetActive(false);
            MoneyFrame.SetActive(false);
            XpFrame.SetActive(false);

            ButtonPosition = -400;
        }
        else
        {
            GameOver.SetActive(true);
            MoneyFrame.SetActive(true);
            XpFrame.SetActive(true);

            ButtonPosition = -510;
        }
    }

    IEnumerator Show()
    {
        GetComponent<Canvas>().enabled = true;
        for (int i = 0; i <= 50; i++)
        {
            GetComponent<CanvasGroup>().alpha = (float)(i * 0.02);

            GameOver.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + 520, 0);
            Table.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + 60, 0);
            MoneyFrame.transform.localPosition = new Vector3(155, (50 - i) * (50 - i) * (-0.5f) - 400, 0);
            XpFrame.transform.localPosition = new Vector3(-155, (50 - i) * (50 - i) * (-0.5f) - 400, 0);
            Button.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + ButtonPosition, 0);

            yield return new WaitForFixedUpdate();
        }
        GetComponent<CanvasGroup>().interactable = true;
    }

    IEnumerator Hide()
    {
        GetComponent<CanvasGroup>().interactable = false;
        for (int i = 50; i >= 0; i--)
        {
            GetComponent<CanvasGroup>().alpha = (float)(i * 0.02);

            GameOver.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + 520, 0);
            Table.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + 60, 0);
            MoneyFrame.transform.localPosition = new Vector3(155, (50 - i) * (50 - i) * (-0.5f) - 400, 0);
            XpFrame.transform.localPosition = new Vector3(-155, (50 - i) * (50 - i) * (-0.5f) - 400, 0);
            Button.transform.localPosition = new Vector3(0, (50 - i) * (50 - i) * (-0.5f) + ButtonPosition, 0);

            yield return new WaitForFixedUpdate();
        }
        GetComponent<Canvas>().enabled = false;
    }
}
