using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagedObject : MonoBehaviour
{

    //You can add the script to the singleton manager either on start or on enable, 
    //if you use onEnable, make sure the manager is executed before the managedObjects 
    //in the scriptExecution Order serrings to prevent null Exceptions
   
    /*private void OnEnable()
    {
        SingletonManager.Instance.AddManagedObject(this);
    }*/

    void Start()
    {
        SingletonManager.Instance.AddManagedObject(this);
    }

    void OnDisable()
    {
        SingletonManager.Instance.RemoveManagedObject(this);
    }

    public void UpdateManagedObject(Vector3 cameraForward)
    {
        transform.forward = cameraForward;
    }
}
