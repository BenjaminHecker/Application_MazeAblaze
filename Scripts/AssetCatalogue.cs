using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Tilemaps;
using static MazeManager;

public class AssetCatalogue : MonoBehaviour
{
    private static AssetCatalogue instance;
    
    public enum World : byte
    {
        Default,
        PlainsOfIgnition,
        SmolderingSands,
        FrostBurnHills
    }

    public enum GroundType : byte
    {
        Standard,
        Path,
        Decorative
    }

    public enum HazardType : byte
    {
        Standard = 5,
        FreezingWater
    }

    public enum BlockType : byte
    {
        Standard = 3,
        Double,
        Free,
        Eternal,
        Hourglass,
        Freeze,
        Healer
    }

    [SerializeField] private GameObject flamePrefab;
    [SerializeField] private GameObject cascadeFlamePrefab;
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private GameObject dragBlockPrefab;
    [SerializeField] private GameObject dragHighlightPrefab;
    [SerializeField] private TileBase effectiveRangeTile;
    [SerializeField] private TileBase flamePathTile;

    private Dictionary<World, BackgroundArt> backgroundArt;
    private Dictionary<(World, GroundType), TileBase> groundTiles;
    private Dictionary<(World, HazardType), TileBase> hazardTiles;
    private Dictionary<(World, HazardType), GameObject> hazardPrefabs;
    private Dictionary<(World, BlockType), GameObject> blockPrefabs;

    // -------------------------------------- Allows prefabs to be entered through Inspector. Awake stores these in dictionaries.
    
    [Serializable]
    private struct BackgroundTiles
    {
        public World world;
        public BackgroundArt art;
    }
    [SerializeField] private List<BackgroundTiles> backgroundArtList;

    [Serializable]
    private struct GroundTile
    {
        public World world;
        public GroundType type;
        public TileBase tile;
    }
    [SerializeField] private List<GroundTile> groundTileList;

    [Serializable]
    private struct HazardTile
    {
        public World world;
        public HazardType type;
        public TileBase tile;
    }
    [SerializeField] private List<HazardTile> hazardTileList;

    [Serializable]
    private struct HazardPrefab
    {
        public World world;
        public HazardType type;
        public GameObject go;
    }
    [SerializeField] private List<HazardPrefab> hazardPrefabList;

    [Serializable]
    private struct BlockPrefab {
        public World world;
        public BlockType type;
        public GameObject go;
    }
    [SerializeField] private List<BlockPrefab> blockPrefabList;
    // --------------------------------------------------------------------------------------------------------------------------

    void Awake()
    {
        instance = this;

        backgroundArt = new Dictionary<World, BackgroundArt>();
        groundTiles = new Dictionary<(World, GroundType), TileBase>();
        hazardTiles = new Dictionary<(World, HazardType), TileBase>();
        hazardPrefabs = new Dictionary<(World, HazardType), GameObject>();
        blockPrefabs = new Dictionary<(World, BlockType), GameObject>();

        foreach (BackgroundTiles tile in backgroundArtList)
            backgroundArt[tile.world] = tile.art;
        foreach (GroundTile tile in groundTileList)
            groundTiles[(tile.world, tile.type)] = tile.tile;
        foreach (HazardTile tile in hazardTileList)
            hazardTiles[(tile.world, tile.type)] = tile.tile;
        foreach (HazardPrefab prefab in hazardPrefabList)
            hazardPrefabs[(prefab.world, prefab.type)] = prefab.go;
        foreach (BlockPrefab prefab in blockPrefabList)
            blockPrefabs[(prefab.world, prefab.type)] = prefab.go;
    }

    public static BackgroundArt GetBackgroundArt()
    {
        if (instance.backgroundArt.ContainsKey(mazeConfig.world))
            return instance.backgroundArt[mazeConfig.world];

        return null;
    }

    public static TileBase GetGroundTile(GroundType type)
    {
        if (instance.groundTiles.ContainsKey((mazeConfig.world, type)))
            return instance.groundTiles[(mazeConfig.world, type)];
        
        return instance.groundTiles[(World.Default, type)];
    }

    public static TileBase GetHazardTile(HazardType type)
    {
        if (instance.hazardTiles.ContainsKey((mazeConfig.world, type)))
            return instance.hazardTiles[(mazeConfig.world, type)];

        return instance.hazardTiles[(World.Default, type)];
    }

    public static GameObject GetHazardPrefab(HazardType type)
    {
        if (instance.hazardPrefabs.ContainsKey((mazeConfig.world, type)))
            return instance.hazardPrefabs[(mazeConfig.world, type)];
        
        return instance.hazardPrefabs[(World.Default, type)];
    }

    public static GameObject GetBlockPrefab(BlockType type)
    {
        if (instance.blockPrefabs.ContainsKey((mazeConfig.world, type)))
            return instance.blockPrefabs[(mazeConfig.world, type)];
        
        return instance.blockPrefabs[(World.Default, type)];
    }

    public static GameObject GetFlamePrefab()
    {
        return instance.flamePrefab;
    }

    public static GameObject GetCascadeFlamePrefab()
    {
        return instance.cascadeFlamePrefab;
    }

    public static GameObject GetTargetPrefab()
    {
        return instance.targetPrefab;
    }

    public static GameObject GetDragBlockPrefab()
    {
        return instance.dragBlockPrefab;
    }

    public static GameObject GetDragHighlightPrefab()
    {
        return instance.dragHighlightPrefab;
    }

    public static TileBase GetEffectiveRangeTile()
    {
        return instance.effectiveRangeTile;
    }

    public static TileBase GetFlamePathTile()
    {
        return instance.flamePathTile;
    }

    public static string GetWorldName(World world)
    {
        //return Regex.Replace(world.ToString(), @"(\B[A-Z])", @" $1");

        if (world == World.PlainsOfIgnition)
            return "Plains of Ignition";
        else if (world == World.SmolderingSands)
            return "Smoldering Sands";
        else if (world == World.FrostBurnHills)
            return "Frost Burn Hills";
        else
            return "Default";
    }

    public static Sprite GetGroundSprite(GroundType type)
    {
        TileBase tile;
        
        if (instance.groundTiles.ContainsKey((mazeConfig.world, type)))
            tile = instance.groundTiles[(mazeConfig.world, type)];
        else
            tile = instance.groundTiles[(World.Default, type)];

        if (tile is RuleTile)
            return ((RuleTile) tile).m_DefaultSprite;
        else
            return ((Tile) tile).sprite;
    }

    public static Sprite GetHazardSprite(HazardType type)
    {
        TileBase tile;

        if (instance.hazardTiles.ContainsKey((mazeConfig.world, type)))
            tile = instance.hazardTiles[(mazeConfig.world, type)];
        else
            tile = instance.hazardTiles[(World.Default, type)];

        if (tile is RuleTile)
            return ((RuleTile) tile).m_DefaultSprite;
        else
            return ((Tile) tile).sprite;
    }

    public static Sprite GetBlockSprite(BlockType type)
    {
        if (instance.blockPrefabs.ContainsKey((mazeConfig.world, type)))
            return instance.blockPrefabs[(mazeConfig.world, type)].GetComponent<Block>().GetSprite();

        return instance.blockPrefabs[(World.Default, type)].GetComponent<Block>().GetSprite();
    }
}
