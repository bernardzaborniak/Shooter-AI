using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGun : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
        }
    }
}
