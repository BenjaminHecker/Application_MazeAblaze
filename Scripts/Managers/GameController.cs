/*
 * Handles higher level functions such as pressing the Quit button to exit the game
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();
    }
}