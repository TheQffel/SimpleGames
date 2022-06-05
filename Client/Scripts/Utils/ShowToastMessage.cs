using UnityEngine;

public class ShowToastMessage : MonoBehaviour
{
    static string ToastText;
    static bool ToastLength;

    static AndroidJavaObject CurrentActivity;

    public static void Send(string Message, bool Length)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            CurrentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");

            ToastText = Message;
            ToastLength = Length;

            CurrentActivity.Call("runOnUiThread", new AndroidJavaRunnable(ShowToast));
        }
    }

    static void ShowToast()
    {
        AndroidJavaObject Context = CurrentActivity.Call<AndroidJavaObject>("getApplicationContext");
        AndroidJavaClass Toast = new AndroidJavaClass("android.widget.Toast");
        AndroidJavaObject String = new AndroidJavaObject("java.lang.String", ToastText);
        
        if (ToastLength)
        {
            AndroidJavaObject ToastMessage = Toast.CallStatic<AndroidJavaObject>("makeText", Context, String, Toast.GetStatic<int>("LENGTH_LONG"));
            ToastMessage.Call("show");
        }
        else
        {
            AndroidJavaObject ToastMessage = Toast.CallStatic<AndroidJavaObject>("makeText", Context, String, Toast.GetStatic<int>("LENGTH_SHORT"));
            ToastMessage.Call("show");
        }
    }
}