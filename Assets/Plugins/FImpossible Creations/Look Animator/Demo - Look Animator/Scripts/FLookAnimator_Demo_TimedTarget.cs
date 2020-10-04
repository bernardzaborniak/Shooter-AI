using FIMSpace.FLook;
using System.Collections.Generic;
using UnityEngine;

public class FLookAnimator_Demo_TimedTarget : MonoBehaviour
{
    public FLookAnimator Look;
    public List<Transform> Targets;
    public Vector2 TimeRange = new Vector2(3f, 4f);
    private float timer = 0f;
    private int current = 0;

    private Transform GetTarget()
    {
        if (current >= Targets.Count) current = 0;
        return Targets[current++];
    }
	
	void Update ()
    {
        timer -= Time.deltaTime;

		if ( timer < 0f)
        {
            Look.ObjectToFollow = GetTarget();
            timer = Random.Range(TimeRange.x, TimeRange.y);
        }
    }
}
