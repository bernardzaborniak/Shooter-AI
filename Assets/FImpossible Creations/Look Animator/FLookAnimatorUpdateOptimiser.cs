using FIMSpace.FLook;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLookAnimatorUpdateOptimiser : MonoBehaviour
{
    public FLookAnimator lookAnimator;
    public bool updateAutomaticlyInLateUpdate;

    private void Start()
    {
        if (updateAutomaticlyInLateUpdate)
        {
            lookAnimator.updateAutomaticlyInLateUpdate = true;
        }
        else
        {
            lookAnimator.updateAutomaticlyInLateUpdate = false;
        }
    }

    /*void Update()
    {
        if (updateAutomaticlyInLateUpdate)
        {
            lookAnimator.updateAutomaticlyInLateUpdate = true;
        }
        else
        {
            lookAnimator.updateAutomaticlyInLateUpdate = false;
        }
    }*/
}
