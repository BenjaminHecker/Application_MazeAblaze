using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class HealerBlock : Block
{
    public static int effectiveRadius = 4;

    private void Awake()
    {
        type = BlockType.Healer;
    }

    private void Update()
    {
        anim.SetBool("Lit", HasPreAbility());
    }

    public override bool HasPreAbility()
    {
        return status == BlockStatus.Active && ManhattanDistance(maze.flame.X, maze.flame.Y) == 1;
    }

    public override int PreAbilityPriority()
    {
        return 0;
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

            if (block != null)
                block.HealMax();

            Hazard haz = maze.tiles[c.x, c.y].GetHazard();

            if (haz != null)
                haz.Unfreeze();
        }
    }

    public override void CrumbleSound()
    {
        SoundManager.PlayMisc("Brick Drop");
    }
}
