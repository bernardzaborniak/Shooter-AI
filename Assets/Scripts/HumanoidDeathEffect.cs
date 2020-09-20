using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidDeathEffect : MonoBehaviour
{


    void Start()
    {
        //gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void EnableDeathEffect()
    {
        Debug.Log("Enable death effect");
        transform.SetParent(null);
        gameObject.SetActive(true);
    }
}
