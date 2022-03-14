using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : MonoBehaviour
{
    public Cell mazePos;

    public int X { get { return mazePos.x; } }
    public int Y { get { return mazePos.y; } }

    public void Move(Cell dest)
    {
        mazePos = dest;
        transform.position = new Vector3(mazePos.x, mazePos.y, 0);
        // animate movement
    }

    public void SetSpriteColor(Color col)
    {
        transform.Find("Flame Sprite").GetComponent<SpriteRenderer>().color = col;
    }
}
