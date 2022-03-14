using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static AssetCatalogue;
using static MazeManager;

public class HourglassBlock : Block
{
    public static int effectiveRadius = 4;

    private bool readyPostAbility;
    
    private void Awake()
    {
        type = BlockType.Hourglass;
        readyPostAbility = false;
    }

    private void Update()
    {
        float bloom = 1 - (float)counter / (mazeConfig.blockCounter / 2);
        anim.SetFloat("Bloom", bloom);
    }

    protected override void UpdateCounter(bool active)
    {
        counterTxt.gameObject.SetActive(active);
        counterTxt.text = counter.ToString();

        counterTxt.color = Color.Lerp(counterColorLow, counterColorMed, counter / (mazeConfig.blockCounter * 0.5f));
    }

    public override bool HasPostAbility()
    {
        return readyPostAbility;
    }

    public override int PostAbilityPriority()
    {
        return 0;
    }

    public override void PostAbility()
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
        }

        readyPostAbility = false;
    }

    public override void Activate()
    {
        status = BlockStatus.Active;
        counter = mazeConfig.blockCounter / 2;

        anim.SetTrigger("Activate");
        UpdateCounter(true);
    }

    public override void DecrementCounter()
    {
        base.DecrementCounter();

        if (counter == 0)
            readyPostAbility = true;
    }

    public override void Crumble()
    {
        status = BlockStatus.Dead;
        frozen = false;
        anim.SetTrigger("Crumble");
        UpdateCounter(false);

        IEnumerator crumbleOffsetRoutine = CrumbleOffsetRoutine(0.6f);
        StartCoroutine(crumbleOffsetRoutine);

        if (!HasPostAbility())
            CrumbleSound();

        if (!gameOver)
            ScreenShake.TriggerShake(0.1f, 0.05f);
    }

    public override void HealMax()
    {
        counter = mazeConfig.blockCounter / 2;

        healEffect.TriggerEffect(counterTxt);

        HealSound();
    }
}
