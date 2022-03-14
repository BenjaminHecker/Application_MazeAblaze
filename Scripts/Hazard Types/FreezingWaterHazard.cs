using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;
using static AssetCatalogue;
using static MazeManager;

public class FreezingWaterHazard : Hazard
{
    [SerializeField] protected float animSpeed = 10;
    
    [Space]
    [SerializeField] protected RuleTile meltedTile1;
    [SerializeField] protected RuleTile meltedTile2;
    [SerializeField] protected RuleTile meltedTile3;
    [SerializeField] protected RuleTile frozenTile;
    [SerializeField] protected RuleTile[] freezeTileAnim;
    [SerializeField] protected RuleTile[] meltTileAnim;

    private IEnumerator freezeRoutine;
    private IEnumerator meltRoutine;
    
    private int counter;
    private bool frozen;
    private bool animRunning;

    private TextMeshProUGUI counterTxt;

    private Color counterColorLow;
    private Color counterColorMed;
    private Color counterColorHigh;

    private bool prevFlameAbove;
    private bool prevStandingBlockAbove;

    private void Awake()
    {
        type = HazardType.FreezingWater;
        prevFlameAbove = false;
        prevStandingBlockAbove = false;
    }

    private void Update()
    {
        bool hasFlameAbove = maze.tiles[mazePos.x, mazePos.y].hasFlame;
        bool hasStandingBlockAbove = maze.tiles[mazePos.x, mazePos.y].HasStandingBlock();

        if (prevFlameAbove && !hasFlameAbove || prevStandingBlockAbove && !hasStandingBlockAbove)
            Unfreeze();

        prevFlameAbove = hasFlameAbove;
        prevStandingBlockAbove = hasStandingBlockAbove;

        if (!frozen && !animRunning)
        {
            Vector3Int tilePos = new Vector3Int(mazePos.x, mazePos.y, 0);
            float freezeProgress = 1 - (float)counter / mazeConfig.blockCounter;

            if (freezeProgress < 1 / 3f)
                hazardTilemap.SetTile(tilePos, meltedTile1);
            else if (freezeProgress < 2 / 3f)
                hazardTilemap.SetTile(tilePos, meltedTile2);
            else
                hazardTilemap.SetTile(tilePos, meltedTile3);
        }
    }

    private void CancelCoroutines()
    {
        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);
        if (meltRoutine != null)
            StopCoroutine(meltRoutine);

        animRunning = false;
    }

    public override void Setup(Cell mazePosition)
    {
        counter = mazeConfig.blockCounter;
        mazePos = mazePosition;
        frozen = true;

        counterTxt = transform.Find("Canvas/Counter").GetComponent<TextMeshProUGUI>();

        counterColorLow = new Color(1, 0.25f, 0.25f);
        counterColorMed = new Color(1, 0.9f, 0.25f);
        counterColorHigh = new Color(0.25f, 1, 0.5f);

        hazardTilemap.SetTile(new Vector3Int(mazePos.x, mazePos.y, 0), frozenTile);

        UpdateCounter(false);
    }

    public override bool IsObstructing()
    {
        return !frozen;
    }

    public override bool IsActive()
    {
        return !frozen;
    }

    private void UpdateCounter(bool active)
    {
        counterTxt.gameObject.SetActive(active);
        counterTxt.text = counter.ToString();

        if (counter >= mazeConfig.blockCounter * 0.5f)
            counterTxt.color = Color.Lerp(counterColorMed, counterColorHigh, (counter / (mazeConfig.blockCounter * 0.5f) - 1));
        else
            counterTxt.color = Color.Lerp(counterColorLow, counterColorMed, counter / (mazeConfig.blockCounter * 0.5f));
    }

    public override void DecrementCounter()
    {
        if (!frozen)
            counter--;

        UpdateCounter(true);

        if (counter == 0)
            Freeze();
        else
            CounterSound();
    }

    public override void Freeze()
    {
        if (!frozen)
        {
            CancelCoroutines();
            freezeRoutine = FreezeCoroutine();
            StartCoroutine(freezeRoutine);

            frozen = true;
            UpdateCounter(false);

            FreezeSound();
        }
    }

    private IEnumerator FreezeCoroutine()
    {
        animRunning = true;
        
        Vector3Int tilePos = new Vector3Int(mazePos.x, mazePos.y, 0);

        if (!frozen)
        {
            for (int i = 0; i < freezeTileAnim.Length; i++)
            {
                hazardTilemap.SetTile(tilePos, freezeTileAnim[i]);
                yield return new WaitForSeconds(1 / animSpeed);
            }
        }

        hazardTilemap.SetTile(tilePos, frozenTile);

        animRunning = false;
    }

    public override void Unfreeze()
    {
        if (!maze.tiles[mazePos.x, mazePos.y].hasFlame && maze.tiles[mazePos.x, mazePos.y].GetStandingBlock() == null)
        {
            CancelCoroutines();
            meltRoutine = UnfreezeCoroutine();
            StartCoroutine(meltRoutine);

            counter = mazeConfig.blockCounter;
            frozen = false;
            UpdateCounter(true);
        }
    }

    private IEnumerator UnfreezeCoroutine()
    {
        animRunning = true;
        
        Vector3Int tilePos = new Vector3Int(mazePos.x, mazePos.y, 0);

        if (frozen)
        {
            for (int i = 0; i < meltTileAnim.Length; i++)
            {
                hazardTilemap.SetTile(tilePos, meltTileAnim[i]);
                yield return new WaitForSeconds(1 / animSpeed);
            }
        }

        hazardTilemap.SetTile(tilePos, meltedTile1);

        animRunning = false;
    }

    public void CounterSound()
    {
        SoundManager.PlayMisc("Radiator 1");
    }

    public void FreezeSound()
    {
        SoundManager.PlayMisc("Mercury Sparkle");
    }
}
