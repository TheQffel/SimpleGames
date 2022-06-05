using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tetris : MonoBehaviour
{
    Transform[,] TetrisPositions = new Transform[10, 20];
    public GameObject[] BlocksPattern;
    public Material[] Materials;
    public GameObject AllBlocks;
    GameObject CurrentBlock;
    GameObject NextBlock;
    Transform[] Blocks = new Transform[8];
    int BlocksNumber = 0;

    void Start()
    {
        for (int i = 0; i < TetrisPositions.GetLength(0); i++)
        {
            for (int j = 0; j < TetrisPositions.GetLength(1); j++)
            {
                TetrisPositions[i, j] = null;
            }
        }

        CreateBlock();
        SwapBlocks();
        CreateBlock();
    }

    int Timer = 0;
    int TimerLimit = 99;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            Rotate();
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            MoveDown();
            Timer = 99;
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            MoveRight();
        }
    }

    void FixedUpdate()
    {
        Timer++;
        if (Timer > TimerLimit)
        {
            Timer = 0;
            MoveDown();
        }
    }

    void MoveLeft()
    {
        CurrentBlock.transform.localPosition -= new Vector3(1, 0, 0);
        if (!IsMoveValid())
        {
            CurrentBlock.transform.localPosition += new Vector3(1, 0, 0);
        }
    }

    void MoveRight()
    {
        CurrentBlock.transform.localPosition += new Vector3(1, 0, 0);
        if (!IsMoveValid())
        {
            CurrentBlock.transform.localPosition -= new Vector3(1, 0, 0);
        }
    }

    void MoveDown()
    {
        CurrentBlock.transform.localPosition -= new Vector3(0, 0, 1);
        if (!IsMoveValid())
        {
            CurrentBlock.transform.localPosition += new Vector3(0, 0, 1);

            BlockBlocks();
            CheckRows();
            SwapBlocks();
            CreateBlock();
        }
    }

    void Rotate()
    {
        CurrentBlock.transform.localEulerAngles += new Vector3(0, 90, 0);
        for (int i = 0; i < BlocksNumber; i++)
        {
            Blocks[i].transform.parent = AllBlocks.transform;
            Blocks[i].localEulerAngles = new Vector3(0, 0, 0);
        }
        CurrentBlock.transform.localEulerAngles -= new Vector3(0, 90, 0);
        for (int i = 0; i < BlocksNumber; i++)
        {
            Blocks[i].transform.parent = CurrentBlock.transform;
            Blocks[i].localEulerAngles = new Vector3(0, 0, 0);
        }

        if(!IsMoveValid())
        {
            CurrentBlock.transform.localEulerAngles -= new Vector3(0, 90, 0);
            for (int i = 0; i < BlocksNumber; i++)
            {
                Blocks[i].transform.parent = AllBlocks.transform;
                Blocks[i].localEulerAngles = new Vector3(0, 0, 0);
            }
            CurrentBlock.transform.localEulerAngles += new Vector3(0, 90, 0);
            for (int i = 0; i < BlocksNumber; i++)
            {
                Blocks[i].transform.parent = CurrentBlock.transform;
                Blocks[i].localEulerAngles = new Vector3(0, 0, 0);
            }
        }
    }

    bool IsMoveValid()
    {
        int GlobalWidth = (int)Math.Round(CurrentBlock.transform.localPosition.x);
        int GlobalHeight = (int)Math.Round(CurrentBlock.transform.localPosition.z);
        foreach (Transform Block in CurrentBlock.transform)
        {
            int LocalWidth = GlobalWidth - (int)Math.Round(Block.localPosition.y);
            int LocalHeight = GlobalHeight + (int)Math.Round(Block.localPosition.x);
            if (LocalWidth < 0 || LocalWidth > 9)
            {
                return false;
            }
            if (LocalHeight < 0 || LocalHeight > 21)
            {
                return false;
            }
            if (TetrisPositions[LocalWidth, LocalHeight] != null)
            {
                return false;
            }
        }
        return true;
    }

    void BlockBlocks()
    {
        for (int i = 0; i < BlocksNumber; i++)
        {
            Blocks[i].parent = AllBlocks.transform;
            int x = (int)Math.Round(Blocks[i].localPosition.x);
            int z = (int)Math.Round(Blocks[i].localPosition.z);
            Blocks[i].localPosition = new Vector3(x, 0, z);
            Blocks[i].localEulerAngles = new Vector3(0, 0, 0);
            TetrisPositions[x, z] = Blocks[i];
        }
        Destroy(CurrentBlock);
    }

    void CheckRows()
    {
        for (int j = 0; j < TetrisPositions.GetLength(1); j++)
        {
            int RowCount = 0;
            for (int i = 0; i < TetrisPositions.GetLength(0); i++)
            {
                if(TetrisPositions[i, j] != null)
                {
                    RowCount++;
                }
            }
            Debug.Log(RowCount);
            if(RowCount > 9)
            {
                for (int i = 0; i < TetrisPositions.GetLength(0); i++)
                {
                    Destroy(TetrisPositions[i, j].gameObject);
                    TetrisPositions[i, j] = null;
                    for (int k = j; k < TetrisPositions.GetLength(1) - 1; k++)
                    {
                        if (TetrisPositions[i, k + 1] != null)
                        {
                            TetrisPositions[i, k] = TetrisPositions[i, k + 1];
                            TetrisPositions[i, k].transform.localPosition -= new Vector3(0, 0, 1);
                            TetrisPositions[i, k + 1] = null;
                        }
                    }
                }
            }
        }
    }

    void CreateBlock()
    {
        NextBlock = Instantiate(BlocksPattern[Random.Range(0, BlocksPattern.Length)], new Vector3(), BlocksPattern[2].transform.rotation);
        NextBlock.name = "NextBlock";
        NextBlock.transform.localEulerAngles = new Vector3(90, 0, 90);
        NextBlock.transform.parent = AllBlocks.transform;
        NextBlock.transform.localScale = new Vector3(1, 1, 1);
        NextBlock.transform.localPosition = new Vector3(17.5f, 0, 17.5f);
        int Color = Random.Range(0, 4);
        foreach (Transform Block in NextBlock.transform)
        {
            Block.GetComponent<MeshRenderer>().material = Materials[Color];
        }
    }

    void SwapBlocks()
    {
        CurrentBlock = NextBlock;
        CurrentBlock.name = "CurrentBlock";

        CurrentBlock.transform.localPosition = new Vector3(5, 0, 20);

        BlocksNumber = 0;
        for (int i = 0; i < Blocks.Length; i++)
        {
            Blocks[i] = null;
        }

        foreach (Transform Block in CurrentBlock.transform)
        {
            Blocks[BlocksNumber] = Block;
            BlocksNumber++;
        }
    }
}
