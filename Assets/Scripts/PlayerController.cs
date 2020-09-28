using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTemplateProjects;

public class PlayerController : MonoBehaviour
{
    public SimpleCameraController simpleCameraController;

    [Header("For Camera Gun")]
    public GameObject projectilePrefab;
    public Transform shootPoint;

    enum PlayerControllerMode
    {
        Fly,
        Posess
    }

    PlayerControllerMode playerControllerMode;

    void Start()
    {
        ChangeToFlyMode();
    }

    void Update()
    {
        #region Slowmo Input
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            Time.timeScale = 1f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
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
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            Time.timeScale = 0.7f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            Time.timeScale = 0.8f;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            Time.timeScale = 0.9f;
        }

        #endregion

        #region FlyGun

        if(playerControllerMode == PlayerControllerMode.Fly)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject projectileGO = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
                projectileGO.GetComponent<Rigidbody>().velocity = projectileGO.transform.forward * 850;
            }
        }

        #endregion
    }

    void ChangeToFlyMode()
    {
        simpleCameraController.enabled = true;
    }

    void ChangeToPosessMode()
    {
        simpleCameraController.enabled = false;

    }
}
