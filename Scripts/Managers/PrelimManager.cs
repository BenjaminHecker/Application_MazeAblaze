using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using static MazeManager;

public class PrelimManager : MonoBehaviour
{
    private enum PrelimMode { New, Existing }
    private static PrelimMode prelimMode;
    
    private static string activeFile;

    private static PrelimManager instance;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField descriptionField;

    [SerializeField] private TextMeshProUGUI mazeSizeTxt;
    [SerializeField] private TextMeshProUGUI blockCounterTxt;
    [SerializeField] private TextMeshProUGUI flameSpeedTxt;

    [SerializeField] private TMP_InputField goalLowField;
    [SerializeField] private TMP_InputField goalMedField;
    [SerializeField] private TMP_InputField goalHighField;
    [SerializeField] private TMP_InputField goalExtraField;
    
    void Awake()
    {
        instance = this;

        nameField.text = mazeConfig.name;
        descriptionField.text = mazeConfig.description;

        mazeSizeTxt.text = mazeConfig.width.ToString() + "x" + mazeConfig.height.ToString();
        blockCounterTxt.text = mazeConfig.blockCounter.ToString();
        flameSpeedTxt.text = (LevelData.DefaultFireTick() / mazeConfig.fireTick).ToString("0.0");

        goalLowField.text = mazeConfig.goals.low.ToString();
        goalMedField.text = mazeConfig.goals.med.ToString();
        goalHighField.text = mazeConfig.goals.high.ToString();
        goalExtraField.text = mazeConfig.goals.extra.ToString();
    }

    public static void Setup(LevelData data, string path)
    {
        levelType = LevelType.Fixed;
        mazeConfig = data;
        activeFile = path;
        prelimMode = PrelimMode.Existing;
    }

    public static void SetupNew()
    {
        levelType = LevelType.Fixed;
        mazeConfig = new LevelData();
        activeFile = "";
        prelimMode = PrelimMode.New;
    }

    public void Back()
    {
        SaveLevel();
        SceneManager.LoadScene("LevelLoader", LoadSceneMode.Single);
    }

    public void PlayLevel()
    {
        SaveLevel();
        PlayerManager.Setup(PlayerManager.PlayerMode.Play);
        SceneManager.LoadScene("LevelPlayer", LoadSceneMode.Single);
    }

    public void EditLevel()
    {
        SaveLevel();
        SceneManager.LoadScene("LevelEditor", LoadSceneMode.Single);
    }

    public static void SaveLevel()
    {
        if (prelimMode == PrelimMode.New)
        {
            activeFile = Application.persistentDataPath + "/custom levels/" + string.Join("_", instance.nameField.text.Split(Path.GetInvalidFileNameChars())) + ".maze";
            prelimMode = PrelimMode.Existing;
        }

        mazeConfig.name = instance.nameField.text;
        mazeConfig.description = instance.descriptionField.text;

        int.TryParse(instance.goalLowField.text, out mazeConfig.goals.low);
        int.TryParse(instance.goalMedField.text, out mazeConfig.goals.med);
        int.TryParse(instance.goalHighField.text, out mazeConfig.goals.high);
        int.TryParse(instance.goalExtraField.text, out mazeConfig.goals.extra);

        SaveSystem.SaveLevel(mazeConfig, activeFile);
    }
}
