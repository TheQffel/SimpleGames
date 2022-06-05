using UnityEngine;

public class TicTacToe : MonoBehaviour
{
    public Transform[] Fields;
    public Transform[] Moves = new Transform[9];
    int[] Turns = new int[9];

    public Transform Cross;
    public Transform Circle;
    public Transform WinnerLine;

    async void Refresh()
    {
        for (int i = 0; i < Turns.Length; i++)
        {
            if (Moves[i] != null)
            {
                Destroy(Moves[i].gameObject);
            }
            Turns[i] = int.Parse(Game.Script.Data[i]);
            if(Turns[i] > 0)
            {
                Vector3 NewPosition = new Vector3();
                if (i == 0) { NewPosition = new Vector3(-1.75f, 3, 1.75f); }
                if (i == 1) { NewPosition = new Vector3(0, 3, 1.75f); }
                if (i == 2) { NewPosition = new Vector3(1.75f, 3, 1.75f); }
                if (i == 3) { NewPosition = new Vector3(-1.75f, 3, 0); }
                if (i == 4) { NewPosition = new Vector3(0, 3, 0); }
                if (i == 5) { NewPosition = new Vector3(1.75f, 3, 0); }
                if (i == 6) { NewPosition = new Vector3(-1.75f, 3, -1.75f); }
                if (i == 7) { NewPosition = new Vector3(0, 3, -1.75f); }
                if (i == 8) { NewPosition = new Vector3(1.75f, 3, -1.75f); }

                if (Turns[i] == 1)
                {
                    Moves[i] = Instantiate(Cross);
                    Moves[i].localPosition = NewPosition;
                    Moves[i].name = "Cross";
                }
                if(Turns[i] == 2)
                {
                    Moves[i] = Instantiate(Circle);
                    Moves[i].localPosition = NewPosition;
                    Moves[i].name = "Circle";
                }
            }
        }

        Game.Script.WhoMovesNow();

        Game.Script.Refreshed = true;
    }

    void Update()
    {
        if (Game.Script.Name == "TicTacToe")
        {
            if (Game.Script.UpdateNow)
            {
                Game.Script.UpdateNow = false;
                Refresh();
            }

            if (Game.Script.Refreshed)
            {
                if (Game.Script.PlayerTurn())
                {
                    if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
                    {
                        Ray Raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
                        RaycastHit RaycastHit;
                        if (Physics.Raycast(Raycast, out RaycastHit))
                        {
                            for (int i = 0; i < Fields.Length; i++)
                            {
                                if (RaycastHit.collider.transform == Fields[i])
                                {
                                    if (Turns[i] < 1)
                                    {
                                        Game.Script.Turn = 0;
                                        Game.Script.Refreshed = false;
                                        ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), i.ToString(), "" });
                                    }
                                }
                            }
                        }
                    }
                }

                if (Game.Script.ComputerTurn())
                {
                    int i;
                    if (Turns[i = Random.Range(0, Fields.Length - 1)] == 0)
                    {
                        Game.Script.Turn = 0;
                        Game.Script.Refreshed = false;
                        ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), i.ToString(), "" });
                    }
                }
            }
        }
    }
}
