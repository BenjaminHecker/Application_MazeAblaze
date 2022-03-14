using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class FreeBlock : Block
{
    private void Awake()
    {
        type = BlockType.Free;
    }

    public override List<Cell> GetDragRegion()
    {
        List<Cell> res = new List<Cell>();

        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                res.Add(new Cell(x, y));
            }
        }

        return res;
    }
}
