using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AssetCatalogue;
using static MazeManager;

public class Maze : ScriptableObject
{
    public int width, height;
    public MazeTile[,] tiles;
    public Flame flame;

    public Maze()
    {

    }

    private void Reset()
    {
        if (groundTilemap != null)
            groundTilemap.ClearAllTiles();

        if (flamePathTilemap != null)
            flamePathTilemap.ClearAllTiles();

        if (hazardTilemap != null)
            hazardTilemap.ClearAllTiles();
        
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Flame"))
            if (child.activeSelf)
                Destroy(child.gameObject);
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Target"))
            if (child.activeSelf)
                Destroy(child.gameObject);
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Block"))
            if (child.activeSelf)
                Destroy(child.gameObject);
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Hazard"))
            if (child.activeSelf)
                Destroy(child.gameObject);
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Indicator"))
            if (child.activeSelf)
                Destroy(child.gameObject);
    }

    public void Generate()
    {
        Reset();

        width = mazeConfig.width;
        height = mazeConfig.height;

        tiles = new MazeTile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new MazeTile();
                Vector3Int pos = new Vector3Int(x, y, 0);

                byte baseTile = mazeConfig.baseTiles[x, y];
                byte topTile = mazeConfig.topTiles[x, y];
                
                if (baseTile <= 4)
                {
                    groundTilemap.SetTile(pos, GetGroundTile((GroundType) baseTile));
                }
                else
                {
                    GameObject newHazard = CreateObject(GetHazardPrefab((HazardType) baseTile), x, y);
                    newHazard.GetComponent<Hazard>().Setup(new Cell(x, y));
                    tiles[x, y].SetHazard(newHazard.GetComponent<Hazard>());
                }

                if (topTile == 0) { }
                else if (topTile == 1)
                {
                    GameObject newFlame = CreateObject(GetFlamePrefab(), x, y);
                    flame = newFlame.GetComponent<Flame>();
                    flame.mazePos = new Cell(x, y);
                    tiles[x, y].hasFlame = true;
                }
                else if (topTile == 2)
                {
                    GameObject newTarget = CreateObject(GetTargetPrefab(), x, y);
                    newTarget.GetComponent<Target>().Setup(new Cell(x, y));
                    tiles[x, y].SetTarget(newTarget.GetComponent<Target>());
                }
                else
                {
                    GameObject newBlock = CreateObject(GetBlockPrefab((BlockType) topTile), x, y);
                    newBlock.GetComponent<Block>().Setup(new Cell(x, y));
                    tiles[x, y].AddBlock(newBlock.GetComponent<Block>());
                }
            }
        }
    }

    private GameObject CreateObject(GameObject go, int x, int y)
    {
        GameObject res = Instantiate(go, new Vector3(x, y), Quaternion.identity, go.transform.parent);
        res.SetActive(true);
        return res;
    }

    public bool FlameAtTarget()
    {
        return tiles[flame.X, flame.Y].IsTarget();
    }

    // move flame to new location
    public void MoveFlame(Cell dest)
    {
        tiles[flame.X, flame.Y].hasFlame = false;
        tiles[dest.x, dest.y].hasFlame = true;
        flame.Move(dest);
    }

    public void DrawPath(ref List<Cell> path)
    {
        TileBase flamePath = GetFlamePathTile();

        flamePathTilemap.ClearAllTiles();

        foreach (Cell c in path)
        {
            flamePathTilemap.SetTile(new Vector3Int(c.x, c.y, 0), flamePath);
        }
    }

    public void TriggerPassive()
    {
        foreach (MazeTile tile in tiles)
            tile.UnfreezeActiveBlock();
        
        TriggerPreAbilities();
        DecrementHazards();
        DecrementBlocks();
        TriggerPostAbilities();
    }

    public void TriggerDestructive()
    {
        CrumbleLowest();
        TriggerPostAbilities();
    }

    private void TriggerPreAbilities()
    {
        SortedDictionary<int, List<Action>> preAbilities = new SortedDictionary<int, List<Action>>();

        foreach (MazeTile tile in tiles)
        {
            foreach (Block block in tile.GetBlocks())
            {
                if (block.HasPreAbility())
                {
                    int priority = block.PreAbilityPriority();

                    if (!preAbilities.ContainsKey(priority))
                        preAbilities[priority] = new List<Action>();

                    preAbilities[priority].Add(block.PreAbility);
                }
            }

            Hazard haz = tile.GetHazard();
            if (haz != null)
            {
                if (haz.HasPreAbility())
                {
                    int priority = haz.PreAbilityPriority();

                    if (!preAbilities.ContainsKey(priority))
                        preAbilities[priority] = new List<Action>();

                    preAbilities[priority].Add(haz.PreAbility);
                }
            }
        }

        foreach (KeyValuePair<int, List<Action>> pair in preAbilities)
        {
            foreach (Action act in pair.Value)
            {
                act();
            }
        }
    }

    private void TriggerPostAbilities()
    {
        SortedDictionary<int, List<Action>> postAbilities = new SortedDictionary<int, List<Action>>();

        foreach (MazeTile tile in tiles)
        {
            foreach (Block block in tile.GetBlocks())
            {
                if (block.HasPostAbility())
                {
                    int priority = block.PostAbilityPriority();

                    if (!postAbilities.ContainsKey(priority))
                        postAbilities[priority] = new List<Action>();

                    postAbilities[priority].Add(block.PostAbility);
                }
            }

            Hazard haz = tile.GetHazard();
            if (haz != null)
            {
                if (haz.HasPostAbility())
                {
                    int priority = haz.PostAbilityPriority();

                    if (!postAbilities.ContainsKey(priority))
                        postAbilities[priority] = new List<Action>();

                    postAbilities[priority].Add(haz.PostAbility);
                }
            }
        }

        foreach (KeyValuePair<int, List<Action>> pair in postAbilities)
        {
            foreach (Action act in pair.Value)
            {
                act();
            }
        }
    }

    // decrement the counter on all active hazards
    private void DecrementHazards()
    {
        foreach (MazeTile tile in tiles)
        {
            tile.DecrementActiveHazard();
        }
    }

    // decrement the counter on all active blocks
    private void DecrementBlocks()
    {
        foreach (MazeTile tile in tiles)
        {
            tile.DecrementActiveBlock();
        }
    }

    // crumble active blocks with the lowest value
    private void CrumbleLowest()
    {
        int minCounter = -1;
        List<Block> minBlocks = new List<Block>();

        foreach (MazeTile tile in tiles)
        {
            Block block = tile.GetActiveBlock();

            if (block == null)
                continue;

            if (block.Type == BlockType.Eternal)
            {
                if (minCounter == -1)
                    minBlocks.Add(block);
            }
            else
            {
                if (minCounter == -1 || block.Counter < minCounter)
                {
                    minCounter = block.Counter;
                    minBlocks.Clear();
                    minBlocks.Add(block);
                }
                else if (block.Counter == minCounter)
                    minBlocks.Add(block);
            }
        }

        foreach (Block block in minBlocks)
            block.Crumble();
    }

    // sets fire to the whole maze in a cascading fashion starting from the center
    public IEnumerator CascadeExplosions()
    {
        Queue<Cell> q = new Queue<Cell>();                                                  // stores tiles to visit next
        Queue<GameObject> cascadeFlames = new Queue<GameObject>();
        bool[,] visited = new bool[width, height];                                          // tracks which tiles have already been visited

        q.Enqueue(flame.mazePos);                                                           // start at flame's current position

        // triggers barrel exploding animation
        flame.gameObject.SetActive(false);
        flamePathTilemap.ClearAllTiles();
        tiles[flame.X, flame.Y].GetTarget().Explode();

        yield return new WaitForSeconds(0.5f);

        // runs until the entire maze is covered
        while (q.Count > 0)
        {
            ScreenShake.TriggerShake(0.2f, 0.3f);
            Queue<Cell> prev = new Queue<Cell>(q);

            // sets fire to current queue of tiles and crumbles any walls in its path
            while (q.Count > 0)
            {
                Cell curr = q.Dequeue();

                cascadeFlames.Enqueue(CreateObject(GetCascadeFlamePrefab(), curr.x, curr.y));

                MazeTile tile = tiles[curr.x, curr.y];
                if (tile.HasStandingBlock())
                {
                    Block block = tile.GetStandingBlock();
                    block.Activate();
                    block.Crumble();
                }
                else if (tile.IsTarget())
                {
                    tile.GetTarget().Explode();
                }

                visited[curr.x, curr.y] = true;                                             // mark these tiles as visited
            }

            yield return new WaitForSeconds(0.1f);

            q = CascadeHelper(prev, ref cascadeFlames, ref visited);                        // get next batch of tiles to explode
        }

        tiles[flame.X, flame.Y].GetTarget().Fall();
    }

    // returns the next batch of adjacent tiles
    private Queue<Cell> CascadeHelper(Queue<Cell> q, ref Queue<GameObject> cascadeFlames, ref bool[,] visited)
    {
        Queue<Cell> res = new Queue<Cell>();

        // removes fire for current queue of tiles while adding unvisited, adjacent tiles to res
        while (q.Count > 0)
        {
            Cell curr = q.Dequeue();

            Destroy(cascadeFlames.Dequeue(), 1f);

            // checks for unvisited adjacent tiles
            List<Cell> adjacent = Adj(curr);
            foreach (Cell c in adjacent)
            {
                if (!visited[c.x, c.y])
                {
                    res.Enqueue(c);
                    visited[c.x, c.y] = true;
                }
            }
        }

        return res;
    }

    // returns adjacent tiles in 4 directions
    private List<Cell> Adj(Cell c)
    {
        List<Cell> neighbors = new List<Cell>();
        int x = c.x;
        int y = c.y;

        if (x > 0)                                  // left
            neighbors.Add(new Cell(x - 1, y));

        if (y > 0)                                  // down
            neighbors.Add(new Cell(x, y - 1));

        if (x < width - 1)                      // right
            neighbors.Add(new Cell(x + 1, y));

        if (y < height - 1)                     // up
            neighbors.Add(new Cell(x, y + 1));

        return neighbors;
    }
}
