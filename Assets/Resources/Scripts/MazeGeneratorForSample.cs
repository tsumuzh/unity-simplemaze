using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MazeGeneratorForSample : MonoBehaviour
{
    [SerializeField] int[,] mazeData;
    [SerializeField] GameObject wall, cube, pillar, startPoint, endPoint, player, cam;
    int row = 8, col = 8;

    int posx, posy, posx_, posy_, direction, horiz, vert;
    void Start()
    {
        if (PlayerPrefs.HasKey("row"))
        {
            row = PlayerPrefs.GetInt("row");
            col = PlayerPrefs.GetInt("col");
        }

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

        /*  for (int i = 0; i < row * 2 + 3; i++)
          {
              for (int j = 0; j < col * 2 + 3; j++)
              {
                  if (mazeData[i, j] == 0)
                  {
                      Instantiate(cube, new Vector3(i, 0, j), Quaternion.identity);
                  }
              }
          }*/

        //  Instantiate(cube, new Vector3(posx, 0, posy), Quaternion.identity);


        while (true)
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

                /*    Instantiate(cube, new Vector3(posx_, 0, posy_), Quaternion.identity); //通路にブロックを置く
                    Instantiate(cube, new Vector3((posx + posx_) / 2, 0, (posy + posy_) / 2), Quaternion.identity); //通路にブロックを置く*/
                mazeData[(posx + posx_) / 2, (posy + posy_) / 2] = 0;

                posx = posx_;
                posy = posy_;
            }
            else
            {
                if (checkMazeComplete()) break;

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

        setWall();
        setStartAndEnd();
        if (row > col)
        {
            cam.GetComponent<Camera>().orthographicSize = row / 2f + 2;
        }
        else
        {
            cam.GetComponent<Camera>().orthographicSize = col / 2f + 2;
        }
        cam.transform.position = new Vector3((row + 1) / 2f, 10, (col + 1) / 2f);

        PlayerPrefs.SetInt("row", row);
        PlayerPrefs.SetInt("col", col);
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

    void setWall()
    {
        for (int i = 0; i < horiz; i++)
        {
            for (int j = 0; j < vert; j++)
            {
                if (mazeData[i, j] == 1)
                {
                    if (i % 2 == 0 || j % 2 == 0)
                    {
                        if (i % 2 == 0)
                        {
                            Instantiate(wall, new Vector3(i / 2f, 0, j / 2f), Quaternion.Euler(0, 90, 0));
                        }
                        else
                        {
                            Instantiate(wall, new Vector3(i / 2f, 0, j / 2f), Quaternion.identity);
                        }
                    }
                    else
                    {
                        Instantiate(pillar, new Vector3(i / 2f, 0, j / 2f), Quaternion.identity);
                    }
                }
            }
        }
    }

    void setStartAndEnd()
    {
        Vector2[] deadEndList = new Vector2[0];
        Vector2 startPos = Vector2.zero, endPos = Vector2.zero;
        for (int i = 0; i < horiz; i++)
        {
            for (int j = 0; j < vert; j++)
            {
                if (mazeData[i, j] == 0 && i > 0 && i < horiz - 1 && j > 0 && j < vert - 1)
                {
                    int c = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        switch (k)
                        {
                            case 0:
                                if (mazeData[i + 1, j] == 1) c++;
                                break;
                            case 1:
                                if (mazeData[i - 1, j] == 1) c++;
                                break;
                            case 2:
                                if (mazeData[i, j + 1] == 1) c++;
                                break;
                            case 3:
                                if (mazeData[i, j - 1] == 1) c++;
                                break;
                        }
                    }
                    if (c == 3)
                    {
                        Array.Resize(ref deadEndList, deadEndList.Length + 1);
                        deadEndList[deadEndList.Length - 1] = new Vector2(i, j);
                    }
                }
            }
        }

        while (startPos == endPos)
        {
            startPos = deadEndList[UnityEngine.Random.Range(0, deadEndList.Length)];
            endPos = deadEndList[UnityEngine.Random.Range(0, deadEndList.Length)];
        }

        player.transform.position = new Vector3(startPos.x, 0, startPos.y) / 2;
        startPoint.transform.position = new Vector3(startPos.x, 0, startPos.y) / 2;
        endPoint.transform.position = new Vector3(endPos.x, 0, endPos.y) / 2;
        player.SetActive(true);

    }

    public void setRow(int n)
    {
        row = n;
    }
    public void setCol(int n)
    {
        col = n;
    }
}
