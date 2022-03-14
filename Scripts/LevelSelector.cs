using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class LevelSelector : MonoBehaviour
{
    [SerializeField] private MazeManager.LevelType levelType;
    [SerializeField] private TextAsset level;
    [SerializeField] private RandomMaze randomData;

    private LevelData fixedData;

    private void Awake()
    {
        if (levelType == MazeManager.LevelType.Fixed)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            Stream stream = new MemoryStream(level.bytes);
            fixedData = (LevelData)formatter.Deserialize(stream);
        }
    }

    public void PlayLevel()
    {
        MazeManager.levelType = levelType;

        if (levelType == MazeManager.LevelType.Fixed)
            MazeManager.mazeConfig = fixedData;
        else
        {
            MazeManager.randomConfig = randomData;
            MazeManager.mazeConfig = MazeRandomizer.GenerateRandomMaze(randomData);
        }

        PlayerManager.Setup(PlayerManager.PlayerMode.Play);
        SceneManager.LoadScene("LevelPlayer", LoadSceneMode.Single);
    }
}
