using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class Friends : MonoBehaviour
{
    public static Friends Script;

    void Awake()
    {
        Script = GetComponent<Friends>();
    }

    public RectTransform Content;
    public RectTransform EmptyFriend;
    public RectTransform NewFriend;
    public RectTransform InviteFriend;
    public RectTransform ContentFrame;

    RectTransform[] CurrentFriends = new RectTransform[256];

    public string[] FriendsData = new string[0];
    public bool Refresh = false;

    float MinSize = -1;
    bool ChangeSize = false;

    void Update()
    {
        bool Change = ChangeSize;
        if (ContentFrame.sizeDelta.y != MinSize)
        {
            MinSize = ContentFrame.sizeDelta.y;
            ChangeSize = true;
        }
        if (Change)
        {
            NewSize(FriendsData.Length);
            ChangeSize = false;
        }
        if (Refresh)
        {
            NewFriendsList();
            Refresh = false;
        }
    }

    void RefreshFriends()
    {
        ClientServer.Script.Send(new string[] { "Friends", "Update", "X" });
    }

    void NewFriendsList()
    {
        for (int i = 0; i < CurrentFriends.Length; i++)
        {
            if (CurrentFriends[i] == null)
            {
                break;
            }
            Destroy(CurrentFriends[i].gameObject);
            CurrentFriends[i] = null;
        }
        if (FriendsData[0].Length > 1)
        {
            for (int i = 0; i < FriendsData.Length; i++)
            {
                CurrentFriends[i] = Instantiate(EmptyFriend, Content);
                CurrentFriends[i].anchoredPosition = new Vector2(CurrentFriends[i].anchoredPosition.x, (-100 * i) - 50);
                CurrentFriends[i].name = "Friend";

                CurrentFriends[i].GetComponentsInChildren<Text>()[0].text = "Niknejm";
                //FriendsAvatars[i] = CurrentFriends[i].GetComponentsInChildren<Image>()[1];
                CurrentFriends[i].GetComponentsInChildren<Text>()[2].text = "LengTajmAgoł";
            }
            NewSize(FriendsData.Length);
        }
        else
        {
            NewSize(0);
        }

        NewSize(FriendsData.Length);
    }

    void NewSize(int FriendsCount)
    {
        Content.sizeDelta = new Vector2(Content.sizeDelta.x, (FriendsCount + 2) * 100);
        Content.anchoredPosition = new Vector2(Content.anchoredPosition.x, -1000);

        InviteFriend.anchoredPosition = new Vector2(InviteFriend.anchoredPosition.x, (-1 * Content.sizeDelta.y) + 50);
        NewFriend.anchoredPosition = new Vector2(NewFriend.anchoredPosition.x, (-1 * Content.sizeDelta.y) + 150);

        if (Content.sizeDelta.y < MinSize)
        {
            Content.sizeDelta = new Vector2(Content.sizeDelta.x, MinSize);
        }
    }

    public void InviteFriends()
    {
        new NativeShare().SetText("https://www.google.com/search?q=ipa+no+jail+break").Share();
    }

    public async void AddFriend()
    {
        Inputbox.AskForInput("Add Friend");
        await Task.Run(() => Inputbox.WaitForAnswer());
        if (Inputbox.Answer != null)
        {
            //AddFriend(Inputbox.Answer);
        }
    }

    public void NetworkAction(string[] Message)
    {
        switch (Message[1])
        {
            case ("Update"):
            {
                break;
            }
        }
    }
}
