using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class Hazard : MonoBehaviour
{
    protected HazardType type;
    protected Cell mazePos;

    public HazardType Type { get { return type; } }
    public Cell MazePos { get { return mazePos; } }
    public int X { get { return mazePos.x; } }
    public int Y { get { return mazePos.y; } }

    public virtual void Setup(Cell mazePosition)
    {
        mazePos = mazePosition;

        hazardTilemap.SetTile(new Vector3Int(mazePos.x, mazePos.y, 0), GetHazardTile(type));
    }

    public virtual bool IsObstructing()
    {
        return true;
    }

    public virtual bool IsActive()
    {
        return false;
    }

    public virtual bool HasPreAbility()
    {
        return false;
    }

    public virtual int PreAbilityPriority()
    {
        return 0;
    }

    public virtual void PreAbility()
    {
        // do nothing
    }

    public virtual bool HasPostAbility()
    {
        return false;
    }

    public virtual int PostAbilityPriority()
    {
        return 0;
    }

    public virtual void PostAbility()
    {
        // do nothing
    }

    public virtual void DecrementCounter()
    {
        // do nothing
    }

    public virtual void Freeze()
    {
        // do nothing
    }

    public virtual void Unfreeze()
    {
        // do nothing
    }
}
