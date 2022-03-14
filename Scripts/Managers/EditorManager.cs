using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using static AssetCatalogue;
using static MazeManager;
using static MazeEditor;

public class EditorManager : MonoBehaviour
{
    private static Transform topButton;
    private static Transform baseButton;
    private static Transform topMazePaletteItems;
    private static Transform baseMazePaletteItems;
    private static Transform deleteButton;

    private static Slider mazeSizeSlider;
    private static Slider blockCounterSlider;
    private static Slider flameSpeedSlider;

    private static TextMeshProUGUI worldTxt;
    private static TextMeshProUGUI mazeSizeTxt;
    private static TextMeshProUGUI blockCounterTxt;
    private static TextMeshProUGUI flameSpeedTxt;

    void Awake()
    {
        maze = null;
        
        Camera.main.transform.position = new Vector3(mazeConfig.width * 0.5f, mazeConfig.height * 0.5f, -10);
        Camera.main.orthographicSize = (mazeConfig.height + 1) * 0.5f * 1.21f;

        GameObject.Find("Canvas/Level Name").GetComponent<TextMeshProUGUI>().text = mazeConfig.name;

        topButton = GameObject.Find("Canvas/Right Panel/Maze Palette/Tabs/Top button").transform;
        baseButton = GameObject.Find("Canvas/Right Panel/Maze Palette/Tabs/Base button").transform;
        topMazePaletteItems = GameObject.Find("Canvas/Right Panel/Maze Palette/Top").transform;
        baseMazePaletteItems = GameObject.Find("Canvas/Right Panel/Maze Palette/Base").transform;
        deleteButton = GameObject.Find("Canvas/Right Panel/Delete").transform;

        mazeSizeSlider = GameObject.Find("Canvas/Left Panel/Maze Size/slider").GetComponent<Slider>();
        blockCounterSlider = GameObject.Find("Canvas/Left Panel/Block Counter/slider").GetComponent<Slider>();
        flameSpeedSlider = GameObject.Find("Canvas/Left Panel/Flame Speed/slider").GetComponent<Slider>();

        worldTxt = GameObject.Find("Canvas/Left Panel/World Selector/value").GetComponent<TextMeshProUGUI>();
        mazeSizeTxt = GameObject.Find("Canvas/Left Panel/Maze Size/value").GetComponent<TextMeshProUGUI>();
        blockCounterTxt = GameObject.Find("Canvas/Left Panel/Block Counter/value").GetComponent<TextMeshProUGUI>();
        flameSpeedTxt = GameObject.Find("Canvas/Left Panel/Flame Speed/value").GetComponent<TextMeshProUGUI>();

        worldTxt.text = GetWorldName(mazeConfig.world);

        mazeSizeSlider.SetValueWithoutNotify(mazeConfig.width);
        mazeSizeTxt.text = mazeConfig.width.ToString() + "x" + mazeConfig.height.ToString();
        
        blockCounterSlider.SetValueWithoutNotify(mazeConfig.blockCounter);
        blockCounterTxt.text = mazeConfig.blockCounter.ToString();

        float flameSpeed = (float) Math.Round(LevelData.DefaultFireTick() / mazeConfig.fireTick, 1);
        flameSpeedSlider.SetValueWithoutNotify(flameSpeed);
        flameSpeedTxt.text = flameSpeed.ToString("0.0");
    }

    void Start()
    {
        Reset();
    }

    public static void Exit()
    {
        PrelimManager.SaveLevel();
        SceneManager.LoadScene("LevelPrelim", LoadSceneMode.Single);
    }

    public static void TestLevel()
    {
        PrelimManager.SaveLevel();
        PlayerManager.Setup(PlayerManager.PlayerMode.Test);
        SceneManager.LoadScene("LevelPlayer", LoadSceneMode.Single);
    }

    public static void SetPaletteModeTop()
    {
        paletteMode = PaletteMode.Top;
        ResetItem();
    }

    public static void SetPaletteModeBase()
    {
        paletteMode = PaletteMode.Base;
        ResetItem();
    }
    
