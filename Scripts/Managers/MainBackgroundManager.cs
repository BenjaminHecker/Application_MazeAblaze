using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MainBackgroundManager : MonoBehaviour
{
    private static MainBackgroundManager instance;
    
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int snakeLength;
    [SerializeField] private float pathTick = 0.2f;
    
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase baseTile;
    [SerializeField] private TileBase pathTile;

    private List<BackgroundSnake> snakes;

    private void Awake()
    {
        instance = this;

        snakes = new List<BackgroundSnake>();
        snakes.Add(new BackgroundSnake(new Vector3Int(0, 0, 0), snakeLength));
        snakes.Add(new BackgroundSnake(new Vector3Int(width - 1, 0, 0), snakeLength));
        snakes.Add(new BackgroundSnake(new Vector3Int(0, height - 1, 0), snakeLength));
        snakes.Add(new BackgroundSnake(new Vector3Int(width - 1, height - 1, 0), snakeLength));

        InvokeRepeating("UpdateSnakes", 0f, pathTick);
    }

    private void UpdateSnakes()
    {
        ResetTilemap();

        foreach (BackgroundSnake snake in snakes)
            snake.UpdateSnake();
    }

    public static int Width() { return instance.width; }
    public static int Height() { return instance.height; }
    public static Tilemap GetTilemap() { return instance.tilemap; }
    public static TileBase GetPathTile() { return instance.pathTile; }

    private void ResetTilemap()
    {
        tilemap.ClearAllTiles();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), baseTile);
            }
        }
    }
}
