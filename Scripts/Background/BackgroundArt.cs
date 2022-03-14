using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AssetCatalogue;
using static MazeManager;

[CreateAssetMenu]
public class BackgroundArt : ScriptableObject
{
    [SerializeField] private TileBase floor;
    [SerializeField] private BackgroundRegion border;
    [SerializeField] private List<BackgroundRegionItem> outerRegions;
    [SerializeField] private List<BackgroundRegionItem> leftRegions;
    [SerializeField] private List<BackgroundRegionItem> rightRegions;

    [Serializable]
    private struct BackgroundRegionItem
    {
        public BackgroundRegion region;
        public int width;
    }

    private int width;
    private int height;
    private float camHeight;
    private float camWidth;
    private int heightPadding;
    private int widthPadding;

    private List<BackgroundTile> animatedTiles = new List<BackgroundTile>();
    private List<Vector3Int> occupiedTiles = new List<Vector3Int>();

    public void Generate()
    {
        width = mazeConfig.width;
        height = mazeConfig.height;

        camHeight = Camera.main.orthographicSize;
        camWidth = camHeight * 16 / 9;
        heightPadding = Mathf.Max(2, Mathf.RoundToInt((camHeight * 2 + 1 - height) / 2));
        widthPadding = Mathf.Max(5, Mathf.RoundToInt((camWidth * 2 + 1 - width) / 2));

        backgroundTilemap.ClearAllTiles();
        animatedTiles.Clear();
        occupiedTiles.Clear();

        border.Reset();
        foreach (BackgroundRegionItem item in outerRegions)
            item.region.Reset();
        foreach (BackgroundRegionItem item in leftRegions)
            item.region.Reset();
        foreach (BackgroundRegionItem item in rightRegions)
            item.region.Reset();

        if (mazeConfig.world == World.PlainsOfIgnition)
            Generate1();
        else if (mazeConfig.world == World.SmolderingSands)
            Generate2();
        else if (mazeConfig.world == World.FrostBurnHills)
            Generate3();

        backgroundTilemap.RefreshAllTiles();
    }

    public void Dance()
    {
        foreach (BackgroundTile tile in animatedTiles)
            tile.Dance();
    }

    private void AddAnimTile(BackgroundTile tile)
    {
        if (!animatedTiles.Contains(tile))
            animatedTiles.Add(tile);
    }

    public void AddOccupiedTiles(List<Vector3Int> positions)
    {
        foreach (Vector3Int pos in positions)
            occupiedTiles.Add(pos);
    }

