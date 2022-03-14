using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    private Cell mazePos;

    private Animator anim;

    public Cell MazePos { get { return mazePos; } }
    public int X { get { return mazePos.x; } }
    public int Y { get { return mazePos.y; } }

    public void Setup(Cell mazePosition)
    {
        mazePos = mazePosition;

        anim = transform.Find("Target Sprite").GetComponent<Animator>();
    }

    public void SetSpriteColor(Color col)
    {
        transform.Find("Target Sprite").GetComponent<SpriteRenderer>().color = col;
    }

    public void Explode()
    {
        anim.SetTrigger("Explode");
    }

    public void Fall()
    {
        anim.SetTrigger("Fall");
    }
}
