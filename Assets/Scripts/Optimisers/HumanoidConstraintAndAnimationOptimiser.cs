using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidConstraintAndAnimationOptimiser : MonoBehaviour
{
    //float max = 2;
    //float min = 0.5f;
   float current;
    float speed = 1;

    // Start is called before the first frame update
    private void OnEnable()
    {
       // Debug.Log("Instance == null: " + HumanoidConstraintAndAnimationOptimisationManager.Instance == null);
       // Debug.Log("update interval: " + HumanoidConstraintAndAnimationOptimisationManager.Instance.updateInterval);
        HumanoidConstraintAndAnimationOptimisationManager.Instance.AddConstraintObject(this);
    }

    private void OnDisable()
    {
        HumanoidConstraintAndAnimationOptimisationManager.Instance.RemoveConstraintObject(this);

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
}
