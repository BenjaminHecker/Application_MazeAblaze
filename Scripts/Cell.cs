using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public int x, y;

    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Cell(Vector2 v)
    {
        x = (int) v.x;
        y = (int) v.y;
    }

    public bool EqualsXY(Cell c)
    {
        return x == c.x && y == c.y;
    }

    public Vector2 CellToVector2()
    {
        return new Vector2(x, y);
    }
}
