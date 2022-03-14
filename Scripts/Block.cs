using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;
using static AssetCatalogue;
using static MazeManager;

public abstract class Block : MonoBehaviour
{
    public enum BlockStatus { Inactive, Active, Dead };

    protected BlockType type;
    protected BlockStatus status;
    protected int counter;
    protected Cell mazePos;
    protected bool frozen;

    [SerializeField] protected int dragRegionRadius;

    protected SpriteRenderer sRender;
    protected Animator anim;
    protected SpriteMask mask;
    protected TextMeshProUGUI counterTxt;

    protected StatusEffect healEffect;
    protected StatusEffect freezeEffect;

    protected Color counterColorLow;
    protected Color counterColorMed;
    protected Color counterColorHigh;

    public BlockType Type { get { return type; } }
    public BlockStatus Status { get { return status; } }
    public int Counter { get { return counter; } }
    public Cell MazePos { get { return mazePos; } }
    public int X { get { return mazePos.x; } }
    public int Y { get { return mazePos.y; } }

    private void Update()
    {
        if (mask != null && sRender != null)
            mask.sprite = sRender.sprite;
    }

    public virtual void Setup(Cell mazePosition)
    {
        status = BlockStatus.Inactive;
        counter = mazeConfig.blockCounter;
        mazePos = mazePosition;
        frozen = false;

        sRender = transform.Find("Block Sprite").GetComponent<SpriteRenderer>();
        anim = transform.Find("Block Sprite").GetComponent<Animator>();
        mask = transform.Find("Block Sprite").GetComponent<SpriteMask>();
        counterTxt = transform.Find("Canvas/Counter").GetComponent<TextMeshProUGUI>();
        mask.sprite = sRender.sprite;

        healEffect = transform.Find("Block Sprite/Heal Effect").GetComponent<StatusEffect>();
        freezeEffect = transform.Find("Block Sprite/Freeze Effect").GetComponent<StatusEffect>();

        counterColorLow = new Color(1, 0.25f, 0.25f);
        counterColorMed = new Color(1, 0.9f, 0.25f);
        counterColorHigh = new Color(0.25f, 1, 0.5f);

        UpdateCounter(false);
    }

    public void Move(Cell dest)
    {
        mazePos = dest;
        transform.position = new Vector3(mazePos.x, mazePos.y, 0);
    }

    public void SetSpriteColor(Color col)
    {
        sRender.color = col;
    }

    public bool IsStanding()
    {
        return status == BlockStatus.Inactive || status == BlockStatus.Active;
    }

    public bool IsActive()
    {
        return status == BlockStatus.Active;
    }

    public virtual void Activate()
    {
        status = BlockStatus.Active;
        counter = mazeConfig.blockCounter;

        anim.SetTrigger("Activate");
        UpdateCounter(true);
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

    protected virtual void UpdateCounter(bool active)
    {
        counterTxt.gameObject.SetActive(active);
        counterTxt.text = counter.ToString();

        if (counter >= mazeConfig.blockCounter * 0.5f)
            counterTxt.color = Color.Lerp(counterColorMed, counterColorHigh, (counter / (mazeConfig.blockCounter * 0.5f) - 1));
        else
            counterTxt.color = Color.Lerp(counterColorLow, counterColorMed, counter / (mazeConfig.blockCounter * 0.5f));
    }

    public virtual void DecrementCounter()
    {
        if (!frozen)
            counter--;

        UpdateCounter(true);

        if (counter == 0)
            Crumble();
        else
            CounterSound();
    }

    public virtual void Crumble()
    {
        status = BlockStatus.Dead;
        frozen = false;
        anim.SetTrigger("Crumble");
        UpdateCounter(false);

        IEnumerator crumbleOffsetRoutine = CrumbleOffsetRoutine(0.6f);
        StartCoroutine(crumbleOffsetRoutine);

        CrumbleSound();

        if (!gameOver)
            ScreenShake.TriggerShake(0.1f, 0.05f);
    }

    protected IEnumerator CrumbleOffsetRoutine(float offsetDelay)
    {
        Vector3 origPos = transform.position;
        Vector3 offset = new Vector3((int)Random.Range(-2, 2), (int)Random.Range(-2, 2), 0);
        
        yield return new WaitForSeconds(offsetDelay);

        GetComponent<SortingGroup>().sortingLayerName = "Debris";

        transform.position = origPos + offset / 16f;
    }

    public virtual List<Cell> GetDragRegion()
    {
        List<Cell> res = new List<Cell>();

        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                if (ManhattanDistance(x, y) <= dragRegionRadius)
                    res.Add(new Cell(x, y));
            }
        }

        return res;
    }

    protected int ManhattanDistance(int destX, int destY)
    {
        return Mathf.Abs(destX - mazePos.x) + Mathf.Abs(destY - mazePos.y);
    }

    public Sprite GetSprite()
    {
        return transform.Find("Block Sprite").GetComponent<SpriteRenderer>().sprite;
    }

    public virtual void Freeze()
    {
        if (!frozen)
        {
            frozen = true;
            SetSpriteColor(new Color(140f / 255, 241f / 255, 245f / 255));

            freezeEffect.TriggerEffect(counterTxt);

            FreezeSound();
        }
    }

    public virtual void CheckUnfreeze()
    {
        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                if (ManhattanDistance(x, y) <= FreezeBlock.effectiveRadius)
                {
                    Block block = maze.tiles[x, y].GetActiveBlock();
                    
                    if (block != null && block.Type == BlockType.Freeze)
                        return;
                }
            }
        }

        Unfreeze();
    }

    public virtual void Unfreeze()
    {
        frozen = false;
        SetSpriteColor(new Color(1, 1, 1));
    }

    public virtual void HealMax()
    {
        counter = mazeConfig.blockCounter;

        healEffect.TriggerEffect(counterTxt);

        HealSound();
    }

    public virtual void CounterSound()
    {
        SoundManager.PlayMisc("Radiator 1");
    }

    public virtual void GrabSound()
    {
        SoundManager.PlayMisc("Thump");
    }

    public virtual void DragSound()
    {
        SoundManager.PlayMisc("Punching Cushion");
    }

    public virtual void DropSound()
    {
        SoundManager.PlayMisc("Grassy Thud");
    }

    public virtual void CrumbleSound()
    {
        SoundManager.PlayMisc("Rock Falls");
    }

    public virtual void HealSound()
    {
        SoundManager.PlayMisc("Menu Scroll");
    }

    public virtual void FreezeSound()
    {
        SoundManager.PlayMisc("Mercury Sparkle");
    }
}
