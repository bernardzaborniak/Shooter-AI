using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestOptimiser : MonoBehaviour, IScriptOptimiser
{
    float current;
    float speed = 1;

    float lastSenseTime = 0;

    private void OnEnable()
    {
        //ScriptOptimisationManager.Instance.AddOptimiser(this);
        TestOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnDisable()
    {
        //ScriptOptimisationManager.Instance.RemoveOptimiser(this);
        TestOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        current = Mathf.Sin(Time.time) * speed;

        transform.localScale = new Vector3(current, current, current);

       // Debug.Log("yee");

        Debug.Log("updated after " + (Time.time - lastSenseTime) + " s");
        lastSenseTime = Time.time;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public string GetName()
    {
        return transform.parent.name;
    }

}
