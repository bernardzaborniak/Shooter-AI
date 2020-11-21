using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonManager : MonoBehaviour
{
    public Transform camTransform;
    public HashSet<ManagedObject> managedObjects = new HashSet<ManagedObject>();


    #region Singleton Code
    public static SingletonManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion


    void Update()
    {
        Vector3 camForward = camTransform.forward;

        foreach (ManagedObject obj in managedObjects)
        {
            obj.UpdateManagedObject(camForward);
        }
    }

    public void AddManagedObject(ManagedObject obj)
    {
        managedObjects.Add(obj);
    }

    public void RemoveManagedObject(ManagedObject obj)
    {
        managedObjects.Remove(obj);
    }
}
