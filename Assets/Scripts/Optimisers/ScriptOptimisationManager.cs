﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptOptimisationLODGroup
{
    public string name;
    [Min(0)]
    public float updateInterval = 1 / 30;
    [Min(1)]
    public int updateGroups = 3;
    public float[] groupsUpdateDelays;
    float nextGeneralUpdateTime;
    public float[] nextGroupUnscaledUpdateTimes;
    public bool[] groupsUpdatedThisCycle;
    //public float[] lastGroupUnscaledUpdateTimes;

    public int[] debugGroupSizes;

    HashSet<IScriptOptimiser>[] groups;
    HashSet<IScriptOptimiser> objectsInLODGroup = new HashSet<IScriptOptimiser>();


    //for determining biggestAndSmallestGroup
    HashSet<IScriptOptimiser> smallestOrLargestGroup;
    int smallestOrLargestGroupSize;

    public void SetUpLODGroup()
    {
        groups = new HashSet<IScriptOptimiser>[updateGroups];

        for (int i = 0; i < groups.Length; i++)
        {
            groups[i] = new HashSet<IScriptOptimiser>();
        }

        groupsUpdateDelays = new float[updateGroups];
        debugGroupSizes = new int[updateGroups];
        nextGroupUnscaledUpdateTimes = new float[updateGroups];
        groupsUpdatedThisCycle = new bool[updateGroups];
        //lastGroupUnscaledUpdateTimes = new float[updateGroups];

        float delay = 0;

        for (int i = 0; i < groupsUpdateDelays.Length; i++)
        {
            delay = (updateInterval / updateGroups) * i;
            groupsUpdateDelays[i] = delay;

            nextGroupUnscaledUpdateTimes[i] = delay;
        }

        nextGeneralUpdateTime = 0;
    }

    public void UpdateLODGroup()
    {
        if (Time.unscaledTime > nextGeneralUpdateTime)
        {
            nextGeneralUpdateTime = Time.unscaledTime + updateInterval;

            for (int i = 0; i < nextGroupUnscaledUpdateTimes.Length; i++)
            {
                nextGroupUnscaledUpdateTimes[i] = Time.unscaledTime + groupsUpdateDelays[i];

                groupsUpdatedThisCycle[i] = false;
            }
        }

        for (int i = 0; i < nextGroupUnscaledUpdateTimes.Length; i++)
        {
            if (!groupsUpdatedThisCycle[i])
            {
                if (Time.unscaledTime > nextGroupUnscaledUpdateTimes[i])
                {
                    groupsUpdatedThisCycle[i] = true;
                    //Debug.Log("update group: " + i);
                    foreach (IScriptOptimiser item in groups[i])
                    {
                        item.UpdateOptimiser();
                    }
                }
            }
        }

        for (int i = 0; i < groups.Length; i++)
        {
            debugGroupSizes[i] = groups[i].Count;

            /*if(Time.unscaledTime > nextGroupUnscaledUpdateTimes[i])
            {
                nextGroupUnscaledUpdateTimes[i] = Time.unscaledTime + updateInterval;  //will the 3 groupupdate times in the end be at the same time becasue of this frame error here?
                //nextGroupUnscaledUpdateTimes[i] = Time.unscaledTime - lastUpdateTIme + updateInterval;  //maybe something like this this couldbe a solution to this problem?

                foreach (HumanoidConstraintAndAnimationOptimiser item in groups[i])
                {
                    item.UpdateOptimiser();
                }
            }*/

        }


        //after x seconds, change the number of things in each group if needed - if one group gets too big difference >1
    }

    public void AddOptimiser(IScriptOptimiser optimiser)
    {
        GetSmallestGroup().Add(optimiser);
        objectsInLODGroup.Add(optimiser);
    }

    public void RemoveOptimiser(IScriptOptimiser optimiser)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i].Remove(optimiser);
        }
        objectsInLODGroup.Remove(optimiser);

       
    }


   /* HashSet<HumanoidConstraintAndAnimationOptimiser> GetBiggestGroup()
    {
        smallestOrLargestGroupSize = 0;

        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i].Count > smallestOrLargestGroupSize)
            {
                smallestOrLargestGroupSize = groups[i].Count;
                smallestOrLargestGroup = groups[i];
            }
        }

        return smallestOrLargestGroup;
    }*/

    HashSet<IScriptOptimiser> GetSmallestGroup()
    {
        smallestOrLargestGroupSize = int.MaxValue;
        //smallestOrLargestGroup = groups[0];

        for (int i = 0; i < groups.Length; i++)
        {
            if (groups[i].Count < smallestOrLargestGroupSize)
            {
                smallestOrLargestGroupSize = groups[i].Count;
                smallestOrLargestGroup = groups[i];
            }
        }

        return smallestOrLargestGroup;
    }

    public bool ContainsOptimiser(IScriptOptimiser optimiser)
    {
        return objectsInLODGroup.Contains(optimiser);
    }

    public void ClearGroup()
    {
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i].Clear();
        }
        objectsInLODGroup.Clear();
    }

    public int GetLODGroupSize()
    {
        return objectsInLODGroup.Count;
    }

}




public class ScriptOptimisationManager : MonoBehaviour
{
    public Transform playerTransform;
    public float playerViewConeAngle;

    public ScriptOptimisationLODGroup[] LODGroups;
    //public HumanoidConstraintAndAnimationOptimisationLODGroup LOD1;
    public float distanceOfLOD1Start;
    float distanceOfLOD1StartSquared;



    HashSet<IScriptOptimiser> optimisersRegisteredInManager = new HashSet<IScriptOptimiser>();

    public float sortIntoLODGroupsInterval;
    public float nextSortIntoLODGroupsTime;

    public int[] LODGroupSizes;



    public static ScriptOptimisationManager Instance;
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

        for (int i = 0; i < LODGroups.Length; i++)
        {
            LODGroups[i].SetUpLODGroup();
        }

        nextSortIntoLODGroupsTime = 0.01f;
        LODGroupSizes = new int[LODGroups.Length];
    }



    void Start()
    {
        distanceOfLOD1StartSquared = distanceOfLOD1Start * distanceOfLOD1Start;
    }

    void Update()
    {
        for (int i = 0; i < LODGroups.Length; i++)
        {
            LODGroups[i].UpdateLODGroup();
            LODGroupSizes[i] = LODGroups[i].GetLODGroupSize();
        }

        if(Time.unscaledTime > nextSortIntoLODGroupsTime)
        {
            nextSortIntoLODGroupsTime = Time.unscaledTime + sortIntoLODGroupsInterval;
            SortOptimisersIntoLODGroups();
        }
    }

    public void AddOptimiser(IScriptOptimiser optimiser)
    {
        //groups[0].Add(optimiser);
        //LOD0.AddOptimiser(optimiser);
        optimisersRegisteredInManager.Add(optimiser);
    }

    public void RemoveOptimiser(IScriptOptimiser optimiser)
    {
        //LOD0.RemoveOptimiser(optimiser);
        optimisersRegisteredInManager.Remove(optimiser);

        for (int i = 0; i < LODGroups.Length; i++)
        {
            if (LODGroups[i].ContainsOptimiser(optimiser))
            {
                LODGroups[i].RemoveOptimiser(optimiser);
                return;
            }
        }
    }




    public void SortOptimisersIntoLODGroups()
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
            if(angle < playerViewConeAngle)
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
