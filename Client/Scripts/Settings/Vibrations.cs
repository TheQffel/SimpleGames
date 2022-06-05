using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Vibrations : MonoBehaviour
{
    public GameObject SliderBackground;
    public GameObject SliderDot;

    bool IsMoving = false;

    void Start()
    {
        RefreshButtonPosition();
    }

    void RefreshButtonPosition()
    {
        if(Get())
        {
            StartCoroutine(SwitchOn());
        }
        else
        {
            StartCoroutine(SwitchOff());
        }
    }

    public bool Get()
    {
        string Value = Settings.Get("Vibrations");
        if (Value != null)
        {
            if (Value.ToLower() == "false")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        return true;
    }

    public void Set(bool Value)
    {
        Settings.Set("Vibrations", Value.ToString());
        RefreshButtonPosition();
    }

    public void Change()
    {
        if (!IsMoving)
        {
            Set(!Get());
        }
    }

    IEnumerator SwitchOn()
    {
        IsMoving = true;
        for (int i = -35; i <= 35; i++)
        {
            SliderBackground.GetComponent<Image>().color = new Color(i * -0.03f, i * 0.03f, 0);
            SliderDot.transform.localPosition = new Vector3(i, 0, 0);

            yield return new WaitForFixedUpdate();
        }
        IsMoving = false;
    }

    IEnumerator SwitchOff()
    {
        IsMoving = true;
        for (int i = 35; i >= -35; i--)
        {
            SliderBackground.GetComponent<Image>().color = new Color(i * -0.03f, i * 0.03f, 0);
            SliderDot.transform.localPosition = new Vector3(i, 0, 0);

            yield return new WaitForFixedUpdate();
        }
        IsMoving = false;
    }
}
