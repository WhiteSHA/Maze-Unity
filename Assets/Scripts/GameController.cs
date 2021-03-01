﻿using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MazeConstructor))]  

public class GameController : MonoBehaviour
{
    [SerializeField] protected FpsMovement player;
    [SerializeField] protected Text timeLabel;
    [SerializeField] protected Text scoreLabel;
    [SerializeField] protected Camera miniMapCam;

    private MazeConstructor generator;

    public int Rows;
    public int Cols;

    private DateTime startTime;
    private int timeLimit;
    //private int reduceLimitBy;

    private int score;
    //private int goalReached;

    // Start is called before the first frame update
    void Start()
    {
        generator = GetComponent<MazeConstructor>();
        StartNewGame();
    }

    private void StartNewGame()
    {
        timeLimit = 80;
        //reduceLimitBy = 5;
        startTime = DateTime.Now;

        score = 0;
        scoreLabel.text = score.ToString();

        StartNewMaze();
    }

    private void StartNewMaze()
    {
        SetPlayerStartPoint();

        int pointOfStartGate = generator.GenerateNewMaze(Rows, Cols, OnStartTrigger, OnGoalTrigger);

        //timeLimit -= reduceLimitBy;
        startTime = DateTime.Now;

        scoreLabel.text = score.ToString();
        scoreLabel.alignment = TextAnchor.MiddleCenter;

        SetPlayerStartPoint();
        SetMiniMapCameraSettings();

        //goalReached = 0;
        player.enabled = true;
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

            
            generator.goalPos.Clear();

            Invoke(nameof(StartNewGame), 1);
        }
    }

    private void SetPlayerStartPoint()
    {
        float x = generator.startCol * generator.hallWidth;
        float y = 2.0f;
        float z = generator.startRow * generator.hallWidth;
        player.transform.position = new Vector3(x, y, z);
        player.transform.eulerAngles = new Vector3(
            player.transform.eulerAngles.x,
            90,
            player.transform.eulerAngles.z
        );
    }

    private void SetMiniMapCameraSettings()
    {

    }

    private void OnGoalTrigger(GameObject trigger, GameObject other)
    {
        //goalReached++;

        /*score++;
        scoreLabel.text = score.ToString();
        */

        foreach (Point go in generator.goalPos)
        {
            if (go.posX == trigger.transform.position.x && go.posY == trigger.transform.position.y)
            {
                generator.goalPos.Remove(go);
            }
        }
        generator.goalPos.Clear();

        if (trigger.name == "True Exit")
        {
            GameObject gate = GameObject.Find("Exit Gate T");
            if (gate != null)
            {
                Light[] lights = gate.GetComponentsInChildren<Light>();
                if (lights != null)
                {
                    foreach (Light light in lights)
                    {
                        light.intensity = 3;
                        light.color = new Color(0.255f, 0.785f, 0.225f, 1f);
                    }
                }

                ParticleSystem[] particleSysytems = gate.GetComponentsInChildren<ParticleSystem>();
                if (lights != null)
                {
                    foreach (ParticleSystem particalSys in particleSysytems)
                    {
                        if (particalSys.name != "Smoke" && particalSys.name != "Sparks")
                        {
                            ParticleSystem.MainModule settings = particalSys.GetComponent<ParticleSystem>().main;
                            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0.4937346f, 1, 0.2764151f, 0.7f));
                        }
                        else if (particalSys.name == "Sparks")
                        {
                            ParticleSystem.MainModule settings = particalSys.GetComponent<ParticleSystem>().main;
                            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0.4937346f, 1, 0.2764151f, 0.6f), new Color(0.4937346f, 1, 0.2764151f, 0.8f));
                        }
                    }
                }
            }
            
            score++;
            scoreLabel.text = "You win";
            scoreLabel.alignment = TextAnchor.MiddleLeft;
            player.enabled = false;

            Invoke(nameof(StartNewMaze), 1);
        }
        else
        {
            GameObject gate = GameObject.Find("Exit Gate " + trigger.name.Substring(4));
            if (gate != null)
            {
                Light[] lights = gate.GetComponentsInChildren<Light>();
                if (lights != null)
                {
                    foreach (Light light in lights)
                    {
                        light.intensity = 5;
                        light.color = new Color(0.785f, 0.255f, 0.225f, 1f);
                    }
                }

                ParticleSystem[] particleSysytems = gate.GetComponentsInChildren<ParticleSystem>();
                if (lights != null)
                {
                    foreach (ParticleSystem particalSys in particleSysytems)
                    {
                        if (particalSys.name != "Smoke" && particalSys.name != "FireDark" && particalSys.name != "Sparks")
                        {
                            ParticleSystem.MainModule settings = particalSys.GetComponent<ParticleSystem>().main;
                            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0.785f, 0.255f, 0.225f, 1f));
                        }
                        else if(particalSys.name == "Sparks")
                        {
                            ParticleSystem.MainModule settings = particalSys.GetComponent<ParticleSystem>().main;
                            settings.startColor = new ParticleSystem.MinMaxGradient(new Color(0.785f, 0.255f, 0.225f, 0.255f), new Color(1f, 0.4745701f, 0f, 1f));
                        }
                    }
                }
            }
        }

        Destroy(trigger);
    }

    private void OnStartTrigger(GameObject trigger, GameObject other)
    {
        /*if(goalReached >= 3)
        {
            player.enabled = false;

            Invoke("StartNewMaze", 1);
        }*/
    }
}
