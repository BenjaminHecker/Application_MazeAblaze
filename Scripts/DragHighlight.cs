using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;

public class DragHighlight : MonoBehaviour
{
    private enum StatusEffects
    {
        None,
        Heal,
        Freeze,
        HealFreeze
    }

    [Serializable]
    private struct DragHighlightTile
    {
        public StatusEffects effects;
        public Sprite tile;
        public Sprite icon;
    }

    [SerializeField] [Range(0f, 1f)] private float inactiveAlpha;
    [SerializeField] private Pulsing tilePulse;

    [SerializeField] private SpriteRenderer tileRender;
    [SerializeField] private SpriteRenderer iconRender;

    [SerializeField] private List<DragHighlightTile> dragHighlightTileList;

    private Dictionary<StatusEffects, Sprite> dragHighlightTiles;
    private Dictionary<StatusEffects, Sprite> dragHighlightIcons;

    private Cell mazePos;
    
    void Awake()
    {
        dragHighlightTiles = new Dictionary<StatusEffects, Sprite>();
        dragHighlightIcons = new Dictionary<StatusEffects, Sprite>();

        foreach (DragHighlightTile x in dragHighlightTileList)
        {
            dragHighlightTiles[x.effects] = x.tile;
            dragHighlightIcons[x.effects] = x.icon;
        }
    }

    void Update()
    {
        bool heal = false;
        bool freeze = false;

        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                int manDist = ManhattanDistance(x, y);
                Block activeBlock = maze.tiles[x, y].GetActiveBlock();

                if (activeBlock == null)
                    continue;

                if (manDist <= HourglassBlock.effectiveRadius && activeBlock.Type == BlockType.Hourglass)
                    heal = true;
                if (manDist <= HealerBlock.effectiveRadius && activeBlock.Type == BlockType.Healer)
                    heal = true;
                if (manDist <= FreezeBlock.effectiveRadius && activeBlock.Type == BlockType.Freeze)
                    freeze = true;
            }
        }

        UpdateSprites(heal, freeze);
    }

    public void Setup(Cell mazePosition)
    {
        mazePos = mazePosition;
    }

    public void SetActive(bool active)
    {
        if (active)
        {
            tilePulse.SetAlpha(1);
            iconRender.color = new Color(1, 1, 1, 1);
        }
        else
        {
            tilePulse.SetAlpha(inactiveAlpha);
            iconRender.color = new Color(1, 1, 1, inactiveAlpha);
        }
    }

    private int ManhattanDistance(int destX, int destY)
    {
        return Mathf.Abs(destX - mazePos.x) + Mathf.Abs(destY - mazePos.y);
    }

    private void UpdateSprites(bool heal, bool freeze)
    {
        StatusEffects effects;

        if (heal && freeze)
            effects = StatusEffects.HealFreeze;
        else if (heal)
            effects = StatusEffects.Heal;
        else if (freeze)
            effects = StatusEffects.Freeze;
        else
            effects = StatusEffects.None;

        tileRender.sprite = dragHighlightTiles[effects];
        iconRender.sprite = dragHighlightIcons[effects];
    }
}
