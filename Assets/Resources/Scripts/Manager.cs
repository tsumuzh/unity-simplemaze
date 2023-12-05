using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    int[,] mazeData;
    public Vector2 startPos, endPos, playerPos;
    [SerializeField] GameObject player, generateGroup, moveGroup;
    [SerializeField] Image panel;
    MazeGenerator mazeGenerator;
    [SerializeField] Text rowLabel, colLabel, turnLabel, fixedTurnLabel, timeLabel, fixedTimeLabel;
    int turnCount;
    bool isSolving;
    float time;
    void Start()
    {
        turnLabel.text = "";
        fixedTurnLabel.text = "";
        timeLabel.text = "";
        fixedTimeLabel.text = "";
        mazeGenerator = GetComponent<MazeGenerator>();
        rowLabel.text = mazeGenerator.row.ToString();
        colLabel.text = mazeGenerator.col.ToString();
        isSolving = false;
        time = 0;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow)) Move(0);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(1);
        if (Input.GetKeyDown(KeyCode.UpArrow)) Move(2);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) Move(3);
        if (isSolving)
        {
            time += Time.deltaTime;
            timeLabel.text = time.ToString("f2");
        }
    }
    public void Move(int facing)
    {
        if (isSolving)
        {
            if (playerPos == startPos)
            {
                player.GetComponent<TrailRenderer>().time = Mathf.Infinity;
            }

            switch (facing)
            {
                case 0:
                    if (mazeData[(int)playerPos.x, (int)playerPos.y - 1] == 0)
                    {
                        playerPos += Vector2.down * 2;
                        player.transform.position += Vector3.back;
                    }
                    break;
                case 1:
                    if (mazeData[(int)playerPos.x + 1, (int)playerPos.y] == 0)
                    {
                        playerPos += Vector2.right * 2;
                        player.transform.position += Vector3.right;

                    }
                    break;
                case 2:
                    if (mazeData[(int)playerPos.x, (int)playerPos.y + 1] == 0)
                    {
                        playerPos += Vector2.up * 2;
                        player.transform.position += Vector3.forward;

                    }
                    break;
                case 3:
                    if (mazeData[(int)playerPos.x - 1, (int)playerPos.y] == 0)
                    {
                        playerPos += Vector2.left * 2;
                        player.transform.position += Vector3.left;

                    }
                    break;
            }
            player.transform.position = new Vector3(playerPos.x, 0, playerPos.y) / 2;

            turnCount++;
            turnLabel.text = turnCount.ToString();

            if (playerPos == endPos) isSolving = false;
        }
    }
    public void ChangeRow(int n)
    {
        mazeGenerator.row += n;
        mazeGenerator.row = (int)Mathf.Clamp(mazeGenerator.row, 3, 99);
        rowLabel.text = mazeGenerator.row.ToString();
    }

    public void ChangeCol(int n)
    {
        mazeGenerator.col += n;
        mazeGenerator.col = (int)Mathf.Clamp(mazeGenerator.col, 3, 99);
        colLabel.text = mazeGenerator.col.ToString();
    }

    public void GenerateMazeRunner()
    {
        mazeGenerator.GenerateMaze();
        mazeData = mazeGenerator.getMazeData();
        playerPos = startPos;
        player.transform.position = new Vector3(playerPos.x, 0, playerPos.y) / 2;
        player.SetActive(true);
        generateGroup.SetActive(false);
        moveGroup.SetActive(true);
        turnLabel.text = turnCount.ToString();
        fixedTurnLabel.text = "経過";
        fixedTimeLabel.text = "時間";
        isSolving = true;
    }

    public void playAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
