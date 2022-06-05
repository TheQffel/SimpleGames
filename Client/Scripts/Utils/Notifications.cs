using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif

#if UNITY_IOS
using Unity.Notifications.iOS;
#endif

public class Notifications : MonoBehaviour
{
#if UNITY_ANDROID

    void Start()
    {
        AndroidNotificationChannel Channel = new AndroidNotificationChannel()
        {
            Id = "SimpleGames",
            Name = "Simple Games",
            Description = "Notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(Channel);
    }

    public void Send(string Title, string Content)
    {
        AndroidNotification Notification = new AndroidNotification
        {
            Title = Languages.Translate(Title, Settings.Get("Language")),
            Text = Languages.Translate(Content, Settings.Get("Language")),
        };

        AndroidNotificationCenter.SendNotification(Notification, "SimpleGames");
    }

#endif

#if UNITY_IOS

    public void Send(string Title, string Content)
    {
        iOSNotification Notification = new iOSNotification
        {
            Title = Languages.Translate(Title, Settings.Get("Language")),
            Body = Languages.Translate(Content, Settings.Get("Language")),
        };

        iOSNotificationCenter.ScheduleNotification(Notification);
    }

#endif
}

