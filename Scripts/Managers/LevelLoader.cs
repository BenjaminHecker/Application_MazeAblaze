using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private GameObject levelButtonPrefab;
    
    private List<LevelData> levels;
    
    void Awake()
    {
        levels = new List<LevelData>();
        string[] files = System.IO.Directory.GetFiles(Application.persistentDataPath + "/custom levels");

        for (int i = 0; i < files.Length; i++)
        {
            LevelData level = SaveSystem.GetLevel(files[i]);
            DisplayLevel(level, i, files[i]);
            levels.Add(level);
        }
    }

    private void DisplayLevel(LevelData level, int i, string filepath)
    {
        GameObject go = Instantiate(levelButtonPrefab, levelButtonPrefab.transform.parent);
        go.SetActive(true);
        go.transform.Find("Level Nav").GetComponent<Button>().onClick.AddListener(() => OpenLevel(i, filepath));
        go.transform.Find("Level Nav/Level Name").GetComponent<TextMeshProUGUI>().text = level.name;
    }

    public void OpenLevel(int i, string filepath)
    {
        PrelimManager.Setup(levels[i], filepath);
        SceneManager.LoadScene("LevelPrelim", LoadSceneMode.Single);
    }

    public void NewLevel()
    {
        PrelimManager.SetupNew();
        SceneManager.LoadScene("LevelPrelim", LoadSceneMode.Single);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void GenerateSampleLevels()
    {
        LevelData temp1 = new LevelData(
            "Sample 1",
            new byte[,]
            {
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 },
                { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
            },
            new byte[,]
            {
                { 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 3, 3, 3, 3, 0, 3, 0 },
                { 0, 3, 0, 0, 0, 3, 0, 3, 0, 0, 0, 3, 0 },
                { 0, 3, 3, 3, 3, 3, 0, 4, 0, 3, 3, 3, 0 },
                { 0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0 },
                { 3, 3, 4, 3, 0, 3, 3, 3, 0, 3, 0, 3, 3 },
                { 0, 0, 0, 3, 0, 0, 2, 3, 0, 3, 0, 0, 0 },
                { 3, 3, 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0 },
                { 0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0 },
                { 0, 3, 3, 3, 0, 3, 0, 3, 3, 3, 0, 3, 0 },
                { 0, 3, 0, 0, 0, 4, 0, 3, 0, 0, 0, 3, 0 },
                { 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0, 3, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1 }
            }
        );

        SaveSystem.SaveLevel(temp1, Application.persistentDataPath + "/custom levels/Sample 1.maze");


        LevelData temp2 = new LevelData(
            "Sample 2",
            new byte[,]
            {
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1, 0, 0 },
                { 1, 1, 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
            },
            new byte[,]
            {
                { 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 4, 0 },
                { 0, 3, 0, 3, 0, 5, 3, 3, 3, 3, 0, 3, 0 },
                { 0, 4, 0, 0, 0, 3, 0, 3, 2, 0, 0, 4, 0 },
                { 0, 5, 3, 4, 3, 3, 0, 4, 0, 3, 4, 3, 0 },
                { 0, 0, 2, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0 },
                { 3, 3, 4, 3, 0, 3, 3, 3, 0, 3, 2, 3, 3 },
                { 0, 0, 0, 3, 0, 0, 2, 3, 0, 4, 0, 0, 0 },
                { 3, 3, 0, 4, 3, 3, 3, 4, 0, 3, 5, 3, 0 },
                { 0, 0, 0, 2, 0, 3, 0, 0, 0, 3, 0, 0, 0 },
                { 0, 3, 3, 3, 0, 3, 0, 3, 3, 3, 0, 3, 0 },
                { 0, 4, 0, 0, 0, 4, 0, 5, 0, 0, 0, 4, 0 },
                { 0, 3, 3, 3, 3, 4, 0, 3, 3, 4, 0, 3, 0 },
                { 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1 }
            }
        );

        SaveSystem.SaveLevel(temp2, Application.persistentDataPath + "/custom levels/Sample 2.maze");


        LevelData temp3 = new LevelData(
            "Sample 3",
            new byte[,]
            {
                { 1, 1, 1 },
                { 0, 0, 1 },
                { 0, 0, 1 }
            },
            new byte[,] {
                { 1, 0, 0 },
                { 3, 3, 0 },
                { 3, 3, 2 }
            }
        );

        SaveSystem.SaveLevel(temp3, Application.persistentDataPath + "/custom levels/Sample 3.maze");

        LevelData temp4 = new LevelData(
            "Sample 4",
            new byte[,]
            {
                { 1, 1, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0, 1 },
                { 1, 0, 1, 0, 1, 0, 0, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 1, 6, 6, 6, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 6, 1, 1, 1 },
                { 0, 0, 0, 0, 1, 0, 0, 0, 1, 6, 1, 0, 0 },
                { 1, 1, 1, 0, 1, 1, 1, 0, 1, 6, 1, 1, 1 },
                { 0, 0, 1, 0, 0, 0, 0, 0, 1, 6, 6, 6, 1 },
                { 1, 1, 1, 1, 1, 0, 1, 1, 1, 0, 1, 1, 1 },
                { 1, 0, 0, 0, 1, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 0, 1, 1, 1, 0, 1, 0, 1, 1, 1, 0, 1 },
                { 1, 0, 0, 0, 0, 0, 1, 0, 0, 0, 1, 0, 1 },
                { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 1 }
            },
            new byte[,]
            {
                { 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 3, 0 },
                { 0, 3, 0, 3, 0, 3, 3, 3, 3, 3, 0, 3, 0 },
                { 0, 3, 0, 0, 0, 3, 0, 3, 0, 0, 0, 3, 0 },
                { 0, 3, 3, 3, 3, 3, 0, 4, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0 },
                { 3, 3, 4, 3, 0, 3, 3, 3, 0, 0, 0, 3, 3 },
                { 0, 0, 0, 3, 0, 0, 2, 3, 0, 0, 0, 0, 0 },
                { 3, 3, 0, 3, 3, 3, 3, 3, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 3, 0, 0, 0, 3, 0, 0, 0 },
                { 0, 3, 3, 3, 0, 3, 0, 3, 3, 3, 0, 3, 0 },
                { 0, 3, 0, 0, 0, 4, 0, 3, 0, 0, 0, 3, 0 },
                { 0, 3, 3, 3, 3, 3, 0, 3, 3, 3, 0, 3, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 1 }
            }
        );

        SaveSystem.SaveLevel(temp4, Application.persistentDataPath + "/custom levels/Sample 4.maze");
    }
}
