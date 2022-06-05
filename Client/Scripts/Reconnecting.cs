using System.Collections;
using UnityEngine;

public class Reconnecting : MonoBehaviour
{
    public static Reconnecting Script;

    void Awake()
    {
        Script = GetComponent<Reconnecting>();
    }

    void Start()
    {
        StartCoroutine(CheckForConnection());
    }

    IEnumerator CheckForConnection()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (ClientServer.Script.Server.Connected)
            {
                GetComponent<CanvasGroup>().interactable = false;
                for (int i = 50; i >= 0; i--)
                {
                    GetComponent<CanvasGroup>().alpha = i * 0.02f;
                    yield return new WaitForFixedUpdate();
                }
                GetComponent<Canvas>().enabled = false;

                while (ClientServer.Script.Server.Connected)
                {
                    yield return new WaitForFixedUpdate();
                }
            }

            if (!ClientServer.Script.Server.Connected)
            {
                GetComponent<Canvas>().enabled = true;
                for (int i = 0; i <= 50; i++)
                {
                    GetComponent<CanvasGroup>().alpha = i * 0.02f;
                    yield return new WaitForFixedUpdate();
                }
                GetComponent<CanvasGroup>().interactable = true;

                while(!ClientServer.Script.Server.Connected)
                {
                    yield return new WaitForFixedUpdate();
                }
            }
        }
    }
}
