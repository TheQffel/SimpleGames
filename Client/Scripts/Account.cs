using System;
using System.Collections;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Account : MonoBehaviour
{
    public static Account Script;

    void Awake()
    {
        Script = GetComponent<Account>();
    }

    public InputField LoginNicknameInput;
    public InputField LoginPasswordInput;
    public Toggle LoginRememberInput;

    public InputField RegisterNicknameInput;
    public InputField RegisterPasswordInput;
    public InputField RegisterRepeatInput;
    public InputField RegisterEmailInput;
    public Toggle RegisterRememberInput;

    public Image AccountAvatar;
    public Text AccountNickname;
    public Text AccountPassword;
    public Text AccountEmail;
    public Text AccountCoins;
    public Text AccountXp;
    public Text AccountLevel;

    public GameObject AccountLogout;
    public GameObject AccountLoginRegister;

    public bool LoggedIn = false;
    public string Nickname = "";
    bool ButtonState = false;
    bool WaitingforConnection = true;

    bool NewAccountInformation = false;
    string[] AccountInformation = new string[5];

    public Sprite LoggedOutTexture;

    void Start()
    {
        AccountLogout.SetActive(false);
        StartCoroutine(LoginAtConnect());
    }

    IEnumerator LoginAtConnect()
    {
        string Nickname = Settings.Get("Nickname");
        string Password = Settings.Get("Password");
        if (Nickname != null && Password != null)
        {
            LoginNicknameInput.text = Nickname;
            LoginPasswordInput.text = Password;
            LoginRememberInput.isOn = true;
            while (!ClientServer.Script.Server.Connected)
            {
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(1);
            Login();
        }
        WaitingforConnection = false;
    }

    void Update()
    {
        if(!ClientServer.Script.Server.Connected)
        {
            if (!WaitingforConnection)
            {
                WaitingforConnection = true;
                StartCoroutine(LoginAtConnect());
            }
        }

        if(NewAccountInformation)
        {
            NewAccountInformation = false;

            AccountNickname.text = AccountInformation[0];
            AccountPassword.text = AccountInformation[1];
            AccountEmail.text = AccountInformation[2];
            AccountCoins.text = AccountInformation[3] + " $";
            AccountXp.text = AccountInformation[4] + " XP";
            AccountLevel.text = XpToLevel(int.Parse(AccountInformation[4])) + " LVL";

            SetAvatar(AccountNickname.text);
            if(AccountEmail.text.Length < 1)
            {
                AccountEmail.text = "--------";
            }
        }

        if(LoggedIn != ButtonState)
        {
            if (LoggedIn)
            {
                AccountLogout.SetActive(true);
                AccountLoginRegister.SetActive(false);
            }
            else
            {
                AccountLogout.SetActive(false);
                AccountLoginRegister.SetActive(true);
            }
            ButtonState = LoggedIn;
        }
    }

    public void Register()
    {
        if(IsNicknameValid(RegisterNicknameInput.text))
        {
            if (IsPasswordValid(RegisterPasswordInput.text))
            {
                if (IsEmailValid(RegisterEmailInput.text) || RegisterEmailInput.text == "")
                {
                    if (RegisterPasswordInput.text == RegisterRepeatInput.text)
                    {
                        ClientServer.Script.Send(new string[] { "Account", "Register", RegisterNicknameInput.text, RegisterPasswordInput.text, RegisterEmailInput.text });

                        if (RegisterRememberInput.isOn)
                        {
                            Settings.Set("Nickname", RegisterNicknameInput.text);
                            Settings.Set("Password", RegisterPasswordInput.text);
                        }
                    }
                    else
                    {
                        Alert.Script.CreateAlert("Error", "Your passwords are incorrect. They not match.", false);
                    }
                }
                else
                {
                    Alert.Script.CreateAlert("Error", "Your email is invalid. Your email should contain at and dot and it must be between 6 and 96 characters long.", false);
                }
            }
            else
            {
                Alert.Script.CreateAlert("Error", "Your password is invalid. Your password should contain big letter, small letter and number and it must be between 5 and 48 characters long.", false);
            }
        }
        else
        {
            Alert.Script.CreateAlert("Error", "Your nickname is invalid. Your nickname should only be letters and numbers and it must be between 4 and 24 characters long.", false);
        }
    }

    public void Login()
    {
        if(IsNicknameValid(LoginNicknameInput.text))
        {
            if (IsPasswordValid(LoginPasswordInput.text))
            {
                ClientServer.Script.Send(new string[] { "Account", "Login", LoginNicknameInput.text, LoginPasswordInput.text });

                if (LoginRememberInput.isOn)
                {
                    Settings.Set("Nickname", LoginNicknameInput.text);
                    Settings.Set("Password", LoginPasswordInput.text);
                }
            }
            else
            {
                Alert.Script.CreateAlert("Error", "Your password is invalid. Your password should contain big letter, small letter and number and it must be between 5 and 48 characters long.", false);
            }
        }
        else
        {
            Alert.Script.CreateAlert("Error", "Your nickname is invalid. Your nickname should only be letters and numbers and it must be between 4 and 24 characters long.", false);
        }
    }

    public void Logout()
    {
        LoggedIn = false;
        ClientServer.Script.Send(new string[] { "Account", "Logout" });
        LoginNicknameInput.text = "";
        LoginPasswordInput.text = "";
        Settings.Del("Nickname");
        Settings.Del("Password");
        Nickname = "";

        AccountNickname.text = "- - - - -";
        AccountPassword.text = "- - - - -";
        AccountEmail.text = "- - - - -";
        AccountCoins.text = "0 $";
        AccountXp.text = "0 XP";
        AccountLevel.text = "0 LVL";
        AccountAvatar.sprite = LoggedOutTexture;
    }

    int[] Levels =
    {
        // 10k = 10 Lvl, 100k = 35 Lvl, 1M = 75 Lvl, 10M = 120 Lvl, 100M = 185 Lvl
        100, 500, 1000, 2000, 3000, 4000, 5500, 7000, 8500, 10000, // 1 - 10
        12000, 14000, 16000, 18000, 20000, 22500, 25000, 27500, 30000, 32500, // 11 - 20
        35000, 37500, 40000, 45000, 50000, 55000, 60000, 65000, 70000, 75000, // 21 - 30
        80000, 85000, 90000, 95000, 100000, 110000, 120000, 130000, 140000, 150000, // 31 - 40
        160000, 170000, 180000, 190000, 200000, 220000, 240000, 260000, 280000, 300000, // 41 - 50
        320000, 340000, 360000, 380000, 400000, 420000, 440000, 460000, 480000, 500000, // 51 - 60
        530000, 560000, 590000, 620000, 650000, 680000, 710000, 740000, 770000, 800000, // 61 - 70
        840000, 880000, 920000, 960000, 1000000, 1050000, 1100000, 1150000, 1200000, 1250000,  // 71 - 80
        1300000, 1350000, 1400000, 1450000, 1500000, 1600000, 1700000, 1800000, 1900000, 2000000, // 81 - 90
        2200000, 2400000, 2600000, 2800000, 3000000, 3200000, 3400000, 3600000, 3800000, 4000000, // 91 - 100
        4250000, 4500000, 4750000, 5000000, 5250000, 5500000, 5750000, 6000000, 6350000, 6650000, // 101 - 110
        7000000, 7350000, 7650000, 8000000, 8350000, 8650000, 9000000, 9350000, 9650000, 10000000, // 111 - 120
        10500000, 11000000, 11500000, 12000000, 12500000, 13000000, 13500000, 14000000, 14500000, 15000000, // 121 - 130
        15500000, 16000000, 16500000, 17000000, 17500000, 18000000, 18500000, 19000000, 19500000, 20000000, // 131 - 140
        21000000, 22000000, 23000000, 24000000, 25000000, 26000000, 27000000, 28000000, 29000000, 30000000, // 141 - 150
        31000000, 32000000, 33000000, 34000000, 35000000, 36000000, 37000000, 38000000, 39000000, 40000000, // 151 - 160
        42000000, 44000000, 46000000, 48000000, 50000000, 52000000, 54000000, 56000000, 58000000, 60000000, // 161 - 170
        62000000, 64000000, 66000000, 68000000, 70000000, 73000000, 76000000, 79000000, 82000000, 85000000, // 171 - 180
        88000000, 91000000, 94000000, 97000000, 100000000, // 181 - 185
        // 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, // pattern to copy
        999999999, // blockade
    };

    public int XpToLevel(int Xp)
    {
        int Level;
        for (Level = 0; Level < Levels.Length; Level++)
        {
            if (Levels[Level] > Xp)
            {
                break;
            }
        }
        return Level;
    }

    public int LevelToXp(int Level)
    {
        return Levels[Level];
    }
    
    IEnumerator GetAvatar(string Username)
    {
        UnityWebRequest Www = UnityWebRequestTexture.GetTexture("http://192.168.1.8/avatars/" + Username + ".jpg");
        yield return Www.SendWebRequest();

        if (Www.isNetworkError || Www.isHttpError)
        {
            Console.Log("Error trying download avatar: " + Www.error);
        }
        else
        {
            AccountAvatar.sprite = Sprite.Create(((DownloadHandlerTexture)Www.downloadHandler).texture, new Rect(0, 0, 128, 128), new Vector2());
        }
    }

    void SetAvatar(string Username)
    {
        Username = Username.Substring(0, 1).ToUpper();
        Regex Pattern = new Regex(@"^[A-Z0-9]+$");
        if(!Pattern.IsMatch(Username)) { Username = "#"; }
        StartCoroutine(GetAvatar(Username + UnityEngine.Random.Range(0, 3)));
    }

    bool IsNicknameValid(string Value)
    {
        Regex Pattern = new Regex(@"^[\w.-]+$");
        return (Pattern.IsMatch(Value) && Value.Length >= 4 && Value.Length <= 24);
    }

    bool IsPasswordValid(string Value)
    {
        Regex PatternA = new Regex(@"^.*[\d]+.*$");
        Regex PatternB = new Regex(@"^.*[\p{Lu}]+.*$");
        Regex PatternC = new Regex(@"^.*[\p{Ll}]+.*$");
        return (PatternA.IsMatch(Value) && PatternB.IsMatch(Value) && PatternC.IsMatch(Value) && Value.Length >= 5 && Value.Length <= 48);
    }

    bool IsEmailValid(string Value)
    {
        Regex Pattern = new Regex(@"^[\w.-]+\@[\w.-]+\.[\w.-]+$");
        bool MailValid(string Mail) { try { MailAddress x = new MailAddress(Mail); return true; } catch { return false; } }
        return (Pattern.IsMatch(Value) && MailValid(Value) && Value.Length >= 6 && Value.Length <= 96);
    }

    public void NetworkAction(string[] Message)
    {
        switch (Message[1])
        {
            case ("Login"):
            {
                if (Message[2] == "Success")
                {
                    LoggedIn = true;

                    ClientServer.Script.Send(new string[] { "Account", "Update", "X" });
                }
                if (Message[2] == "Failed")
                {
                    if (Message[3] == "WrongUsernameOrPassword")
                    {
                        Alert.Script.CreateAlert("Error", "Login failed. Reason: Wrong username or password. Please try again.", false);
                    }

                    Settings.Del("Nickname");
                    Settings.Del("Password");
                }
                break;
            }
            case ("Register"):
            {
                if (Message[2] == "Success")
                {
                    LoggedIn = true;

                    GuiChanger.Script.SetGui(GuiChanger.Script.CurrentGui + " Account");

                    ClientServer.Script.Send(new string[] { "Account", "Update", "X" });
                }
                if (Message[2] == "Failed")
                {
                    if (Message[3] == "WrongNickname")
                    {
                        Alert.Script.CreateAlert("Error", "Your nickname is invalid. Your nickname should only be letters and numbers and it must be between 4 and 24 characters long.", false);
                    }
                    if (Message[3] == "WrongPassword")
                    {
                        Alert.Script.CreateAlert("Error", "Your password is invalid. Your password should contain big letter, small letter and number and it must be between 5 and 48 characters long.", false);
                    }
                    if (Message[3] == "WrongEmail")
                    {
                        Alert.Script.CreateAlert("Error", "Your email is invalid. Your email should contain at and dot and it must be between 6 and 96 characters long.", false);
                    }
                    if (Message[3] == "UserAlreadyExists")
                    {
                        Alert.Script.CreateAlert("Error", "Unfortunately, user with the same username already exists. Please choose another username and try again.", false);
                    }

                    Settings.Del("Nickname");
                    Settings.Del("Password");
                }
                break;
            }
            case ("Update"):
            {
                if (Message[2] == "X")
                {
                    AccountInformation[0] = Message[3];
                    AccountInformation[1] = "#####";
                    AccountInformation[2] = Message[4];
                    AccountInformation[3] = Message[5];
                    AccountInformation[4] = Message[6];

                    NewAccountInformation = true;
                    Nickname = AccountInformation[0];
                }
                else
                {
                    // other account
                }
                break;
            }
        }
    }
}
