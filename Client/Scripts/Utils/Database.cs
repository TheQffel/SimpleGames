using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.IO;

public class Database : MonoBehaviour
{
    static IDbConnection Connection;

    void Awake()
    {
        #if !UNITY_EDITOR

        UnityWebRequest DownloadFile = UnityWebRequest.Get(Application.streamingAssetsPath + "/Data.db");
        DownloadFile.SendWebRequest();
        while (!DownloadFile.isDone) { }
        byte[] FileParts = DownloadFile.downloadHandler.data;
        File.WriteAllBytes(Application.persistentDataPath + "/Data.db", FileParts);
        
        #endif 

        Connection = new SqliteConnection("URI=file:" + Application.persistentDataPath + "/Data.db");
        Connection.Open();
    }

    public static string[] Get(string Table, string Column = "*", string When = "", string Order = "")
    {
        IDbCommand Command = Connection.CreateCommand();
        Command.CommandText = "SELECT " + Column + " FROM " + Table;

        if (When != "")
        {
            Command.CommandText += " WHERE " + When;
        }
        if (Order != "")
        {
            Command.CommandText += " ORDER BY " + Order;
        }

        List<string> Result = new List<string>();
        IDataReader Reader = Command.ExecuteReader();

        while (Reader.Read())
        {
            for (int i = 0; i < Reader.FieldCount; i++)
            {
                Result.Add(Reader[i].ToString());
            }
        }
        Reader.Close();

        if (Result.Count == 0) { Result.Add(""); }

        return Result.ToArray();
    }

    void OnApplicationQuit()
    {
        Connection.Close();
    }

    // Data Base is Read Only.

    /*
    public static int Set(string Table, string ColumnValues, string When)
    {
        IDbCommand Command = Connection.CreateCommand();
        Command.CommandText = "UPDATE " + Table + " SET " + ColumnValues + " WHERE " + When;
        return Command.ExecuteNonQuery();
    }

    public static int Add(string Table, string Columns, string Values)
    {
        IDbCommand Command = Connection.CreateCommand();
        Command.CommandText = "INSERT INTO " + Table + " (" + Columns + ") VALUES (" + Values + ")";
        return Command.ExecuteNonQuery();
    }

    public static int Del(string Table, string Where, string Value)
    {
        IDbCommand Command = Connection.CreateCommand();
        Command.CommandText = "DELETE FROM " + Table + " WHERE " + Where + " = '" + Value + "'";
        return Command.ExecuteNonQuery();
    }
    */
}
