using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class DoubleBlock : Block
{
    private void Awake()
    {
        type = BlockType.Double;
    }

    protected override void UpdateCounter(bool active)
    {
        counterTxt.gameObject.SetActive(active);
        counterTxt.text = counter.ToString();

        if (counter >= mazeConfig.blockCounter)
            counterTxt.color = counterColorHigh;
        else if (counter >= mazeConfig.blockCounter * 0.5f)
            counterTxt.color = Color.Lerp(counterColorMed, counterColorHigh, (counter / (mazeConfig.blockCounter * 0.5f) - 1));
        else
            counterTxt.color = Color.Lerp(counterColorLow, counterColorMed, counter / (mazeConfig.blockCounter * 0.5f));
    }

    public override void Activate()
    {
        status = BlockStatus.Active;
        counter = mazeConfig.blockCounter * 2;

        anim.SetTrigger("Activate");
        UpdateCounter(true);
    }

    public override void HealMax()
    {
        counter = mazeConfig.blockCounter * 2;

        healEffect.TriggerEffect(counterTxt);

        HealSound();
    }
}
