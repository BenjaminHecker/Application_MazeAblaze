using static AssetCatalogue;

[System.Serializable]
public class LevelData
{
    public string name;
    public string description;
    
    public World world;
    public int blockCounter;
    public float fireTick;
    
    public int width, height;
    public byte[,] baseTiles;
    public byte[,] topTiles;

    public Goals goals;

    [System.Serializable]
    public struct Goals
    {
        public int low;
        public int med;
        public int high;
        public int extra;
    }

    public LevelData()
    {
        name = "";
        description = "";

        world = World.Default;
        blockCounter = DefaultBlockCounter();
        fireTick = DefaultFireTick();

        width = 13;
        height = 13;

        goals.low = 1;
        goals.med = 2;
        goals.high = 3;
        goals.extra = 4;

        baseTiles = new byte[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };

        topTiles = new byte[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 }
        };
    }

    public LevelData(string _name, byte[,] _baseTiles, byte[,] _topTiles)
    {
        name = _name;
        description = "";
        
        world = World.FrostBurnHills;
        blockCounter = DefaultBlockCounter();
        fireTick = DefaultFireTick();

        width = _baseTiles.GetLength(0);
        height = _baseTiles.GetLength(1);

        baseTiles = _baseTiles;
        topTiles = _topTiles;

        goals.low = 1;
        goals.med = 2;
        goals.high = 3;
        goals.extra = 4;
    }

    public LevelData(RandomMaze randomConfig)
    {
        name = randomConfig.name;
        description = randomConfig.description;

        world = randomConfig.world;
        blockCounter = randomConfig.blockCounter;
        fireTick = randomConfig.fireTick;

        width = randomConfig.width;
        height = randomConfig.height;

        baseTiles = new byte[width, height];
        topTiles = new byte[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (randomConfig.defaultType == RandomMaze.DefaultType.Block)
                    topTiles[x, y] = (byte)randomConfig.block;
                else if (randomConfig.defaultType == RandomMaze.DefaultType.Hazard)
                    baseTiles[x, y] = (byte)randomConfig.hazard;
            }
        }

        goals.low = randomConfig.goals.low;
        goals.med = randomConfig.goals.med;
        goals.high = randomConfig.goals.high;
        goals.extra = randomConfig.goals.extra;
    }

    public static int DefaultBlockCounter()
    {
        return 30;
    }

    public static float DefaultFireTick()
    {
        return 0.8f;
    }
}
