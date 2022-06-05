using System;
using System.Threading.Tasks;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public Rigidbody DiceGood;
    public Rigidbody DiceBad;
    public Transform[] Rounds;

    public Transform[] RoundsResultA;
    public Transform[] RoundsResultB;
    public Transform[] RoundsResultC;
    public Transform[] RoundsResultD;
    public Transform[] RoundsResultE;

    public Material ResultGood;
    public Material ResultBad;

    int GoodHeight = 0;
    int BadHeight = 0;
    bool Shake = false;
    int[] Moves = new int[8];

    async void Refresh()
    {
        if (Game.Script.ShowMove)
        {
            if (int.Parse(Game.Script.Data[0]) < 8)
            {
                DiceGood.transform.localPosition = new Vector3(-1, int.Parse(Game.Script.Move[0]), 0.75f);
                DiceBad.transform.localPosition = new Vector3(1, int.Parse(Game.Script.Move[4]), 0.75f);
                DiceGood.transform.localEulerAngles = new Vector3(0, 0, 0);
                DiceBad.transform.localEulerAngles = new Vector3(0, 0, 0);

                DiceGood.AddTorque(new Vector3(int.Parse(Game.Script.Move[1]), int.Parse(Game.Script.Move[2]), int.Parse(Game.Script.Move[3])));
                DiceBad.AddTorque(new Vector3(int.Parse(Game.Script.Move[5]), int.Parse(Game.Script.Move[6]), int.Parse(Game.Script.Move[7])));

                while (!DicesStopMoving())
                {
                    await Task.Run(() => Task.Delay(10));
                }
                await Task.Run(() => Task.Delay(1000));
            }
        }

        Shake = false;
        GoodHeight = UnityEngine.Random.Range(5, 11);
        BadHeight = UnityEngine.Random.Range(5, 11);

        if (Game.Script.PlayerTurn() || Game.Script.ComputerTurn())
        {
            DiceGood.isKinematic = true;
            DiceBad.isKinematic = true;

            DiceGood.transform.localPosition = new Vector3(-1, GoodHeight, 0.75f);
            DiceBad.transform.localPosition = new Vector3(1, BadHeight, 0.75f);
            DiceGood.transform.localEulerAngles = new Vector3(-1, 0, 0);
            DiceBad.transform.localEulerAngles = new Vector3(1, 0, 0);
        }

        Transform[][] RoundsValue = new Transform[][] { RoundsResultA, RoundsResultB, RoundsResultC, RoundsResultD, RoundsResultE };

        for (int i = 0; i < Game.Script.Data.Length / 2; i++)
        {
            int Value = int.Parse(Game.Script.Data[(i * 2) + (Game.Script.Player - 1)]);
            float Position = (i - 2) * 0.75f;

            if (Value < 8)
            {
                for (int j = 0; j < RoundsValue[i].Length; j++)
                {
                    RoundsValue[i][j].localPosition = new Vector3(Position, -1, -1.5f);
                }
                RoundsValue[i][Math.Abs(Value)].localPosition = new Vector3(Position, 1, -1.5f);

                if (Value > 0)
                {
                    RoundsValue[i][Math.Abs(Value)].GetComponent<MeshRenderer>().material = ResultGood;
                }
                if (Value < 0)
                {
                    RoundsValue[i][Math.Abs(Value)].GetComponent<MeshRenderer>().material = ResultBad;
                }
                Rounds[i].localPosition = new Vector3(Position, -1, -1.5f);
            }
            else
            {
                Rounds[i].localPosition = new Vector3(Position, 1, -1.5f);
            }
        }

        Game.Script.WhoMovesNow();

        Game.Script.Refreshed = true;
    }

    void Update()
    {
        if (Game.Script.Name == "Dice")
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
                    if (!Shake)
                    {
                        if (Input.acceleration.sqrMagnitude > 5)
                        {
                            Moves = new int[] { UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001) };

                            DiceGood.isKinematic = false;
                            DiceBad.isKinematic = false;

                            DiceGood.AddTorque(new Vector3(Moves[0], Moves[1], Moves[2]));
                            DiceBad.AddTorque(new Vector3(Moves[3], Moves[4], Moves[5]));

                            Shake = true;
                        }
                    }
                    else
                    {
                        if (DicesStopMoving())
                        {
                            DiceGood.velocity = new Vector3(0, 0, 0);
                            DiceBad.velocity = new Vector3(0, 0, 0);
                            DiceGood.angularVelocity = new Vector3(0, 0, 0);
                            DiceBad.angularVelocity = new Vector3(0, 0, 0);

                            Game.Script.Turn = 0;
                            Game.Script.Refreshed = false;
                            ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), (DiceResult(true) - DiceResult(false)).ToString(), GoodHeight + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + BadHeight + " " + Moves[3] + " " + Moves[4] + " " + Moves[5] });
                        }
                    }
                }

                if (Game.Script.ComputerTurn())
                {
                    if (!Shake)
                    {
                        DiceGood.transform.localPosition = new Vector3(-1, GoodHeight, 0.75f);
                        DiceBad.transform.localPosition = new Vector3(1, BadHeight, 0.75f);

                        Moves = new int[] { UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001), UnityEngine.Random.Range(-1000, 1001) };

                        DiceGood.isKinematic = false;
                        DiceBad.isKinematic = false;

                        DiceGood.AddTorque(new Vector3(Moves[0], Moves[1], Moves[2]));
                        DiceBad.AddTorque(new Vector3(Moves[3], Moves[4], Moves[5]));

                        Shake = true;
                    }
                    else
                    {
                        if (DicesStopMoving())
                        {
                            DiceGood.velocity = new Vector3(0, 0, 0);
                            DiceBad.velocity = new Vector3(0, 0, 0);
                            DiceGood.angularVelocity = new Vector3(0, 0, 0);
                            DiceBad.angularVelocity = new Vector3(0, 0, 0);

                            Game.Script.Turn = 0;
                            Game.Script.Refreshed = false;
                            ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), (DiceResult(true) - DiceResult(false)).ToString(), GoodHeight + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + BadHeight + " " + Moves[3] + " " + Moves[4] + " " + Moves[5] });
                        }
                    }
                }
            }
        }
    }

    bool DicesStopMoving()
    {
        return (DiceGood.transform.localPosition.y < 0.6) && (DiceBad.transform.localPosition.y < 0.6);
    }

    int DiceResult(bool Good)
    {
        float Rotation = 100;
        int Result = 0;

        if (Good)
        {
            if (Vector3.Angle(DiceGood.transform.up, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceGood.transform.up, Vector3.up);
                Result = 1;
            }
            if (Vector3.Angle(-DiceGood.transform.up, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceGood.transform.up, Vector3.up);
                Result = 6;
            }
            if (Vector3.Angle(DiceGood.transform.right, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceGood.transform.right, Vector3.up);
                Result = 5;
            }
            if (Vector3.Angle(-DiceGood.transform.right, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceGood.transform.right, Vector3.up);
                Result = 2;
            }
            if (Vector3.Angle(DiceGood.transform.forward, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceGood.transform.forward, Vector3.up);
                Result = 3;
            }
            if (Vector3.Angle(-DiceGood.transform.forward, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceGood.transform.forward, Vector3.up);
                Result = 4;
            }
        }
        else
        {
            if (Vector3.Angle(DiceBad.transform.up, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceBad.transform.up, Vector3.up);
                Result = 1;
            }
            if (Vector3.Angle(-DiceBad.transform.up, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceBad.transform.up, Vector3.up);
                Result = 6;
            }
            if (Vector3.Angle(DiceBad.transform.right, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceBad.transform.right, Vector3.up);
                Result = 5;
            }
            if (Vector3.Angle(-DiceBad.transform.right, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceBad.transform.right, Vector3.up);
                Result = 2;
            }
            if (Vector3.Angle(DiceBad.transform.forward, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(DiceBad.transform.forward, Vector3.up);
                Result = 3;
            }
            if (Vector3.Angle(-DiceBad.transform.forward, Vector3.up) < Rotation)
            {
                Rotation = Vector3.Angle(-DiceBad.transform.forward, Vector3.up);
                Result = 4;
            }
        }

        return Result;
    }
}
