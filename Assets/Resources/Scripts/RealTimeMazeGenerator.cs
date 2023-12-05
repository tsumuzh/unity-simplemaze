using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RealTimeMazeGenerator : MonoBehaviour
{
    [SerializeField] int[,] mazeData;
    [SerializeField] GameObject wall;
    [SerializeField] int row = 8, col = 8;
    bool isCompleted;

    int posx, posy, posx_, posy_, direction, horiz, vert;
    void Start()
    {
        isCompleted = false;
        horiz = row * 2 + 3;
        vert = col * 2 + 3;

        mazeData = new int[horiz, vert];
        for (int i = 0; i < horiz; i++)
        {
            for (int j = 0; j < vert; j++)
            {
                mazeData[i, j] = 1;
            }
        }
        for (int i = 0; i < horiz; i++)
        {
            mazeData[i, 0] = 0;
            mazeData[i, vert - 1] = 0;
        }
        for (int j = 0; j < vert; j++)
        {
            mazeData[0, j] = 0;
            mazeData[horiz - 1, j] = 0;
        }

        posx = UnityEngine.Random.Range(0, row) * 2 + 2;
        posy = UnityEngine.Random.Range(0, col) * 2 + 2;
        posx_ = posx;
        posy_ = posy;

        for (int i = 0; i < row * 2 + 3; i++)
        {
            for (int j = 0; j < col * 2 + 3; j++)
            {
                if (mazeData[i, j] == 0)
                {
                    Instantiate(wall, new Vector3(i, 0, j), Quaternion.identity);
                }
            }
        }

        Instantiate(wall, new Vector3(posx, 0, posy), Quaternion.identity);

    }

    void Update()
    {
        if (!checkMazeComplete())
        {
            mazeData[posx, posy] = 0;
            int[] diggableDirection = new int[0];

            if (!checkSurround(posx, posy))
            {
                for (int i = 0; i < 4; i++) //方向を決定
                {
                    switch (i)
                    {
                        case 0:
                            posy_ = posy - 2;
                            break;
                        case 1:
                            posx_ = posx + 2;
                            break;
                        case 2:
                            posy_ = posy + 2;
                            break;
                        case 3:
                            posx_ = posx - 2;
                            break;
                    }

                    if (mazeData[posx_, posy_] == 1) //方向の候補を決定
                    {
                        Array.Resize(ref diggableDirection, diggableDirection.Length + 1);
                        diggableDirection[diggableDirection.Length - 1] = i;
                    }
                    posx_ = posx;
                    posy_ = posy;
                }

                direction = diggableDirection[UnityEngine.Random.Range(0, diggableDirection.Length)]; //方向を決定

                switch (direction)
                {
                    case 0:
                        posy_ = posy - 2;
                        break;
                    case 1:
                        posx_ = posx + 2;
                        break;
                    case 2:
                        posy_ = posy + 2;
                        break;
                    case 3:
                        posx_ = posx - 2;
                        break;
                }

                Instantiate(wall, new Vector3(posx_, 0, posy_), Quaternion.identity); //通路にブロックを置く
                Instantiate(wall, new Vector3((posx + posx_) / 2, 0, (posy + posy_) / 2), Quaternion.identity); //通路にブロックを置く
                mazeData[(posx + posx_) / 2, (posy + posy_) / 2] = 0;

                posx = posx_;
                posy = posy_;
            }
            else
            {
                posx_ = posx;
                posy_ = posy;

                Vector2[] startablePoints = new Vector2[0];

                for (int i = 0; i < horiz; i++)
                {
                    for (int j = 0; j < vert; j++)
                    {
                        if (i % 2 == 0 && j % 2 == 0 && i != 0 && j != 0 && i != horiz - 1 && j != vert - 1 && mazeData[i, j] == 0)
                        {
                            if (!checkSurround(i, j))
                            {
                                Array.Resize(ref startablePoints, startablePoints.Length + 1);
                                startablePoints[startablePoints.Length - 1] = new Vector2(i, j);
                            }
                        }
                    }
                }

                int r = UnityEngine.Random.Range(0, startablePoints.Length);

                posx = (int)startablePoints[r].x;
                posy = (int)startablePoints[r].y;
                posx_ = posx;
                posy_ = posy;
            }
        }
    }

    bool checkMazeComplete()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
                if (!checkSurround(i * 2 + 2, j * 2 + 2)) return false;
            }
        }

        return true;
    }
    bool checkSurround(int a, int b) //これ以上掘れないならtrue
    {
        for (int i = 0; i < 4; i++)
        {
            int a_ = a;
            int b_ = b;

            switch (i)
            {
                case 0:
                    b_ = b - 2;
                    break;
                case 1:
                    a_ = a + 2;
                    break;
                case 2:
                    b_ = b + 2;
                    break;
                case 3:
                    a_ = a - 2;
                    break;
            }

            if (mazeData[a_, b_] == 1)
            {
                return false;
            }
        }

        return true;
    }
}
