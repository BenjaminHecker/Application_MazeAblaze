using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class FreezeBlock : Block
{
    public static int effectiveRadius = 4;

    private void Awake()
    {
        type = BlockType.Freeze;
    }

    public override bool HasPreAbility()
    {
        return status == BlockStatus.Active;
    }

    public override int PreAbilityPriority()
    {
        return 1;
    }

    public override void PreAbility()
    {
        List<Cell> effectTargets = new List<Cell>();

        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                Cell pos = new Cell(x, y);
                
                if (ManhattanDistance(x, y) <= effectiveRadius && !pos.EqualsXY(mazePos))
                    effectTargets.Add(new Cell(x, y));
            }
        }

        foreach (Cell c in effectTargets)
        {
            Block block = maze.tiles[c.x, c.y].GetActiveBlock();

            if (block != null && block.Type != BlockType.Freeze)
                block.Freeze();

            Hazard haz = maze.tiles[c.x, c.y].GetHazard();

            if (haz != null)
                haz.Freeze();
        }
    }

    public override void CrumbleSound()
    {
        SoundManager.PlayMisc("Glass Shatter");
    }
}