    private void Generate1()
    {
        for (int x = -widthPadding; x < width + widthPadding; x++)
        {
            for (int y = -heightPadding; y < height + heightPadding; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                BackgroundTile backgroundTile = null;

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    backgroundTilemap.SetTile(pos, floor);
                    continue;
                }
                else if (x >= -1 && x < width + 1)
                {
                    backgroundTile = border.GetBackgroundTile(pos, 1, ref occupiedTiles);
                    AddOccupiedTiles(backgroundTile.SetTile(pos));
                }
                else
                {
                    int prevCumulativeWidth;
                    int cumulativeWidth = 1;

                    for (int i = 0; i < outerRegions.Count; i++)
                    {
                        prevCumulativeWidth = cumulativeWidth;
                        cumulativeWidth += (i == outerRegions.Count - 1) ? widthPadding - cumulativeWidth : outerRegions[i].width;

                        int horizontalSpace = (x <= width / 2) ? x + cumulativeWidth + 1 : x - width - prevCumulativeWidth + 1;

                        if (x >= -cumulativeWidth && x < width + cumulativeWidth)
                        {
                            backgroundTile = outerRegions[i].region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                            AddOccupiedTiles(backgroundTile.SetTile(pos));
                            break;
                        }
                    }
                }

                if (backgroundTile && backgroundTile.Type == BackgroundTile.TileType.Animated)
                    AddAnimTile(backgroundTile);
            }
        }
    }

    private void Generate2()
    {
        for (int x = -widthPadding; x < width + widthPadding; x++)
        {
            for (int y = -heightPadding; y < height + heightPadding; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                BackgroundTile backgroundTile = null;

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    backgroundTilemap.SetTile(pos, floor);
                    continue;
                }
                else if (x >= -1 && x < width + 1)
                {
                    backgroundTile = border.GetBackgroundTile(pos, 1, ref occupiedTiles);
                    AddOccupiedTiles(backgroundTile.SetTile(pos));
                }
                else
                {
                    if (x <= width / 2)
                    {
                        int prevCumulativeWidth;
                        int cumulativeWidth = 1;

                        for (int i = 0; i < leftRegions.Count; i++)
                        {
                            prevCumulativeWidth = cumulativeWidth;
                            cumulativeWidth += (i >= leftRegions.Count - 2) ? widthPadding - cumulativeWidth : leftRegions[i].width;

                            int horizontalSpace = x + cumulativeWidth + 1;
                            int innerDist = x + prevCumulativeWidth;

                            if (i >= leftRegions.Count - 2)
                            {
                                int regionCount = leftRegions.Count;
                                BackgroundRegion region = (innerDist % 2 == 0) ? leftRegions[regionCount - 1].region : leftRegions[regionCount - 2].region;

                                backgroundTile = region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                                AddOccupiedTiles(backgroundTile.SetTile(pos));
                                break;
                            }
                            else if (x >= -cumulativeWidth && x < width + cumulativeWidth)
                            {
                                backgroundTile = leftRegions[i].region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                                AddOccupiedTiles(backgroundTile.SetTile(pos));
                                break;
                            }
                        }
                    }
                    else
                    {
                        int prevCumulativeWidth;
                        int cumulativeWidth = 1;

                        for (int i = 0; i < rightRegions.Count; i++)
                        {
                            prevCumulativeWidth = cumulativeWidth;
                            cumulativeWidth += (i >= rightRegions.Count - 2) ? widthPadding - cumulativeWidth : rightRegions[i].width;

                            int horizontalSpace = x - width - prevCumulativeWidth + 1;

                            if (i >= rightRegions.Count - 2)
                            {
                                int regionCount = rightRegions.Count;
                                BackgroundRegion region = (horizontalSpace % 2 == 0) ? rightRegions[regionCount - 1].region : rightRegions[regionCount - 2].region;

                                backgroundTile = region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                                AddOccupiedTiles(backgroundTile.SetTile(pos));
                                break;
                            }
                            else if (x >= -cumulativeWidth && x < width + cumulativeWidth)
                            {
                                backgroundTile = rightRegions[i].region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                                AddOccupiedTiles(backgroundTile.SetTile(pos));
                                break;
                            }
                        }
                    }
                }

                if (backgroundTile && backgroundTile.Type == BackgroundTile.TileType.Animated)
                    AddAnimTile(backgroundTile);
            }
        }
    }

    private void Generate3()
    {
        for (int x = -widthPadding; x < width + widthPadding; x++)
        {
            for (int y = -heightPadding; y < height + heightPadding; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                BackgroundTile backgroundTile = null;

                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    backgroundTilemap.SetTile(pos, floor);
                    continue;
                }
                else if (x >= -1 && x < width + 1)
                {
                    backgroundTile = border.GetBackgroundTile(pos, 1, ref occupiedTiles);
                    AddOccupiedTiles(backgroundTile.SetTile(pos));
                }
                else
                {
                    int prevCumulativeWidth;
                    int cumulativeWidth = 1;

                    for (int i = 0; i < outerRegions.Count; i++)
                    {
                        prevCumulativeWidth = cumulativeWidth;
                        cumulativeWidth += (i == outerRegions.Count - 1) ? widthPadding - cumulativeWidth : outerRegions[i].width;

                        int horizontalSpace = (x <= width / 2) ? x + cumulativeWidth + 1 : x - width - prevCumulativeWidth + 1;

                        if (x >= -cumulativeWidth && x < width + cumulativeWidth)
                        {
                            backgroundTile = outerRegions[i].region.GetBackgroundTile(pos, horizontalSpace, ref occupiedTiles);
                            AddOccupiedTiles(backgroundTile.SetTile(pos));
                            break;
                        }
                    }
                }

                if (backgroundTile && backgroundTile.Type == BackgroundTile.TileType.Animated)
                    AddAnimTile(backgroundTile);
            }
        }
    }
}
