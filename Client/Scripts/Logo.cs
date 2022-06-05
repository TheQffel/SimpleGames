using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Logo : MonoBehaviour
{
    public static Logo Script;

    void Awake()
    {
        Script = GetComponent<Logo>();
    }

    public Text Text;
    public Image Image;

    void Start()
    {
        StartCoroutine(Stop());
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        for (int i = 0; i < 50; i++)
        {
            Image.color = new Color(1, 1, 1, (i + 1) * 0.02f);
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator Stop()
    {
        Text.text += "L";
        yield return new WaitForSeconds(0.25f);
        Text.text += "o";
        yield return new WaitForSeconds(0.25f);
        Text.text += "a";
        yield return new WaitForSeconds(0.25f);
        Text.text += "d";
        yield return new WaitForSeconds(0.25f);
        Text.text += "i";
        yield return new WaitForSeconds(0.25f);
        Text.text += "n";
        yield return new WaitForSeconds(0.25f);
        Text.text += "g";
        yield return new WaitForSeconds(0.25f);
        Text.text += ".";
        yield return new WaitForSeconds(0.25f);
        for (int i = 50; i > 0; i--)
        {
            if(i % 25 == 0)
            {
                Text.text += ".";
            }
            GetComponent<CanvasGroup>().alpha = i * 0.02f;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
