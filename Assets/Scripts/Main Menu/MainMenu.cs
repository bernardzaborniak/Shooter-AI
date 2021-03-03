using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject controlsMenu;
    public GameObject mainMenu;

    public void OnEnterMap1Clicked()
    {
        SceneManager.LoadScene(1);
    }
    public void OnEnterMap2Clicked()
    {
        SceneManager.LoadScene(2);
    }

    public void OnSettingsClicked()
    {
        controlsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void OnBackToMainMenuClicked()
    {
        controlsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void OnExitClicked()
    {
        Application.Quit();
    }
}
