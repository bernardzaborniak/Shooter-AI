﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerController : MonoBehaviour
{
    public SimpleCameraController simpleCameraController;
    public TextMeshProUGUI tmp_currentTimescale;

    [Header("For Camera Gun")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    [Tooltip("max is 5")]
    public int timeSpeedLevel = 3;

    [Header("Controls Screen")]
    public GameObject controlsScreen;
    public GameObject openControlsButton;



    enum PlayerControllerMode
    {
        Fly,
        Posess
    }

    PlayerControllerMode playerControllerMode;

    void Start()
    {
        ChangeToFlyMode();

        timeSpeedLevel = 3;
        Time.timeScale = 1;

    }

    void Update()
    {

        #region Slowmo Input

        /*else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Time.timeScale = 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Time.timeScale = 0.2f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Time.timeScale = 0.3f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Time.timeScale = 0.4f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            Time.timeScale = 0.5f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Time.timeScale = 0.6f;
        }
        else */
        /*if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Time.timeScale = 0f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 0.2f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 0.5f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Time.timeScale = 1f;
        }*/

        if (Input.GetKeyDown(KeyCode.C))
        {
            timeSpeedLevel = Mathf.Clamp(timeSpeedLevel - 1, 0, 5);

            UpdateTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            timeSpeedLevel = Mathf.Clamp(timeSpeedLevel + 1, 0, 5);

            UpdateTimeScale();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Time.timeScale == 0)
            {
                UpdateTimeScale();
            }
            else
            {
                Time.timeScale = 0;
                tmp_currentTimescale.text = Time.timeScale.ToString("F2");
            }
        }

        #endregion

        #region FlyGun

        if (playerControllerMode == PlayerControllerMode.Fly)
        {
            /*if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
                projectileGO.GetComponent<Rigidbody>().velocity = projectileGO.transform.forward * 850;
            }*/
        }

        #endregion



        //Back to Main menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    void UpdateTimeScale()
    {
        if (timeSpeedLevel == 0)
        {
            Time.timeScale = 0f;
        }
        else if (timeSpeedLevel == 1)
        {
            Time.timeScale = 0.2f;

        }
        else if (timeSpeedLevel == 2)
        {
            Time.timeScale = 0.5f;

        }
        else if (timeSpeedLevel == 3)
        {
            Time.timeScale = 1f;
        }
        else if (timeSpeedLevel == 4)
        {
            Time.timeScale = 2f;
        }
        else if (timeSpeedLevel == 5)
        {
            Time.timeScale = 5f;
        }
        tmp_currentTimescale.text = Time.timeScale.ToString("F2");
    }

    void ChangeToFlyMode()
    {
        simpleCameraController.enabled = true;
    }

    void ChangeToPosessMode()
    {
        simpleCameraController.enabled = false;

    }

    public void OnOpenControlsMenuClicked()
    {
        controlsScreen.SetActive(true);
        openControlsButton.SetActive(false);
    }

    public void OnCloseControlsMenuClicked()
    {
        controlsScreen.SetActive(false);
        openControlsButton.SetActive(true);
    }
}
