using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static MazeManager;

public static class Pathfinder
{
    public static void PathToNearestTarget(out List<Cell> path, Cell src)
    {
        Queue<Cell> q = new Queue<Cell>();                          // stores queue of cells to search

        //an empty List to hold the path. This will be filled if a path to a target is found, and left empty otherwise
        path = new List<Cell>();

        bool[,] visited = new bool[maze.width, maze.height];        // tracks visited cells
        Cell[,] pred = new Cell[maze.width, maze.height];           // stores predecessors

        // resets arrays before pathfinding
        Cell empty = new Cell(-1, -1);
        for (int i = 0; i < maze.width; i++)
        {
            for (int j = 0; j < maze.height; j++)
            {
                visited[i, j] = false;
                pred[i, j] = empty;
            }
        }

        // starts at src
        visited[src.x, src.y] = true;
        q.Enqueue(src);

        bool found = false;
        Cell dest = null;

        // runs until the entire maze has been searched or until target has been found
        while (q.Count != 0 && !found)
        {
            Cell u = q.Dequeue();                       // stores the first cell in the queue

            // iterates through adjacent open cells
            List<Cell> adjacent = Adj(u);
            foreach (Cell c in adjacent)
            {
                int x = c.x;
                int y = c.y;

                // if unvisited, mark it as visited, store its predecessor, and add it to the queue
                if (visited[x, y] == false)
                {
                    visited[x, y] = true;
                    pred[x, y] = u;
                    q.Enqueue(c);

                    // if target is found, break foreach and while loops
                    if (maze.tiles[x, y].IsTarget())
                    {
                        found = true;
                        dest = c;
                        break;
                    }
                }
            }
        }

        // backtracks through the predecessors of dest to create path
        if (found)
        {
            path.Add(dest);

            while (!pred[dest.x, dest.y].EqualsXY(empty))
            {
                path.Add(pred[dest.x, dest.y]);
                dest = pred[dest.x, dest.y];
            }

            // reverse path and remove the first cell, which is the current position of the flame
            path.Reverse();
            path.RemoveAt(0);
        }
    }

    // returns a list of the adjacent open cells
    private static List<Cell> Adj(Cell c)
    {
        List<Cell> neighbors = new List<Cell>();
        int x = c.x;
        int y = c.y;

        if (x > 0 && maze.tiles[x - 1, y].OpenForFire())                   // left
            neighbors.Add(new Cell(x - 1, y));

        if (y > 0 && maze.tiles[x, y - 1].OpenForFire())                   // down
            neighbors.Add(new Cell(x, y - 1));

        if (x < maze.width - 1 && maze.tiles[x + 1, y].OpenForFire())      // right
            neighbors.Add(new Cell(x + 1, y));

        if (y < maze.height - 1 && maze.tiles[x, y + 1].OpenForFire())     // up
            neighbors.Add(new Cell(x, y + 1));

        return neighbors;
    }
}
