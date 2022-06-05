using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Fullscreen : MonoBehaviour
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

            #if UNITY_ANDROID && !UNITY_EDITOR

            Screen.fullScreen = false;
            AndroidJavaObject Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass LayoutParamsClass = new AndroidJavaClass("android.view.WindowManager$LayoutParams");
                AndroidJavaObject WindowObject = Activity.Call<AndroidJavaObject>("getWindow");
                WindowObject.Call("clearFlags", LayoutParamsClass.GetStatic<int>("FLAG_FORCE_NOT_FULLSCREEN"));
                WindowObject.Call("clearFlags", LayoutParamsClass.GetStatic<int>("FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS"));
                WindowObject.Call("addFlags", LayoutParamsClass.GetStatic<int>("FLAG_FULLSCREEN"));
            }));

            #endif
        }
        else
        {
            StartCoroutine(SwitchOff());

            #if UNITY_ANDROID && !UNITY_EDITOR

            Screen.fullScreen = false;
            AndroidJavaObject Activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            Activity.Call("runOnUiThread", new AndroidJavaRunnable(() =>
            {
                AndroidJavaClass LayoutParamsClass = new AndroidJavaClass("android.view.WindowManager$LayoutParams");
                AndroidJavaObject WindowObject = Activity.Call<AndroidJavaObject>("getWindow");
                WindowObject.Call("clearFlags", LayoutParamsClass.GetStatic<int>("FLAG_FULLSCREEN"));
                WindowObject.Call("addFlags", LayoutParamsClass.GetStatic<int>("FLAG_FORCE_NOT_FULLSCREEN"));
                WindowObject.Call("addFlags", LayoutParamsClass.GetStatic<int>("FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS"));
            }));

            #endif
        }
    }

    public bool Get()
    {
        string Value = Settings.Get("Fullscreen");
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
        Settings.Set("Fullscreen", Value.ToString());
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
