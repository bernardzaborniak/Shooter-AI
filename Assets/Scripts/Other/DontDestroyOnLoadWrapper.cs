using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadWrapper : MonoBehaviour
{
    public static DontDestroyOnLoadWrapper Instance;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }  
    }

}
