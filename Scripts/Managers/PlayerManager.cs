using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using static MazeManager;

public class PlayerManager : MonoBehaviour
{
    public enum PlayerMode { Play, Test }

    [System.Serializable]
    private struct WorldColorScheme
    {
        public AssetCatalogue.World world;
        public Color primary;
        public Color secondary;
    }

    [HideInInspector] public static PlayerMode playerMode;

    [SerializeField] private Button startButton;

    [Space]
    [SerializeField] private Image topBarImg;
    [SerializeField] private Image bottomBarImg;
    [SerializeField] private Image stepBkgImg;
    [SerializeField] private Image background;

    [SerializeField] private WorldColorScheme[] colorsUI;

    [Space]
    [SerializeField] private TextMeshProUGUI worldNameTxt;
    [SerializeField] private TextMeshProUGUI levelNameTxt;
    [SerializeField] private TextMeshProUGUI stepCounterTxt;

    [SerializeField] private TextMeshProUGUI goalLowTxt;
    [SerializeField] private TextMeshProUGUI goalMedTxt;
    [SerializeField] private TextMeshProUGUI goalHighTxt;
    [SerializeField] private TextMeshProUGUI goalExtraTxt;

    private static PlayerManager instance;

    void Awake()
    {
        instance = this;
        
        maze = null;
        
        Camera.main.transform.position = new Vector3(mazeConfig.width * 0.5f, mazeConfig.height * 0.5f, -10);
        Camera.main.orthographicSize = (mazeConfig.height + 1) * 0.5f * 1.21f;
        ScreenShake.SetOriginalPosition();

        Color primary = new Color();
        Color secondary = new Color();
        foreach (WorldColorScheme scheme in colorsUI)
        {
            if (scheme.world == mazeConfig.world)
            {
                primary = scheme.primary;
                secondary = scheme.secondary;
            }
        }
        topBarImg.color = primary;
        bottomBarImg.color = primary;
        stepBkgImg.color = secondary;

        Color backgroundColor = primary;
        backgroundColor.a = 0.7f;
        background.color = backgroundColor;


        worldNameTxt.text = AssetCatalogue.GetWorldName(mazeConfig.world);
        levelNameTxt.text = mazeConfig.name;

        goalLowTxt.text = mazeConfig.goals.low.ToString();
        goalMedTxt.text = mazeConfig.goals.med.ToString();
        goalHighTxt.text = mazeConfig.goals.high.ToString();
        goalExtraTxt.text = mazeConfig.goals.extra.ToString();
    }

    void Start()
    {
        StaticReset();
    }

    void Update()
    {
        bool readyStart = !gameOver && !gameRunning;

        startButton.interactable = readyStart;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Back();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StaticReset();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (readyStart)
                StaticStart();
            else if (gameOver)
                StaticReset();
            else
                ToggleSpeed();
        }
    }

    public static void Setup(PlayerMode mode)
    {
        playerMode = mode;
    }

    public void Back()
    {
        ResetTimeScale();
        SoundManager.StopThemes();
        
        if (playerMode == PlayerMode.Play)
        {
            SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
        }
        else if (playerMode == PlayerMode.Test)
        {
            SceneManager.LoadScene("LevelEditor", LoadSceneMode.Single);
        }
    }

    public static void UpdateStepCounter(int totalSteps)
    {
        instance.stepCounterTxt.text = totalSteps.ToString();
    }
}
