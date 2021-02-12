using System.Collections.Generic;
using UnityEngine;

public class MazeDataGenerator
{
    public float placementTreshold;

    public MazeDataGenerator()
    {
        placementTreshold = .1f;
    }

    public int[,] FromDimensions(int sizeRows, int sizeCols, ref int pointOfGate)
    {
        int[,] maze = new int[sizeRows, sizeCols];

        int rMax = maze.GetUpperBound(0);
        int cMax = maze.GetUpperBound(1);

        // Generating main Maze
        for(int i = 0; i <= rMax; ++i)
        {
            for(int j = 0; j <= cMax; ++j)
            {
                if (i == 0 || j == 0 || i == rMax || j == cMax)
                    maze[i, j] = 1;
                else if (i % 2 == 0 && j % 2 == 0)
                {
                    if(Random.value > placementTreshold)
                    {
                        maze[i, j] = 1;

                        int a = Random.value < .5 ? 0 : (Random.value < .5 ? -1 : 1);
                        int b = a != 0 ? 0 : (Random.value < .5 ? -1 : 1);
                        maze[i + a, j + b] = 1;
                    }
                }
            }
        }

        // Generating start
        while (true)
        {
            int xPoint = (int)(Random.value * sizeRows);
            if (xPoint == 0)
                xPoint = 1;
            else if (xPoint == sizeRows)
                xPoint = sizeRows - 1;

            maze[xPoint, 0] = 2;

            if (maze[xPoint, 1] == 0)
            {
                pointOfGate = xPoint;
                break;
            }
            else
                maze[xPoint, 0] = 1;
        }

        while (true)
        {
            int yPoint = (int)(Random.value * sizeCols);
            if (yPoint == 0)
                yPoint = 1;
            else if (yPoint == sizeCols)
                yPoint = sizeCols - 1;

            maze[0, yPoint] = 3;

            if (maze[1, yPoint] == 0)
                break;
            else
                maze[0, yPoint] = 1;
        }

        while (true)
        {
            int yPoint = (int)(Random.value * sizeCols);
            if (yPoint == 0)
                yPoint = 1;
            else if (yPoint == sizeCols)
                yPoint = sizeCols - 1;

            maze[sizeRows - 1, yPoint] = 3;

            if (maze[sizeRows - 2, yPoint] == 0)
                break;
            else
                maze[sizeRows - 1, yPoint] = 1;
        }

        while (true)
        {
            int xPoint = (int)(Random.value * sizeRows);
            if (xPoint == 0)
                xPoint = 1;
            else if (xPoint == sizeRows)
                xPoint = sizeRows - 1;

            maze[xPoint, sizeCols - 1] = 3;

            if (maze[xPoint, sizeCols - 2] == 0)
            {
                pointOfGate = xPoint;
                break;
            }
            else
                maze[xPoint, sizeCols - 1] = 1;
        }

        return maze;
    }
}
