using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cinemachine;

public class LevelSelectManager : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private GameObject zoomedOutButtons;
    [SerializeField] private GameObject zoomedInButtons;
    
    [SerializeField] private CinemachineVirtualCamera defaultCam;
    [SerializeField] private CinemachineVirtualCamera[] worldCams;
    [SerializeField] private GameObject[] worldButtons;

    private bool zoomedOutView = true;
    private int worldIdx = 0;

    private void Awake()
    {
        ResetCams();

        scrollRect.enabled = true;
        zoomedOutView = true;

        zoomedOutButtons.SetActive(zoomedOutView);
        zoomedInButtons.SetActive(!zoomedOutView);

        ResetWorldButtons();
    }

    public void SwitchCamera(int world)
    {
        ResetCams();

        scrollRect.enabled = false;
        zoomedOutView = false;

        zoomedOutButtons.SetActive(zoomedOutView);
        zoomedInButtons.SetActive(!zoomedOutView);

        ResetWorldButtons();

        worldIdx = world;
        worldCams[worldIdx].Priority = 2;
    }

    public void ZoomOut()
    {
        ResetCams();

        scrollRect.enabled = true;
        zoomedOutView = true;

        zoomedOutButtons.SetActive(zoomedOutView);
        zoomedInButtons.SetActive(!zoomedOutView);

        ResetWorldButtons();
    }

    public void GoToNextWorld()
    {
        worldIdx++;
        if (worldIdx >= worldCams.Length)
            worldIdx = 0;

        SwitchCamera(worldIdx);
    }

    public void GoToPreviousWorld()
    {
        worldIdx--;
        if (worldIdx < 0)
            worldIdx = worldCams.Length - 1;

        SwitchCamera(worldIdx);
    }

    private void ResetCams()
    {
        zoomedOutView = true;
        defaultCam.Priority = 1;
        foreach (var cam in worldCams)
        {
            cam.Priority = 0;
        }
    }

    private void ResetWorldButtons()
    {
        foreach (var go in worldButtons)
            go.SetActive(zoomedOutView);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
