using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Autorotate : MonoBehaviour
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

            Alert.Script.CreateAlert("Warning", "Auto rotate allows for landscape game mode. This function is experimental and some bugs may appear. In case of problems with the game, turn off auto rotate.", false);

            Screen.orientation = ScreenOrientation.AutoRotation;
        }
        else
        {
            StartCoroutine(SwitchOff());

            Screen.orientation = ScreenOrientation.Portrait;
        }
    }

    public bool Get()
    {
        string Value = Settings.Get("Autorotate");
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
        return false;
    }

    public void Set(bool Value)
    {
        Settings.Set("Autorotate", Value.ToString());
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
