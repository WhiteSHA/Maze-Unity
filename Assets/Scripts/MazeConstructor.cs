using System.Collections.Generic;
using UnityEngine; 

public struct Point
{
    public int posX;
    public int posY;
    public Identificators type;

    public Point(int X, int Y, Identificators type)
    {
        this.posX = X;
        this.posY = Y;
        this.type = type;
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
    [SerializeField] private GameObject startGate;
    [SerializeField] private GameObject exitGate;
    [SerializeField] private GameObject wallTorch;

    private int sizeOfCols;
    private int sizeOfRows;

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
        meshGenerator = new MazeMeshGenerator(wallTorch);
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

        sizeOfRows = sizeRows;
        sizeOfCols = sizeCols;

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
                else if (maze[i, j] == 4)
                {
                    msg += "TE";
                }
                else 
                {
                    msg += "==";
                }
            }
            msg += "\n";
        }

        GUI.Label(new Rect(20, 20, 500, 500), (1.0f / Time.smoothDeltaTime).ToString());
    }

    private void DisplayMaze()
    {
        GameObject go = new GameObject();
        go.transform.position = Vector3.zero;
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
            if(maze[i, 0] == (int)Identificators.START_GATE)
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
            if (maze[i, cMax] == (int)Identificators.EXIT_GATE || maze[i, cMax] == (int)Identificators.TRUE_EXIT_GATE)
            {
                Debug.Log("Goal of right side is detected");
                goalPos.Add(new Point(i, cMax, (Identificators)maze[i, cMax]));
            }
        }
        for (int i = 0; i <= cMax; ++i)
        {
            if (maze[rMax, i] == (int)Identificators.EXIT_GATE || maze[rMax, i] == (int)Identificators.TRUE_EXIT_GATE)
            {
                Debug.Log("Goal of up side is detected");
                goalPos.Add(new Point(rMax, i, (Identificators)maze[rMax, i]));
            }

            if (maze[0, i] == (int)Identificators.EXIT_GATE || maze[0, i] == (int)Identificators.TRUE_EXIT_GATE)
            {
                Debug.Log("Goal of left side is detected");
                goalPos.Add(new Point(0, i, (Identificators)maze[0, i]));

            }
        }

    }

    private void PlaceStartTrigger(TriggerEventHandler callback)
    {
        GameObject go = Instantiate(startGate);
        float x = startCol * hallWidth;
        float y = -5.00f;
        float z = startRow * hallWidth;
        go.transform.position = new Vector3(x - 10.82f, y, z - 3.15f);
        go.tag = "Generated";
        go.AddComponent<BoxCollider>();

        // Set is trigger
        //mc.convex = true;

        GameObject startSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        startSphere.transform.position = new Vector3(startCol * hallWidth, .5f, startRow * hallWidth);
        startSphere.name = "Start Trigger";
        startSphere.tag = "Generated";

        // Set is trigger
        startSphere.GetComponent<SphereCollider>().isTrigger = true;

        // Set material
        startSphere.GetComponent<MeshRenderer>().enabled = false;

        TriggerEventRouter tc = startSphere.AddComponent<TriggerEventRouter>();
        tc.callback = callback;
    }

    private void PlaceGoalTrigger(TriggerEventHandler callback)
    {
        Debug.Log("Count of goald = " + goalPos.Count);
        foreach (Point point in goalPos)
        {
            GameObject go = Instantiate(exitGate);
            float x = point.posY * hallWidth;
            float y = 14.18f;
            float z = point.posX * hallWidth;

            if (point.posX == 0)
            {
                go.transform.position = new Vector3(x - 0.41f, y, z + 9.845f);

                go.transform.eulerAngles = new Vector3(
                    go.transform.eulerAngles.x,
                    go.transform.eulerAngles.y - 180f,
                    go.transform.eulerAngles.z);
            }
            else if(point.posY == sizeOfCols - 1)
            {
                go.transform.position = new Vector3(x - 9.81f, y, z - 0.49f);

                go.transform.eulerAngles = new Vector3(
                    go.transform.eulerAngles.x,
                    go.transform.eulerAngles.y + 90f,
                    go.transform.eulerAngles.z);
            }
            else
            {
                go.transform.position = new Vector3(x + 0.43f, y, z - 9.845f);
            }

            go.tag = "Generated";
            if (point.type == Identificators.TRUE_EXIT_GATE)
                go.name = ("Exit Gate T");
            else
                go.name = ("Exit Gate " + point.posX);
            //

            Debug.Log("Goal point is detected" + point.posX + " " + point.posY);
            GameObject goal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            goal.transform.position = new Vector3(point.posY * hallWidth, .5f, point.posX * hallWidth);
            if(point.type == Identificators.TRUE_EXIT_GATE)
                goal.name = ("True Exit");
            else
                goal.name = ("Exit" + point.posX);
            goal.tag = "Generated";

            BoxCollider bc = goal.GetComponent<BoxCollider>();
            bc.isTrigger = true;
            bc.size = new Vector3(bc.size.x + 2, bc.size.y, bc.size.z);
            goal.GetComponent<MeshRenderer>().enabled = false;

            TriggerEventRouter tc = goal.AddComponent<TriggerEventRouter>();
            tc.callback = callback;
        }
    }
}
