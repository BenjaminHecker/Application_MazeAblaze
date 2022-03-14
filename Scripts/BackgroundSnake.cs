using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MainBackgroundManager;

public class BackgroundSnake
{
    private Vector3Int front;
    private Queue<Vector3Int> path;
    private int length;

    private bool readyTurn;
    private int lastDir;

    private enum Direction { N, E, S, W }
    private static int[] DX = { 0, 1, 0, -1 };
    private static int[] DY = { 1, 0, -1, 0 };
    private static Direction[] opposite = { Direction.S, Direction.W, Direction.N, Direction.E };

    public BackgroundSnake(Vector3Int pos, int _length)
    {
        front = pos;
        
        path = new Queue<Vector3Int>();
        path.Enqueue(front);

        length = _length;
        readyTurn = true;
    }

    public void UpdateSnake()
    {
        if (readyTurn)
            front = GetRandomDirection();
        else
            front = new Vector3Int(front.x + DX[lastDir], front.y + DY[lastDir], 0);

        path.Enqueue(front);

        if (path.Count > length)
            path.Dequeue();

        readyTurn = !readyTurn;

        DrawSnake();
    }

    private Vector3Int GetRandomDirection()
    {
        Direction[] directions = { Direction.N, Direction.E, Direction.S, Direction.W };

        ShuffleDirections(ref directions);

        foreach (int dir in directions)
        {
            int mx = front.x + DX[dir];
            int my = front.y + DY[dir];
            int nx = front.x + DX[dir] * 2;
            int ny = front.y + DY[dir] * 2;
            Vector3Int pos = new Vector3Int(mx, my, 0);
            Vector3Int advancePos = new Vector3Int(nx, ny, 0);

            if (!InBounds(mx, my) || path.Contains(pos) || path.Contains(advancePos))
                continue;

            lastDir = dir;
            return pos;
        }

        foreach (int dir in directions)
        {
            int mx = front.x + DX[dir];
            int my = front.y + DY[dir];
            Vector3Int pos = new Vector3Int(mx, my, 0);

            if (!InBounds(mx, my))
                continue;

            lastDir = dir;
            return pos;
        }

        return new Vector3Int();
    }

    private void ShuffleDirections(ref Direction[] directions)
    {
        int n = directions.Length;

        while (n > 1)
        {
            int k = Random.Range(0, n--);
            Direction temp = directions[n];
            directions[n] = directions[k];
            directions[k] = temp;
        }
    }

    private bool InBounds(int x, int y)
    {
        return (x >= 0 && x < Width() && y >= 0 && y < Height());
    }

    private void DrawSnake()
    {
        foreach (Vector3Int pos in path)
        {
            Tilemap tilemap = GetTilemap();
            TileBase pathTile = GetPathTile();
            tilemap.SetTile(pos, pathTile);
        }
    }
}
