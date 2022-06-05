using System.Threading.Tasks;
using UnityEngine;

public class CoinToss : MonoBehaviour
{
    public Rigidbody Coin;
    public Transform[] Rounds;
    public Transform[] RoundsYes;
    public Transform[] RoundsNo;

    int Rotation = 0;
    int Timeout = 0;
    bool Toss = false;
    int[] Moves = new int[4];

    async void Refresh()
    {
        if (Game.Script.ShowMove)
        {
            if (int.Parse(Game.Script.Data[0]) < 4)
            {
                Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);
                Coin.transform.localEulerAngles = new Vector3(int.Parse(Game.Script.Move[0]) * 180, 280 - (int.Parse(Game.Script.Move[0]) * 200), 180);

                Coin.AddForce(new Vector3(0, int.Parse(Game.Script.Move[1]), 0));
                Coin.AddTorque(new Vector3(int.Parse(Game.Script.Move[2]), int.Parse(Game.Script.Move[3]), int.Parse(Game.Script.Move[4])));

                for (int i = 0; i < 100; i++)
                {
                    Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);
                    await Task.Run(() => Task.Delay(10));
                }
                while (!CoinStopMoving())
                {
                    Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);
                    await Task.Run(() => Task.Delay(10));
                }
                await Task.Run(() => Task.Delay(1000));
            }
        }

        Timeout = 0;
        Toss = false;
        Rotation = Random.Range(0, 2);

        if (Game.Script.PlayerTurn() || Game.Script.ComputerTurn())
        {
            Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);
            Coin.transform.localEulerAngles = new Vector3(Rotation * 180, 280 - (Rotation * 200), 180);
        }

        for (int i = 0; i < Game.Script.Data.Length / 2; i++)
        {
            int Value = int.Parse(Game.Script.Data[(i * 2) + (Game.Script.Player - 1)]);
            float Position = (i - 2) * 0.75f;

            if (Value < 4)
            {
                if (Value == 1)
                {
                    RoundsYes[i].localPosition = new Vector3(Position, 1, -1.5f);
                    RoundsNo[i].localPosition = new Vector3(Position, -1, -1.5f);
                }
                if (Value == 0)
                {
                    RoundsYes[i].localPosition = new Vector3(Position, -1, -1.5f);
                    RoundsNo[i].localPosition = new Vector3(Position, 1, -1.5f);
                }
                Rounds[i].localPosition = new Vector3(Position, -1, -1.5f);
            }
            else
            {
                RoundsYes[i].localPosition = new Vector3(Position, -1, -1.5f);
                RoundsNo[i].localPosition = new Vector3(Position, -1, -1.5f);
                Rounds[i].localPosition = new Vector3(Position, 1, -1.5f);
            }
        }

        Game.Script.WhoMovesNow();

        Game.Script.Refreshed = true;
    }

    void Update()
    {
        if (Game.Script.Name == "CoinToss")
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
                    Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);

                    if (!Toss)
                    {
                        if (Input.acceleration.z > 0.5f)
                        {
                            Timeout++;
                        }
                        if (Input.acceleration.z < -0.5f)
                        {
                            if (Timeout > 0 && Timeout < 250)
                            {
                                Moves = new int[] { Random.Range(500, 2501), Random.Range(-1000, 1001), Random.Range(-1000, 1001), Random.Range(-1000, 1001) };

                                Coin.AddForce(new Vector3(0, Moves[0], 0));
                                Coin.AddTorque(new Vector3(Moves[1], Moves[2], Moves[3]));

                                Toss = true;
                            }
                            Timeout = 0;
                        }
                    }
                    else
                    {
                        Timeout--;

                        if (Timeout < -100)
                        {
                            if (CoinStopMoving())
                            {
                                Coin.velocity = new Vector3(0, 0, 0);
                                Coin.angularVelocity = new Vector3(0, 0, 0);

                                if (Coin.transform.localEulerAngles.x > 90 && Coin.transform.localEulerAngles.x < 270)
                                {
                                    Toss = !Toss;
                                }
                                if (Coin.transform.localEulerAngles.z > 90 && Coin.transform.localEulerAngles.z < 270)
                                {
                                    Toss = !Toss;
                                }

                                if (Toss)
                                {
                                    Game.Script.Turn = 0;
                                    Game.Script.Refreshed = false;
                                    ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), "1", Rotation + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + Moves[3] });
                                }
                                else
                                {
                                    Game.Script.Turn = 0;
                                    Game.Script.Refreshed = false;
                                    ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), "0", Rotation + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + Moves[3] });
                                }
                            }
                        }
                    }
                }

                if (Game.Script.ComputerTurn())
                {
                    Coin.transform.localPosition = new Vector3(0, Coin.transform.localPosition.y, 0.75f);

                    if (Timeout == 0)
                    {
                        Moves = new int[] { Random.Range(500, 2501), Random.Range(-1000, 1001), Random.Range(-1000, 1001), Random.Range(-1000, 1001) };

                        Coin.AddForce(new Vector3(0, Moves[0], 0));
                        Coin.AddTorque(new Vector3(Moves[1], Moves[2], Moves[3]));

                        Toss = true;
                    }

                    Timeout--;

                    if (Timeout < -100)
                    {
                        if (CoinStopMoving())
                        {
                            Coin.velocity = new Vector3(0, 0, 0);
                            Coin.angularVelocity = new Vector3(0, 0, 0);

                            if (Coin.transform.localEulerAngles.x > 90 && Coin.transform.localEulerAngles.x < 270)
                            {
                                Toss = !Toss;
                            }
                            if (Coin.transform.localEulerAngles.z > 90 && Coin.transform.localEulerAngles.z < 270)
                            {
                                Toss = !Toss;
                            }

                            if (Toss)
                            {
                                Game.Script.Turn = 0;
                                Game.Script.Refreshed = false;
                                ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), "1", Rotation + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + Moves[3] });
                            }
                            else
                            {
                                Game.Script.Turn = 0;
                                Game.Script.Refreshed = false;
                                ClientServer.Script.Send(new string[] { "Games", "Move", Game.Script.Id.ToString(), "0", Rotation + " " + Moves[0] + " " + Moves[1] + " " + Moves[2] + " " + Moves[3] });
                            }
                        }
                    }
                }
            }
        }
    }

    bool CoinStopMoving()
    {
        return (Coin.transform.localPosition.y < 0.15);
    }
}
