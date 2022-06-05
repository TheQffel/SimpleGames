using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class ClientServer : MonoBehaviour
{
    public static ClientServer Script;

    //string Domain = "";
    string IpAddress = "192.168.1.8";
    int Port = 12347;

    void Awake()
    {
        Script = GetComponent<ClientServer>();

        //IpAddress = Dns.GetHostAddresses(Domain)[0].ToString();

        Listen();
    }

    public TcpClient Server;
    string[] Splitter = { " | [Tq] | " };

    async void Listen()
    {
        while (true)
        {
            Server = new TcpClient();
            try { await Server.ConnectAsync(IPAddress.Parse(IpAddress), Port); } catch (Exception) { }
            if (Server.Connected)
            {
                Console.Log("Connected to server.");

                Server.ReceiveBufferSize = 25600;
                Server.SendBufferSize = 25600;

                byte[] Buffer = new byte[25600];
                Stream Stream = Server.GetStream();

                Task<int> Task;
                while (await (Task = Stream.ReadAsync(Buffer, 0, Buffer.Length)) > 0)
                {
                    Receive(Encryption.Decrypt(Buffer.Take(Task.Result).ToArray()));
                }
                Server.Close();

                Console.Log("Disconnected from server.");
            }
        }
    }

    public void Send(string[] Messages)
    {
        if (Server.Connected)
        {
            if (Account.Script.LoggedIn || Messages[0] == "Account")
            {
                string Message = "";
                for (int i = 0; i < Messages.Length; i++)
                {
                    Message += Messages[i] + Splitter[0];
                }
                Stream Stream = Server.GetStream();
                byte[] Buffer = Encryption.Encrypt(Message + Messages.Length);
                Stream.Write(Buffer, 0, Buffer.Length);
                Console.Log("Message send to server: " + Message.Replace(Splitter[0], "|") + Messages.Length + ".");
            }
            else
            {
                Alert.Script.CreateAlert("Error", "You must be logged in to do this!", false);
                GuiChanger.Script.CancelLoading = true;
            }
        }
    }

    public void Receive(string Message)
    {
        Console.Log("Message recieved from server: " + Message.Replace(Splitter[0], "|") + ".");
        string[] Messages = Message.Split(Splitter, StringSplitOptions.None);
        Array.Resize(ref Messages, Messages.Length - 1);
        switch (Messages[0])
        {
            case ("Account"):
            {
                Account.Script.NetworkAction(Messages);
                break;
            }
            case ("Games"):
            {
                Games.Script.NetworkAction(Messages);
                break;
            }
            case ("Friends"):
            {
                Friends.Script.NetworkAction(Messages);
                break;
            }
        }
    }

    void OnApplicationQuit()
    {
        Server.Close();
    }
}
