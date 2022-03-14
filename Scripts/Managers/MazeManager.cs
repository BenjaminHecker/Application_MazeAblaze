using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeManager : MonoBehaviour
{
    public static MazeManager instance;
    
    public static Maze maze;
    public static bool gameOver;
    public static bool gameRunning;

    public enum LevelType { Fixed, Random }

    public static LevelType levelType;
    public static LevelData mazeConfig;
    public static RandomMaze randomConfig;

    public static Tilemap backgroundTilemap;
    public static Tilemap groundTilemap;
    public static Tilemap flamePathTilemap;
    public static Tilemap hazardTilemap;
    public static Tilemap dragRegionTilemap;

    private int totalSteps;

    private BackgroundArt backgroundArt;

    private IEnumerator advanceFire;
    private IEnumerator gameOverRoutine;
    private IEnumerator cascade;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        backgroundTilemap = GameObject.Find("Grid/Background Art").GetComponent<Tilemap>();
        groundTilemap = GameObject.Find("Grid/Ground").GetComponent<Tilemap>();
        flamePathTilemap = GameObject.Find("Grid/Flame Path").GetComponent<Tilemap>();
        hazardTilemap = GameObject.Find("Grid/Hazards").GetComponent<Tilemap>();
        dragRegionTilemap = GameObject.Find("Grid/Drag Region").GetComponent<Tilemap>();

        gameOver = true;
        gameRunning = false;
        maze = ScriptableObject.CreateInstance<Maze>();

        GenerateBackground();
        
        maze.Generate();
        GeneratePath(out _);
    }

    public static void StaticReset()
    {
        instance.ResetGame();
    }

    public static void StaticStart()
    {
        instance.StartGame();
    }

    public static void ToggleSpeed()
    {
        Time.timeScale = (Time.timeScale == 2) ? 1 : 2;
        SoundManager.UpdatePitch();
    }

    public static void ResetTimeScale()
    {
        Time.timeScale = 1;
        SoundManager.UpdatePitch();
    }

    private void GenerateBackground()
    {
        backgroundArt = AssetCatalogue.GetBackgroundArt();
        backgroundArt.Generate();
    }

    public void ResetGame()
    {
        ResetTimeScale();

        if (advanceFire != null)
            StopCoroutine(advanceFire);
        if (gameOverRoutine != null)
            StopCoroutine(gameOverRoutine);
        if (cascade != null)
            StopCoroutine(cascade);

        CancelInvoke("AdvanceFire");

        totalSteps = 0;
        PlayerManager.UpdateStepCounter(totalSteps);

        if (levelType == LevelType.Random)
            mazeConfig = MazeRandomizer.GenerateRandomMaze(randomConfig);

        maze.Generate();
        GeneratePath(out _);

        gameOver = false;
        gameRunning = false;

        SoundManager.StopThemes();
    }

    public void StartGame()
    {
        ResetTimeScale();

        gameRunning = true;
        
        float initialDelay = SoundManager.PlayTheme(mazeConfig.world);

        InvokeRepeating("AdvanceFire", initialDelay, mazeConfig.fireTick);
    }

    private void AdvanceFire()
    {
        List<Cell> path;

        // runs until the fire reaches the target
        if (!maze.FlameAtTarget())
        {
            GeneratePath(out path);
            backgroundArt.Dance();

            // if a path was found, move the fire along that path, increment the step counter, and decrement all MazeTiles
            if (path.Count > 0)
            {
                maze.MoveFlame(path[0]);

                totalSteps++;
                PlayerManager.UpdateStepCounter(totalSteps);

                maze.TriggerPassive();
            }
            // otherwise if no path is found, crumble the lowest value tiles
            else
            {
                maze.TriggerDestructive();
                ScreenShake.TriggerShake(0.1f, 0.1f);
            }

            return;
        }

        // start coroutine for ending the game
        gameOverRoutine = GameOver();
        StartCoroutine(gameOverRoutine);

        CancelInvoke("AdvanceFire");
    }

    public static void GeneratePath(out List<Cell> path)
    {
        Pathfinder.PathToNearestTarget(out path, maze.flame.mazePos);
        maze.DrawPath(ref path);
    }

    private IEnumerator GameOver()
    {
        ResetTimeScale();

        gameOver = true;
        gameRunning = false;

        SoundManager.StopThemes();

        cascade = maze.CascadeExplosions();                                          // triggers cascading explosions
        yield return StartCoroutine(cascade);
    }
}
