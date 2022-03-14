using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;
using static Block;

public class DragDrop : MonoBehaviour
{
    private bool isDragged;
    private Block originalBlock;
    private GameObject dragBlock;

    private List<Cell> dragRegion;
    private Dictionary<Cell, DragHighlight> dragHighlights;

    private void Awake()
    {
        isDragged = false;
        originalBlock = GetComponent<Block>();

        dragRegion = new List<Cell>();
        dragHighlights = new Dictionary<Cell, DragHighlight>();
    }

    private void Update()
    {
        if (isDragged)
        {
            foreach (Cell c in dragRegion)
                dragHighlights[c].SetActive(maze.tiles[c.x, c.y].OpenForBlock());

            UpdateEffectiveRange();
        }

        if (isDragged && gameOver)
            DropBlock();
    }

    private void OnMouseDown()
    {
        if (!gameOver && originalBlock.Status == BlockStatus.Inactive)
        {
            isDragged = true;
            originalBlock.SetSpriteColor(new Color(1, 1, 1, 0.5f));
            
            dragBlock = CreateObject(GetDragBlockPrefab(), originalBlock.X, originalBlock.Y);
            dragBlock.transform.GetComponentInChildren<SpriteRenderer>().sprite = originalBlock.GetSprite();

            dragRegion = originalBlock.GetDragRegion();

            foreach (Cell c in dragRegion)
            {
                dragHighlights[c] = CreateObject(GetDragHighlightPrefab(), c.x, c.y).GetComponent<DragHighlight>();
                dragHighlights[c].Setup(new Cell(c.x, c.y));
            }

            originalBlock.GrabSound();
        }
    }

    private void OnMouseDrag()
    {
        if (isDragged)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector3 newPos = NearestAvailableTile(mousePos);

            if (dragBlock.transform.position != newPos)
                originalBlock.DragSound();

            dragBlock.transform.position = newPos;
        }
    }

    private void OnMouseUp()
    {
        if (isDragged)
        {
            DropBlock();
            dragRegionTilemap.ClearAllTiles();
        }
    }

    private void DropBlock()
    {
        isDragged = false;
        originalBlock.SetSpriteColor(new Color(1, 1, 1));
        Cell dropPos = new Cell(dragBlock.transform.position);

        if (!dropPos.EqualsXY(originalBlock.MazePos))
        {
            maze.tiles[originalBlock.X, originalBlock.Y].RemoveBlock(originalBlock);
            maze.tiles[dropPos.x, dropPos.y].AddBlock(originalBlock);

            originalBlock.Move(dropPos);
            originalBlock.Activate();

            if (originalBlock.HasPreAbility())
                originalBlock.PreAbility();

            originalBlock.DropSound();
        }

        Destroy(dragBlock);

        foreach (var item in dragHighlights)
            Destroy(item.Value.gameObject);

        dragHighlights.Clear();
    }

    private GameObject CreateObject(GameObject go, int x, int y)
    {
        GameObject res = Instantiate(go, new Vector3(x, y), Quaternion.identity, go.transform.parent);
        res.SetActive(true);
        return res;
    }

    private Vector3 NearestAvailableTile(Vector2 mousePos)
    {
        Cell mouseCell = new Cell(mousePos);

        if (InBounds(mousePos))
        {
            if (mouseCell.EqualsXY(originalBlock.MazePos) || dragRegion.Contains(mouseCell))
                return mouseCell.CellToVector2();
        }

        Vector3 res = originalBlock.transform.position;
        float minDist = Mathf.Infinity;

        foreach (Cell c in dragRegion)
        {
            if (maze.tiles[c.x, c.y].OpenForBlock())
            {
                Vector2 cellCenter = new Vector2(c.x + 0.5f, c.y + 0.5f);

                Vector2 delta = cellCenter - mousePos;
                float distance = delta.sqrMagnitude;

                if (distance < minDist)
                {
                    minDist = distance;
                    res = c.CellToVector2();
                }
            }
        }

        return res;
    }

    private bool InBounds(Vector2 mousePos)
    {
        int mazeWidth = maze.width;
        int mazeHeight = maze.height;
        
        return (mousePos.x >= 0 && mousePos.x < mazeWidth && mousePos.y >= 0 && mousePos.y < mazeHeight);
    }

    private void UpdateEffectiveRange()
    {
        dragRegionTilemap.ClearAllTiles();

        int effectiveRange;

        if (originalBlock == null)
            return;
        else if (originalBlock.Type == BlockType.Hourglass)
            effectiveRange = HourglassBlock.effectiveRadius;
        else if (originalBlock.Type == BlockType.Healer)
            effectiveRange = HealerBlock.effectiveRadius;
        else if (originalBlock.Type == BlockType.Freeze)
            effectiveRange = FreezeBlock.effectiveRadius;
        else
            return;

        Cell dragPos = new Cell(dragBlock.transform.position);

        for (int x = 0; x < maze.width; x++)
        {
            for (int y = 0; y < maze.height; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                int manDist = ManhattanDistance(dragPos.x, dragPos.y, x, y);

                if (manDist <= effectiveRange)
                    dragRegionTilemap.SetTile(pos, GetEffectiveRangeTile());
            }
        }
    }

    private int ManhattanDistance(int srcX, int srcY, int destX, int destY)
    {
        return Mathf.Abs(destX - srcX) + Mathf.Abs(destY - srcY);
    }
}
