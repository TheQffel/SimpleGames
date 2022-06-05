using System;
using System.IO;
using UnityEngine;

public class Console : MonoBehaviour
{
    void Awake()
    {
        if(!Directory.Exists(Application.persistentDataPath + "/Logs/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Logs/");
        }
        //Application.logMessageReceived += Handle;
    }

    public static void Log(string String, bool DisplayTime = true)
    {
        DateTime Time = DateTime.Now;
        Debug.Log(String);

        if (DisplayTime)
        {
            String = "[" + Time.Hour.ToString("D2") + ":" + Time.Minute.ToString("D2") + ":" + Time.Second.ToString("D2") + "]: " + String;
        }
        using (StreamWriter File = new StreamWriter(Application.persistentDataPath + "/Logs/" + Time.Day.ToString("D2") + "-" + Time.Month.ToString("D2") + "-" + Time.Year.ToString("D2") + ".log", true))
        {
            File.WriteLine(String);
        }
    }

    void Handle(string String, string StackTrace, LogType Type)
    {
        
    }
}
