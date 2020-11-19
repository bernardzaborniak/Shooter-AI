using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScriptOptimisationLODGroup
{
    public string name;
    public float updateInterval = 1 / 30;
    public int updateGroups = 3;
    public float[] groupsUpdateDelays;
    float nextGeneralUpdateTime;
    public float[] nextGroupUnscaledUpdateTimes;
    public bool[] groupsUpdatedThisCycle;
    //public float[] lastGroupUnscaledUpdateTimes;

    public int[] debugGroupSizes;

    HashSet<HumanoidConstraintAndAnimationOptimiser>[] groups;
    HashSet<HumanoidConstraintAndAnimationOptimiser> objectsInLODGroup = new HashSet<HumanoidConstraintAndAnimationOptimiser>();


    //for determining biggestAndSmallestGroup
    HashSet<HumanoidConstraintAndAnimationOptimiser> smallestOrLargestGroup;
    int smallestOrLargestGroupSize;

    public void SetUpLODGroup()
    {
        groups = new HashSet<HumanoidConstraintAndAnimationOptimiser>[updateGroups];

        for (int i = 0; i < groups.Length; i++)
        {
            groups[i] = new HashSet<HumanoidConstraintAndAnimationOptimiser>();
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
                    foreach (HumanoidConstraintAndAnimationOptimiser item in groups[i])
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

    public void AddOptimiser(HumanoidConstraintAndAnimationOptimiser optimiser)
    {
        GetSmallestGroup().Add(optimiser);
        objectsInLODGroup.Add(optimiser);
    }

    public void RemoveOptimiser(HumanoidConstraintAndAnimationOptimiser optimiser)
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

    HashSet<HumanoidConstraintAndAnimationOptimiser> GetSmallestGroup()
    {
        smallestOrLargestGroupSize = int.MaxValue;

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

    public bool ContainsOptimiser(HumanoidConstraintAndAnimationOptimiser optimiser)
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



    HashSet<HumanoidConstraintAndAnimationOptimiser> obtimisersRegisteredInManager = new HashSet<HumanoidConstraintAndAnimationOptimiser>();

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

    public void AddOptimiser(HumanoidConstraintAndAnimationOptimiser optimiser)
    {
        //groups[0].Add(optimiser);
        //LOD0.AddOptimiser(optimiser);
        obtimisersRegisteredInManager.Add(optimiser);
    }

    public void RemoveOptimiser(HumanoidConstraintAndAnimationOptimiser optimiser)
    {
        //LOD0.RemoveOptimiser(optimiser);
        obtimisersRegisteredInManager.Remove(optimiser);

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

        foreach (HumanoidConstraintAndAnimationOptimiser optimiser in obtimisersRegisteredInManager)
        {
            

            directionTowardsObject = optimiser.transform.position - playerPosition;

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
