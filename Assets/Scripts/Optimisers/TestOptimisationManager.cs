using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOptimisationManager : ScriptOptimisationManager
{
    [Header("Group Sorting Conditions")]
    [SerializeField] float distanceOfLOD1Start;
    float distanceOfLOD1StartSquared;

    float lastUpdateTime;

    public static TestOptimisationManager Instance;  //only declare classes which derive from this script as singleton
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

        lastUpdateTime = Time.time;
    }

    protected override void UpdateOptimisationManager()
    {
        base.UpdateOptimisationManager();

        Debug.Log("time since last Update: " + (Time.time - lastUpdateTime) + "-------------------------------------------------------");
        lastUpdateTime = Time.time;
    }

    public override void SortOptimisersIntoLODGroups()
    {
        

        Vector3 playerCameraForward = playerTransform.forward;

        Vector3 directionTowardsObject;  //maybe also have the option to optimise dased ona angle?
        float squaredDistance = 0;
        float angle = 0;
        Vector3 playerPosition = playerTransform.position;

        bool alternativeSortMode = false; //i have 2 sort algorythms, cant decide whch is better - the alternative mode seems to be less acurrate but cleaner

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
            HashSet<IScriptOptimiser>[] optimiserToRemoveFromGroups = new HashSet<IScriptOptimiser>[LODGroups.Length];
            HashSet<IScriptOptimiser>[] optimiserToAddToGroups = new HashSet<IScriptOptimiser>[LODGroups.Length];
            for (int i = 0; i < LODGroups.Length; i++)
            {
                optimiserToRemoveFromGroups[i] = new HashSet<IScriptOptimiser>();
                optimiserToAddToGroups[i] = new HashSet<IScriptOptimiser>();
            }

            #region Fill the ToRemove and ToAdd Groups

            //first check all optimisers which arent assigned to a group?
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
