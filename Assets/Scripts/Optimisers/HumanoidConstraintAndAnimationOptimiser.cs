using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HumanoidConstraintAndAnimationOptimiser : MonoBehaviour, IScriptOptimiser
{
    float current;
    float speed = 1;


    private void OnEnable()
    {
        ScriptOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnDisable()
    {
        ScriptOptimisationManager.Instance.RemoveOptimiser(this);

    }

    public void UpdateOptimiser()
    {
        current = Mathf.Sin(Time.time) * speed;

        transform.localScale = new Vector3(current, current, current);

        Debug.Log("yee");
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

}
