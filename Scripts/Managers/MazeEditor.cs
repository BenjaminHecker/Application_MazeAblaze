using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;
using static MazeManager;
using static EditorManager;
using static MazePaletteItem;

public class MazeEditor : MonoBehaviour
{
    public enum PaletteMode { Top, Base }

    [HideInInspector] public static PaletteMode paletteMode;

    public enum EditorMode { Draw, Delete }

    [HideInInspector] public static EditorMode editorMode;

    [HideInInspector] public static MazePaletteItem selectedItem;

    private static GameObject hoverItem;

    private void Update()
    {
        Cell mousePos = new Cell(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        if (hoverItem != null && editorMode == EditorMode.Draw)
        {
            hoverItem.SetActive(InBounds(mousePos) && AvailableTile(mousePos));
            hoverItem.transform.position = new Vector3(mousePos.x, mousePos.y, 0);

            if (hoverItem.activeSelf && Input.GetMouseButton(0))
            {
                PlaceItem(mousePos);
                GeneratePath(out _);
            }
        }

        if (editorMode == EditorMode.Delete)
        {
            if (InBounds(mousePos) && Input.GetMouseButton(0))
            {
                RemoveItem(mousePos);
                GeneratePath(out _);
            }
        }
    }

    public static void Reset()
    {
        paletteMode = PaletteMode.Top;
        editorMode = EditorMode.Draw;
        ResetItem();
    }

    public static void SetItem(MazePaletteItem item)
    {
        editorMode = EditorMode.Draw;
        selectedItem = item;

        UpdateRightPanel();

        Destroy(hoverItem);

        hoverItem = CreateObject(GetDragBlockPrefab(), 0, 0);
        hoverItem.transform.GetComponentInChildren<SpriteRenderer>().sprite = selectedItem.GetSprite();
    }

    public static void UpdateHoverItem()
    {
        if (hoverItem != null)
            hoverItem.transform.GetComponentInChildren<SpriteRenderer>().sprite = selectedItem.GetSprite();
    }

    public static void ResetItem()
    {
        selectedItem = null;

        UpdateRightPanel();

        Destroy(hoverItem);
    }

    private static GameObject CreateObject(GameObject go, int x, int y)
    {
        GameObject res = Instantiate(go, new Vector3(x, y), Quaternion.identity, go.transform.parent);
        res.SetActive(true);
        return res;
    }

    private void PlaceItem(Cell placePos)
    {
        RemoveItem(placePos);

        if (selectedItem.Item == ItemType.Flame)
        {
            mazeConfig.topTiles[maze.flame.mazePos.x, maze.flame.mazePos.y] = 0;
            mazeConfig.topTiles[placePos.x, placePos.y] = 1;

            maze.MoveFlame(placePos);
        }
        else if (selectedItem.Item == ItemType.Target)
        {
            GameObject newTarget = CreateObject(GetTargetPrefab(), placePos.x, placePos.y);
            newTarget.GetComponent<Target>().Setup(placePos);
            maze.tiles[placePos.x, placePos.y].SetTarget(newTarget.GetComponent<Target>());
            
            mazeConfig.topTiles[placePos.x, placePos.y] = 2;
        }
        else if (selectedItem.Item == ItemType.Block)
        {
            GameObject newBlock = CreateObject(GetBlockPrefab(selectedItem.Block), placePos.x, placePos.y);
            newBlock.GetComponent<Block>().Setup(placePos);
            maze.tiles[placePos.x, placePos.y].AddBlock(newBlock.GetComponent<Block>());

            mazeConfig.topTiles[placePos.x, placePos.y] = (byte) selectedItem.Block;
        }
        else if (selectedItem.Item == ItemType.Ground)
        {
            Vector3Int pos = new Vector3Int(placePos.x, placePos.y, 0);
            groundTilemap.SetTile(pos, GetGroundTile(selectedItem.Ground));

            mazeConfig.baseTiles[placePos.x, placePos.y] = (byte) selectedItem.Ground;
        }
        else if (selectedItem.Item == ItemType.Hazard)
        {
            GameObject newHazard = CreateObject(GetHazardPrefab(selectedItem.Hazard), placePos.x, placePos.y);
            newHazard.GetComponent<Hazard>().Setup(placePos);
            maze.tiles[placePos.x, placePos.y].SetHazard(newHazard.GetComponent<Hazard>());

            mazeConfig.baseTiles[placePos.x, placePos.y] = (byte) selectedItem.Hazard;
        }
    }

    public static void RemoveItem(Cell pos)
    {
        if (selectedItem != null && selectedItem.Item != ItemType.Ground)
        {
            Target target = maze.tiles[pos.x, pos.y].GetTarget();

            if (target != null)
            {
                Destroy(target.gameObject);

                mazeConfig.topTiles[pos.x, pos.y] = 0;
            }
            
            Block block = maze.tiles[pos.x, pos.y].GetStandingBlock();

            if (block != null)
            {
                maze.tiles[pos.x, pos.y].RemoveBlock(block);
                Destroy(block.gameObject);

                mazeConfig.topTiles[pos.x, pos.y] = 0;
            }
        }

        Hazard haz = maze.tiles[pos.x, pos.y].GetHazard();

        if (haz != null)
        {
            Destroy(haz.gameObject);

            Vector3Int vPos = new Vector3Int(pos.x, pos.y, 0);
            hazardTilemap.SetTile(vPos, null);
            groundTilemap.SetTile(vPos, GetGroundTile(0));

            mazeConfig.baseTiles[pos.x, pos.y] = 0;
        }

        if (paletteMode == PaletteMode.Base)
        {
            Vector3Int vPos = new Vector3Int(pos.x, pos.y, 0);
            groundTilemap.SetTile(vPos, GetGroundTile(0));

            mazeConfig.baseTiles[pos.x, pos.y] = 0;
        }
    }

    private bool InBounds(Cell mousePos)
    {
        int mazeWidth = maze.width;
        int mazeHeight = maze.height;

        return (mousePos.x >= 0 && mousePos.x < mazeWidth && mousePos.y >= 0 && mousePos.y < mazeHeight);
    }

    private bool AvailableTile(Cell mousePos)
    {
        bool hasFlame = maze.tiles[mousePos.x, mousePos.y].hasFlame;
        bool hasTarget = maze.tiles[mousePos.x, mousePos.y].IsTarget();

        if (selectedItem.Item == ItemType.Target)
        {
            return !hasFlame;
        }
        else if (selectedItem.Item == ItemType.Block)
        {
            return !hasFlame;
        }
        else if (selectedItem.Item == ItemType.Hazard)
        {
            return !hasFlame;
        }

        return true;
    }
}
