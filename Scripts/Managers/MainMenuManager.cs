using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect", LoadSceneMode.Single);
    }

    public void LevelEditor()
    {
        SceneManager.LoadScene("LevelLoader", LoadSceneMode.Single);
    }
}
