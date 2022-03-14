using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class BackgroundRegion : ScriptableObject
{
    [System.Serializable]
    private struct RegionItem
    {
        [Range(0f, 1f)] public float probability;
        public BackgroundTile tile;
    }

    [SerializeField] private List<RegionItem> items;

    public void Reset()
    {
        foreach (RegionItem item in items)
            item.tile.Reset();
    }

    public BackgroundTile GetBackgroundTile(Vector3Int topRight, int horizontalSpace, ref List<Vector3Int> occupiedTiles)
    {
        while (true)
        {
            BackgroundTile tile = GetRandomTile();

            if (tile.Type == BackgroundTile.TileType.Structure)
            {
                if (tile.Width <= horizontalSpace)
                {
                    List<Vector3Int> structurePos = tile.GetStructureTiles(topRight);
                    bool isObstructed = false;

                    foreach (Vector3Int pos in structurePos)
                        if (occupiedTiles.Contains(pos))
                            isObstructed = true;

                    if (!isObstructed)
                        return tile;
                }
            }
            else
                return tile;
        }
    }

    private BackgroundTile GetRandomTile()
    {
        float rand = Random.Range(0f, 1f);
        float cumulativeProb = 0;

        foreach (RegionItem item in items)
        {
            cumulativeProb += item.probability;

            if (rand <= cumulativeProb)
                return item.tile;
        }

        return null;
    }
}
