using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//[RequireComponent(typeof(Renderer))]
public class HumanoidConstraintAndAnimationOptimiser : MonoBehaviour, IScriptOptimiser
{
    //float max = 2;
    //float min = 0.5f;
   float current;
    float speed = 1;

    // Start is called before the first frame update
    private void OnEnable()
    {
        //Debug.Log("Instance == null: " + ScriptOptimisationManager.Instance == null);
        //Debug.Log("update interval: " + ScriptOptimisationManager.Instance.nextSortIntoLODGroupsTime);
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

        //for (int i = 0; i < ; i++)
        //{
            Debug.Log("yee");
        //}
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    /*private void OnBecameVisible()
    {
        HumanoidConstraintAndAnimationOptimisationManager.Instance.AddOptimiser(this);
    }

    private void OnBecameInvisible()
    {
        HumanoidConstraintAndAnimationOptimisationManager.Instance.RemoveOptimiser(this);
    }*/

}
