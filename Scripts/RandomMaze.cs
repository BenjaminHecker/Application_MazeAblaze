using UnityEngine;
using static AssetCatalogue;
using static LevelData;

[System.Serializable]
public class RandomMaze
{
    public enum Placement { Center, Corner, Random }
    public enum DefaultType { None, Block, Hazard }

    [System.Serializable]
    public struct Replacement
    {
        public DefaultType defaultType;
        public BlockType block;
        public HazardType hazard;
        public int count;
    }

    [Header("Generic Level Data")]
    public string name;
    public string description;

    public World world;
    public int blockCounter = DefaultBlockCounter();
    public float fireTick = DefaultFireTick();

    public int width = 13;
    public int height = 13;

    public Goals goals;

    [Header("Random Settings")]
    public Placement flamePlacement;
    public Placement targetPlacement;

    [Space]
    public DefaultType defaultType = DefaultType.Block;
    public BlockType block = BlockType.Standard;
    public HazardType hazard = HazardType.Standard;

    [Space]
    public Replacement[] replacements;
}
