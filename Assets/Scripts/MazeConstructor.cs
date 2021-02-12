using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int posX;
    public int posY;

    public Point(int X, int Y)
    {
        this.posX = X;
        this.posY = Y;
    }
}

public class MazeConstructor : MonoBehaviour
{
    public bool showDebug;
    private MazeDataGenerator dataGenerator;
    private MazeMeshGenerator meshGenerator;

    [SerializeField] private Material mazeMat1;
    [SerializeField] private Material mazeMat2;
    [SerializeField] private Material startMat;
    [SerializeField] private Material treasureMat;    

    public int[,] data
    {
        get; private set;
    }

    void Awake()
    {
        data = new int[,]
        {
            {1, 1, 1},
            {1, 0, 1},
            {1, 1, 1}
        };

        dataGenerator = new MazeDataGenerator();
        meshGenerator = new MazeMeshGenerator();
    }

    public int GenerateNewMaze(int sizeRows, int sizeCols, TriggerEventHandler startCallback = null, TriggerEventHandler goalCallback = null)
    {
        if(sizeRows % 2 == 0 && sizeCols % 2 == 0)
        {
            Debug.Log("Odd numbers work better for dungeon size");
        }

        DisposeOldMaze();

        int pointOfGate = 0;
        data = dataGenerator.FromDimensions(sizeRows, sizeCols, ref pointOfGate);

        FindStartPosition();
        FindGoalPosition();

        hallWidth = meshGenerator.width;
        hallHeight = meshGenerator.height;

        DisplayMaze();

        PlaceStartTrigger(startCallback);
        PlaceGoalTrigger(goalCallback);

        return pointOfGate;
    }

    void OnGUI()
    {
        if (!showDebug)
        {
            return;
        }

        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        string msg = "";

        for (int i = rMax; i >= 0; i--)
        {
            for (int j = 0; j <= cMax; j++)
            {
                if (maze[i, j] == 0)
                {
                    msg += "....";
                }
                else if (maze[i, j] == 2)
                {
                    msg += "GA";
                }
                else if (maze[i, j] == 3)
                {
                    msg += "EX";
                }
                else
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), msg);
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = UnityEngine.Vector3.zero;
        go.name = "Procedural Maze";
        go.tag = "Generated";

        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = meshGenerator.FromData(data);

        MeshCollider mc = go.AddComponent<MeshCollider>();
        mc.sharedMesh = mf.mesh;

        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.materials = new Material[2] { mazeMat1, mazeMat2 };
    }

    public float hallWidth
    {
        get; private set;
    }

    public float hallHeight
    {
        get; private set;
    }

    public int startRow
    {
        get; private set;
    }

    public int startCol
    {
        get; private set;
    }

    public List<Point> goalPos = new List<Point>();
    /*{
        get; private set;
    }*/

    public void DisposeOldMaze()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Generated");
        foreach(GameObject go in objects)
        {
            Destroy(go);
        }
    }

    private void FindStartPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);

        for(int i = 0; i <= rMax; ++i)
        {
            if(maze[i, 0] == 2)
            {
                startRow = i;
                startCol = 0;
                return;
            }
        }
    }

    private void FindGoalPosition()
    {
        int[,] maze = data;
        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        for (int i = 0; i <= rMax; ++i)
        {
            if (maze[i, cMax] == 3)
            {
                Debug.Log("Goal of right side is detected");
                goalPos.Add(new Point(i, cMax));
                //goalPos = new Point(i, cMax);
            }
        }
        for (int i = 0; i <= cMax; ++i)
        {
            if (maze[rMax, i] == 3)
            {
                Debug.Log("Goal of up side is detected");
                goalPos.Add(new Point(rMax, i));
                //goalPos = new Point(rMax, i);
            }

            if (maze[0, i] == 3)
            {
                Debug.Log("Goal of left side is detected");
                goalPos.Add(new Point(0, i));
                //goalPos = new Point(0, i);
            }
        }

    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        go.name = "Start Trigger";
        go.tag = "Generated";

        // Set is trigger
        go.GetComponent<BoxCollider>().isTrigger = true;
        // Set material
        go.GetComponent<MeshRenderer>().sharedMaterial = startMat;

        TriggerEventRouter tc = go.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        Debug.Log("Count of goald = " + goalPos.Count);
        foreach(Point point in goalPos)
        {
            Debug.Log("Goal point is detected");
            GameObject goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goal.transform.position = new Vector3(point.posY * hallWidth, .5f, point.posX * hallWidth);
            goal.name = ("Exit" + point.posX);
            goal.tag = "Generated";

            goal.GetComponent<BoxCollider>().isTrigger = true;
            goal.GetComponent<MeshRenderer>().sharedMaterial = treasureMat;

            TriggerEventRouter tc = goal.AddComponent<TriggerEventRouter>();
            tc.callback = callback;
        }
    }
}
