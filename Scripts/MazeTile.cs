using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeTile
{
    private List<Block> blocks;
    private Hazard hazard;
    private Target target;
    public bool hasFlame;

    public MazeTile()
    {
        blocks = new List<Block>();
        hasFlame = false;
    }

    public void AddBlock(Block block)
    {
        blocks.Add(block);
    }

    public void RemoveBlock(Block block)
    {
        blocks.Remove(block);
    }

    public void SetHazard(Hazard haz)
    {
        hazard = haz;
    }

    public void SetTarget(Target tar)
    {
        target = tar;
    }

    public bool OpenForBlock()
    {
        return !(hasFlame || IsTarget() || HasObstructingHazard() || HasStandingBlock());
    }

    public bool OpenForFire()
    {
        return !(HasObstructingHazard() || HasStandingBlock());
    }

    public bool HasBlock()
    {
        return blocks.Count > 0;
    }

    public bool HasStandingBlock()
    {
        foreach (Block block in blocks)
        {
            if (block.IsStanding())
                return true;
        }
        return false;
    }

    public bool HasObstructingHazard()
    {
        if (hazard == null)
            return false;
        else
            return hazard.IsObstructing();
    }

    public Hazard GetHazard()
    {
        return hazard;
    }

    public bool IsTarget()
    {
        return (target != null);
    }

    public Target GetTarget()
    {
        return target;
    }

    public List<Block> GetBlocks()
    {
        return blocks;
    }

    public Block GetStandingBlock()
    {
        foreach (Block block in blocks)
        {
            if (block.IsStanding())
                return block;
        }
        return null;
    }

    public Block GetActiveBlock()
    {
        foreach (Block block in blocks)
        {
            if (block.IsActive())
            {
                return block;
            }
        }
        return null;
    }

    public void UnfreezeActiveBlock()
    {
        Block block = GetActiveBlock();

        if (block != null)
            block.CheckUnfreeze();
    }

    public void DecrementActiveHazard()
    {
        if (hazard != null && hazard.IsActive())
            hazard.DecrementCounter();
    }

    public void DecrementActiveBlock()
    {
        Block block = GetActiveBlock();

        if (block != null)
            block.DecrementCounter();
    }
}
