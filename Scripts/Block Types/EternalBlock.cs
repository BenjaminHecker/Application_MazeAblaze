using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static AssetCatalogue;

public class EternalBlock : Block
{
    private void Awake()
    {
        type = BlockType.Eternal;
    }

    public override void Setup(Cell mazePosition)
    {
        status = BlockStatus.Inactive;
        mazePos = mazePosition;
        frozen = false;

        sRender = transform.Find("Block Sprite").GetComponent<SpriteRenderer>();
        anim = transform.Find("Block Sprite").GetComponent<Animator>();
        mask = transform.Find("Block Sprite").GetComponent<SpriteMask>();
        mask.sprite = sRender.sprite;
    }

    public override void Activate()
    {
        status = BlockStatus.Active;

        anim.SetTrigger("Activate");
    }

    public override void DecrementCounter()
    {
        // do nothing
    }

    public override void Crumble()
    {
        status = BlockStatus.Dead;
        anim.SetTrigger("Crumble");

        IEnumerator crumbleOffsetRoutine = CrumbleOffsetRoutine(0.8f);
        StartCoroutine(crumbleOffsetRoutine);

        CrumbleSound();

        if (!MazeManager.gameOver)
            ScreenShake.TriggerShake(0.1f, 0.05f);
    }

    public override void Freeze()
    {
        // do nothing
    }

    public override void HealMax()
    {
        // do nothing
    }
}
