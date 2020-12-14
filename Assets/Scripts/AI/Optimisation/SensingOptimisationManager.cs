using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Manages Update Times of all AIC_sensing components with the SensingOptimiser
public class SensingOptimisationManager : ScriptOptimisationManager
{
    #region Fields

    [Header("Group Sorting Conditions")]
    [SerializeField] float distanceOfLOD1Start;
    float distanceOfLOD1StartSquared;

    public static SensingOptimisationManager Instance;  //only declare classes which derive from this script as singleton

    #region Variables chached to prevent garbage collection

    // ----- Used inside SortOptimisersintoLODGroups()---------

    Vector3 playerCameraForward;
    Vector3 directionTowardsObject; 
    float squaredDistance;
    float angle;
    Vector3 playerPosition;
    bool alternativeSortMode;

    HashSet<IScriptOptimiser>[] optimiserToRemoveFromGroups;
    HashSet<IScriptOptimiser>[] optimiserToAddToGroups;

    // ---------------------------------

    #endregion

    #endregion


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

        optimiserToRemoveFromGroups = new HashSet<IScriptOptimiser>[LODGroups.Length];
        optimiserToAddToGroups = new HashSet<IScriptOptimiser>[LODGroups.Length];
        for (int i = 0; i < LODGroups.Length; i++)
        {
            optimiserToRemoveFromGroups[i] = new HashSet<IScriptOptimiser>();
            optimiserToAddToGroups[i] = new HashSet<IScriptOptimiser>();
        }
    }


    public override void SortOptimisersIntoLODGroups()
    {
         playerCameraForward = playerTransform.forward;
         squaredDistance = 0;
         angle = 0;
         playerPosition = playerTransform.position;
         alternativeSortMode = false; //i have 2 sort algorythms, cant decide whch is better - the alternative mode seems to be less acurrate but cleaner

        if (alternativeSortMode)
        {
            #region this method is simpler to write but produces more unnacurrate results
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
            #endregion
        }
        else
        {
            for (int i = 0; i < LODGroups.Length; i++)
            {
                optimiserToRemoveFromGroups[i].Clear();
                optimiserToAddToGroups[i].Clear();
            }

            #region Fill the ToRemove and ToAdd Groups

            // First check all optimisers which arent assigned to a group?
            foreach (IScriptOptimiser optimiser in optimisersRegisteredButNotAddedToALODGroupYet)
            {
                directionTowardsObject = optimiser.GetPosition() - playerPosition;

                angle = Vector3.Angle(directionTowardsObject, playerCameraForward);
                if (angle < playerViewConeAngle)
                {
                    squaredDistance = directionTowardsObject.sqrMagnitude;

                    if (squaredDistance > distanceOfLOD1StartSquared)
                    {
                        optimiserToAddToGroups[1].Add(optimiser);
                    }
                    else
                    {
                        optimiserToAddToGroups[0].Add(optimiser);
                    }
                }
                else
                {
                    optimiserToAddToGroups[2].Add(optimiser);
                }
            }
            optimisersRegisteredButNotAddedToALODGroupYet.Clear();

            foreach (IScriptOptimiser optimiser in LODGroups[0].GetObjectsInGroup())
            {
                directionTowardsObject = optimiser.GetPosition() - playerPosition;

                angle = Vector3.Angle(directionTowardsObject, playerCameraForward);
                if (angle < playerViewConeAngle)
                {
                    squaredDistance = directionTowardsObject.sqrMagnitude;

                    if (squaredDistance > distanceOfLOD1StartSquared)
                    {
                        optimiserToRemoveFromGroups[0].Add(optimiser);
                        optimiserToAddToGroups[1].Add(optimiser);
                    }
                }
                else
                {
                    optimiserToRemoveFromGroups[0].Add(optimiser);
                    optimiserToAddToGroups[2].Add(optimiser);
                }
            }


            foreach (IScriptOptimiser optimiser in LODGroups[1].GetObjectsInGroup())
            {
                directionTowardsObject = optimiser.GetPosition() - playerPosition;

                angle = Vector3.Angle(directionTowardsObject, playerCameraForward);
                if (angle < playerViewConeAngle)
                {
                    squaredDistance = directionTowardsObject.sqrMagnitude;

                    if (squaredDistance < distanceOfLOD1StartSquared)
                    {
                        optimiserToRemoveFromGroups[1].Add(optimiser);
                        optimiserToAddToGroups[0].Add(optimiser);
                    }
                }
                else
                {
                    optimiserToRemoveFromGroups[1].Add(optimiser);
                    optimiserToAddToGroups[2].Add(optimiser);
                }
            }

            foreach (IScriptOptimiser optimiser in LODGroups[2].GetObjectsInGroup())
            {
                directionTowardsObject = optimiser.GetPosition() - playerPosition;

                angle = Vector3.Angle(directionTowardsObject, playerCameraForward);
                if (angle < playerViewConeAngle)
                {
                    squaredDistance = directionTowardsObject.sqrMagnitude;

                    if (squaredDistance > distanceOfLOD1StartSquared)
                    {

                        optimiserToRemoveFromGroups[2].Add(optimiser);
                        optimiserToAddToGroups[1].Add(optimiser);
                    }
                    else
                    {
                        optimiserToRemoveFromGroups[2].Add(optimiser);
                        optimiserToAddToGroups[0].Add(optimiser);
                    }
                }

            }

            #endregion


            for (int i = 0; i < LODGroups.Length; i++)
            {
                foreach (IScriptOptimiser optimiser in optimiserToRemoveFromGroups[i])
                {
                    LODGroups[i].RemoveOptimiser(optimiser);
                }

                foreach (IScriptOptimiser optimiser in optimiserToAddToGroups[i])
                {
                    LODGroups[i].AddOptimiser(optimiser);
                }
            }
        }

    }
}
