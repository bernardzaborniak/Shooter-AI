using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SerializedTest1 : MonoBehaviour
{


    void Start()
    {
        var so = new SerializedObject(transform);
    }

    void Update()
    {
        
    }
}
