using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIControllerOptimisationManager : ScriptOptimisationManager
{
    [Header("Group Sorting Conditions")]
    [SerializeField] float distanceOfLOD1Start;
    float distanceOfLOD1StartSquared;

    public static AIControllerOptimisationManager Instance;  //only declare classes which derive from this script as singleton
    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else
        {
            Instance = this;
        }

        distanceOfLOD1StartSquared = distanceOfLOD1Start * distanceOfLOD1Start;
    }

    public override void SortOptimisersIntoLODGroups()
    {
        Vector3 playerCameraForward = playerTransform.forward;

        Vector3 directionTowardsObject;  //maybe also have the option to optimise dased ona angle?
        float squaredDistance = 0;
        float angle = 0;
        Vector3 playerPosition = playerTransform.position;

        for (int i = 0; i < LODGroups.Length; i++)
        {
            LODGroups[i].ClearGroup();
        }

        foreach (IScriptOptimiser optimiser in optimisersRegisteredInManager)
        {
            directionTowardsObject = optimiser.GetPosition() - playerPosition;

            angle = Vector3.Angle(directionTowardsObject, playerCameraForward);
            if (angle < playerViewConeAngle)
            {
                squaredDistance = directionTowardsObject.sqrMagnitude;

                if (squaredDistance > distanceOfLOD1StartSquared)
                {
                    LODGroups[1].AddOptimiser(optimiser);
                }
                else
                {
                    LODGroups[0].AddOptimiser(optimiser);
                }
            }
            else
            {
                LODGroups[2].AddOptimiser(optimiser);
            }


        }
    }
}
