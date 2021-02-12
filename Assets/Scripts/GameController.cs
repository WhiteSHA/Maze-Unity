using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]  

public class GameController : MonoBehaviour
{
    [SerializeField] protected FpsMovement player;
    [SerializeField] protected Text timeLabel;
    [SerializeField] protected Text scoreLabel;

    private MazeConstructor generator;

    public int Rows;
    public int Cols;

    private DateTime startTime;
    private int timeLimit;
    private int reduceLimitBy;

    private int score;
    private int goalReached;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    private void StartNewGame()
    {
        timeLimit = 80;
        reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

    private void StartNewMaze()
    {
        int pointOfStartGate = generator.GenerateNewMaze(Rows, Cols, OnStartTrigger, OnGoalTrigger);

        float x = generator.startCol * generator.hallWidth;
        float y = 1;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);

        goalReached = 0;
        player.enabled = true;

        timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;
    }

    void Update()
    {
        if(!player.enabled)
        {
            return;
        }

        int timeUsed = (int)(DateTime.Now - startTime).TotalSeconds;
        int timeLeft = timeLimit - timeUsed;

        if(timeLeft > 0)
        {
            timeLabel.text = timeLeft.ToString();
        }
        else
        {
            timeLabel.text = "TIME UP";
            player.enabled = false;

            Invoke("StartNewGame", 4);
        }
    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        Debug.Log("Goal !");
        goalReached++;
        
        score++;
        scoreLabel.text = score.ToString();

        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        if(goalReached >= 3)
        {
            Debug.Log("Finish!!!");

            player.enabled = false;

            Invoke("StartNewMaze", 4);
        }
    }
}
