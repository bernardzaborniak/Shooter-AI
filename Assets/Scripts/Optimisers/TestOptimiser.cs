using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class TestOptimiser : MonoBehaviour, IScriptOptimiser
{
    float current;
    float speed = 1;

    float lastSenseTime;
    int lastSenseFrameCount;

    private void OnEnable()
    {
        TestOptimisationManager.Instance.AddOptimiser(this);
        lastSenseTime = Time.time;
        lastSenseFrameCount = Time.frameCount;
    }

    private void OnDisable()
    {
        TestOptimisationManager.Instance.RemoveOptimiser(this);
    }

    public void UpdateOptimiser()
    {
        current = Mathf.Sin(Time.time) * speed;

        transform.localScale = new Vector3(current, current, current);

        Debug.Log("updated after " + (Time.time - lastSenseTime) + " s & " + (Time.frameCount - lastSenseFrameCount) + " frames");
        lastSenseTime = Time.time;
        lastSenseFrameCount = Time.frameCount;
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
