using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;

namespace SgServer
{
    class Database
    {
        static object Blockade = new object();

        public static string MySqlServer = "127.0.0.1";
        public static string MySqlDatabase = "database";
        public static string MySqlUsername = "username";
        public static string MySqlPassword = "password";
        public static string MySqlPrefix = "sg_";

        public static MySqlConnection[] Connections = new MySqlConnection[1];
        public static string Connection = "Server = " + MySqlServer + "; Database = " + MySqlDatabase + "; User = " + MySqlUsername + "; Password = " + MySqlPassword + "; SslMode = None; ConvertZeroDatetime = True; CharSet = utf8;";

        public static void OpenConnections()
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                Connections[i] = new MySqlConnection(Connection);
                Connections[i].Open();
                
                MySqlCommand Command = Connections[i].CreateCommand();
                Command.CommandText = "SET @@sql_mode = '';";
                Command.ExecuteNonQuery();
            }
        }

        public static void CloseConnections()
        {
            for (int i = 0; i < Connections.Length; i++)
            {
                Connections[i].Close();
            }
        }

        static int Counter = 0;

        
        public static string[][] Get(string Table, string Column = "*", string When = "", string Order = "")
        {
            lock (Blockade)
            {
                MySqlCommand Command = Connections[Counter].CreateCommand();
                Command.CommandText = "SELECT " + Column + " FROM " + Table;

                if (When != "")
                {
                    Command.CommandText += " WHERE " + When;
                }
                if (Order != "")
                {
                    Command.CommandText += " ORDER BY " + Order;
                }

                DataTable Result = new DataTable();
                Result.Load(Command.ExecuteReader());

                // Row - How much "objects"
                // Column - How much "values" in single "object"

                if (Result.Rows.Count > 0)
                {
                    object[][] Objects = Result.AsEnumerable().Select(x => x.ItemArray).ToArray();
                    string[][] Texts = new string[Objects.Length][];
                    for (int i = 0; i < Texts.Length; i++)
                    {
                        Texts[i] = new string[Objects[i].Length];
                        for (int j = 0; j < Objects[i].Length; j++)
                        {
                            if (Objects[i][j] is DateTime)
                            {
                                Texts[i][j] = ((DateTime)Objects[i][j]).ToString("yyyy-MM-dd HH:mm:ss");
                            }
                            else
                            {
                                Texts[i][j] = Objects[i][j].ToString();
                            }
                        }
                    }
                    return Texts;
                }
                return new[] { new[] { "" } };
            }
        }

        public static void Set(string Table, string ColumnValues, string When)
        {
            lock (Blockade)
            {
                MySqlCommand Command = Connections[Counter].CreateCommand();
                Command.CommandText = "UPDATE " + Table + " SET " + ColumnValues + " WHERE " + When;
                Command.ExecuteNonQuery();
            }
        }

        public static void Add(string Table, string Columns, string Values)
        {
            lock (Blockade)
            {
                MySqlCommand Command = Connections[Counter].CreateCommand();
                Command.CommandText = "INSERT INTO " + Table + " (" + Columns + ") VALUES (" + Values + ")";
                Command.ExecuteNonQuery();
            }
        }

        public static void Del(string Table, string Where)
        {
            lock (Blockade)
            {
                MySqlCommand Command = Connections[Counter].CreateCommand();
                Command.CommandText = "DELETE FROM " + Table + " WHERE " + Where;
                Command.ExecuteNonQuery();
            }
        }
    }
}