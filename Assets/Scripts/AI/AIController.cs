using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public GameEntity entityAttachedTo;
    public AIComponent[] aIComponents;

    void Start()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].SetUpComponent(entityAttachedTo);
        }
    }

    void Update()
    {
        for (int i = 0; i < aIComponents.Length; i++)
        {
            aIComponents[i].UpdateComponent();
        }
    }
}
