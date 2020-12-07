using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensingOptimiser : MonoBehaviour, IScriptOptimiser
{
    private void OnEnable()
    {
         SensingOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnDisable()
    {
         SensingOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        /*current = Mathf.Sin(Time.time) * speed;

        transform.localScale = new Vector3(current, current, current);

        Debug.Log("yee");*/
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