    public static void UpdateRightPanel()
    {
        MazePaletteItem[] items;
        Color groupColor;
        
        if (paletteMode == PaletteMode.Top)
        {
            topButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            baseButton.GetComponent<Image>().color = new Color(1, 1, 1, (float) 140 / 255);
            topMazePaletteItems.gameObject.SetActive(true);
            baseMazePaletteItems.gameObject.SetActive(false);

            groupColor = new Color(1, 1, 1, 1);

            items = topMazePaletteItems.GetComponentsInChildren<MazePaletteItem>();
        }
        else
        {
            topButton.GetComponent<Image>().color = new Color(1, 1, 1, (float) 140 / 255);
            baseButton.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            topMazePaletteItems.gameObject.SetActive(false);
            baseMazePaletteItems.gameObject.SetActive(true);

            groupColor = new Color(1, 1, 1, 0.3f);

            items = baseMazePaletteItems.GetComponentsInChildren<MazePaletteItem>();
        }

        if (flamePathTilemap != null)
            flamePathTilemap.color = groupColor;

        if (maze != null)
            maze.flame.SetSpriteColor(groupColor);

        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Target"))
            if (child.activeSelf)
                child.GetComponent<Target>().SetSpriteColor(groupColor);
        foreach (GameObject child in GameObject.FindGameObjectsWithTag("Block"))
            if (child.activeSelf)
                child.GetComponent<Block>().SetSpriteColor(groupColor);

        foreach (MazePaletteItem item in items)
        {
            item.SetSelected(editorMode == EditorMode.Draw && selectedItem == item);
            item.UpdateSprite();
        }

        if (editorMode == EditorMode.Delete)
        {
            deleteButton.GetComponent<Image>().color = new Color(1, 1, 1, (float) 100 / 255);
        }
        else
        {
            deleteButton.GetComponent<Image>().color = new Color(1, 1, 1, (float) 20 / 255);
        }
    }

    public static void SetEditorModeDelete()
    {
        editorMode = EditorMode.Delete;
        UpdateRightPanel();
    }

    public static void CycleWorld(bool forward)
    {
        int worldCount = 4;

        if (forward)
        {
            mazeConfig.world++;

            if ((int)mazeConfig.world >= worldCount)
                mazeConfig.world = 0;
        }
        else
        {
            mazeConfig.world--;

            if ((int)mazeConfig.world >= worldCount)
                mazeConfig.world = (World)(worldCount - 1);
        }

        worldTxt.text = GetWorldName(mazeConfig.world);

        maze.Generate();
        GeneratePath(out _);
        UpdateRightPanel();
        UpdateHoverItem();
    }

    public static void UpdateMazeSize()
    {
        ResizeMaze();
        mazeSizeTxt.text = mazeConfig.width.ToString() + "x" + mazeConfig.height.ToString();
    }

    public static void UpdateBlockCounter()
    {
        mazeConfig.blockCounter = (int)blockCounterSlider.value;
        blockCounterTxt.text = mazeConfig.blockCounter.ToString();
    }

    public static void UpdateFlameSpeed()
    {
        mazeConfig.fireTick = (float)Math.Round(LevelData.DefaultFireTick() / flameSpeedSlider.value, 1);
        flameSpeedTxt.text = flameSpeedSlider.value.ToString("0.0");
    }

    private static void ResizeMaze()
    {
        int size = (int) mazeSizeSlider.value;

        if (maze != null && (maze.flame.X >= size || maze.flame.Y >= size))
        {
            Cell newFlamePos = new Cell(0, 0);

            RemoveItem(newFlamePos);
            
            mazeConfig.topTiles[maze.flame.mazePos.x, maze.flame.mazePos.y] = 0;
            mazeConfig.topTiles[0, 0] = 1;

            maze.MoveFlame(newFlamePos);
        }
        
        byte[,] newBaseTiles = new byte[size, size];
        byte[,] newTopTiles = new byte[size, size];
        
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (x < mazeConfig.width && y < mazeConfig.height)
                {
                    newBaseTiles[x, y] = mazeConfig.baseTiles[x, y];
                    newTopTiles[x, y] = mazeConfig.topTiles[x, y];
                }
                else
                {
                    newBaseTiles[x, y] = 0;
                    newTopTiles[x, y] = 0;
                }
            }
        }

        mazeConfig.width = size;
        mazeConfig.height = size;
        mazeConfig.baseTiles = newBaseTiles;
        mazeConfig.topTiles = newTopTiles;

        if (maze != null)
        {
            maze.Generate();
            GeneratePath(out _);
        }

        Camera.main.transform.position = new Vector3(mazeConfig.width * 0.5f, mazeConfig.height * 0.5f, -10);
        Camera.main.orthographicSize = (mazeConfig.height + 1) * 0.5f;
    }
}
