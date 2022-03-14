using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RandomMaze;

public static class MazeRandomizer
{
    private static RandomMaze randomConfig;
    private static LevelData mazeConfig;

    private static bool[,] grid;

    private enum Direction { N, E, S, W }
    private static int[] DX = { 0, 1, 0, -1 };
    private static int[] DY = { 1, 0, -1, 0 };
    
    public static LevelData GenerateRandomMaze(RandomMaze _randomConfig)
    {
        randomConfig = _randomConfig;
        mazeConfig = new LevelData(randomConfig);

        grid = new bool[mazeConfig.width, mazeConfig.height];

        GenerateMazeLayout(0, 0);
        ReplaceTiles();
        PlaceFlameTarget();

        return mazeConfig;
    }

    private static void GenerateMazeLayout(int x, int y)
    {
        Direction[] directions = { Direction.N, Direction.E, Direction.S, Direction.W };

        ShuffleDirections(ref directions);

        foreach (int dir in directions)
        {
            int mx = x + DX[dir];
            int my = y + DY[dir];
            int nx = x + DX[dir] * 2;
            int ny = y + DY[dir] * 2;

            if (InBounds(nx, ny) && grid[nx, ny] == false)
            {
                grid[x, y] = true;
                grid[mx, my] = true;
                grid[nx, ny] = true;

                mazeConfig.baseTiles[x, y] = (byte) AssetCatalogue.GroundType.Path;
                mazeConfig.baseTiles[mx, my] = (byte) AssetCatalogue.GroundType.Path;
                mazeConfig.baseTiles[nx, ny] = (byte) AssetCatalogue.GroundType.Path;

                mazeConfig.topTiles[x, y] = 0;
                mazeConfig.topTiles[mx, my] = 0;
                mazeConfig.topTiles[nx, ny] = 0;

                GenerateMazeLayout(nx, ny);
            }
        }
    }

    private static void ReplaceTiles()
    {
        List<Cell> walls = new List<Cell>();

        for (int x = 0; x < mazeConfig.width; x++)
            for (int y = 0; y < mazeConfig.height; y++)
                if (!grid[x, y])
                    walls.Add(new Cell(x, y));

        for (int i = 0; i < randomConfig.replacements.Length && walls.Count > 0; i++)
        {
            Replacement rep = randomConfig.replacements[i];

            for (int j = 0; j < rep.count && walls.Count > 0; j++)
            {
                Cell c = GetRandomCell(ref walls);
                ReplaceWall(c, rep);
            }
        }
    }

    private static void ReplaceWall(Cell c, Replacement rep)
    {
        byte baseTile = 0;
        byte topTile = 0;

        if (rep.defaultType == DefaultType.Block)
            topTile = (byte)rep.block;
        else if (rep.defaultType == DefaultType.Hazard)
            baseTile = (byte)rep.hazard;

        mazeConfig.baseTiles[c.x, c.y] = baseTile;
        mazeConfig.topTiles[c.x, c.y] = topTile;
    }

    private static void PlaceFlameTarget()
    {
        Cell flamePos, targetPos;
        
        List<Cell> corners = new List<Cell> {
            new Cell(0, 0),
            new Cell(mazeConfig.width - 1, mazeConfig.height - 1),
            new Cell(0, mazeConfig.height - 1),
            new Cell(mazeConfig.width - 1, 0)
        };

        List<Cell> path = new List<Cell>();
        for (int x = 0; x < mazeConfig.width; x++)
        {
            for (int y = 0; y < mazeConfig.height; y++)
            {
                if (grid[x, y])
                    path.Add(new Cell(x, y));
            }
        }

        if (randomConfig.flamePlacement == Placement.Center)
        {
            flamePos = new Cell(mazeConfig.width / 2, mazeConfig.height / 2);

            if (randomConfig.targetPlacement == Placement.Center)
                randomConfig.targetPlacement = Placement.Corner;
        }
        else if (randomConfig.flamePlacement == Placement.Corner)
            flamePos = GetRandomCell(ref corners);
        else
            flamePos = GetRandomCell(ref path);

        if (randomConfig.targetPlacement == Placement.Center)
            targetPos = new Cell(mazeConfig.width / 2, mazeConfig.height / 2);
        else if (randomConfig.targetPlacement == Placement.Corner)
            targetPos = GetRandomCell(ref corners);
        else
            targetPos = GetRandomCell(ref path);

        mazeConfig.topTiles[flamePos.x, flamePos.y] = 1;
        mazeConfig.topTiles[targetPos.x, targetPos.y] = 2;
    }

    private static void ShuffleDirections(ref Direction[] directions)
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

    private static bool InBounds(int x, int y)
    {
        return (x >= 0 && x <= mazeConfig.width - 1 && y >= 0 && y <= mazeConfig.height - 1);
    }

    private static Cell GetRandomCell(ref List<Cell> available)
    {
        int n = Random.Range(0, available.Count);

        Cell res = available[n];
        available.RemoveAt(n);

        return res;
    }
}
