using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanoidConstraintAndAnimationOptimisationManager : MonoBehaviour
{
    public float updateInterval = 1/30;
    public int updateGroups = 3;
    public float[] groupsUpdateDelays;
    float nextGeneralUpdateTime;
    public float[] nextGroupUnscaledUpdateTimes;
    public bool[] groupsUpdatedThisCycle;
    //public float[] lastGroupUnscaledUpdateTimes;

    public int[] debugGroupSizes;

    HashSet<HumanoidConstraintAndAnimationOptimiser>[] groups;


    //for determining biggestAndSmallestGroup
    HashSet<HumanoidConstraintAndAnimationOptimiser> smallestOrLargestGroup;
    int smallestOrLargestGroupSize;

    public static HumanoidConstraintAndAnimationOptimisationManager Instance;
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

            nextGroupUnscaledUpdateTimes[i] =  delay;
        }

        nextGeneralUpdateTime = 0;
    }



    void Start()
    {
        
    }

    void Update()
    {
        if(Time.unscaledTime > nextGeneralUpdateTime)
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
                if(Time.unscaledTime > nextGroupUnscaledUpdateTimes[i])
                {
                    groupsUpdatedThisCycle[i] = true;

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
        
    }

    public void AddConstraintObject(HumanoidConstraintAndAnimationOptimiser optimiser)
    {
        //groups[0].Add(optimiser);
        GetSmallestGroup().Add(optimiser);
    }



    public void RemoveConstraintObject(HumanoidConstraintAndAnimationOptimiser optimiser)
    {
        for (int i = 0; i < groups.Length; i++)
        {
            groups[i].Remove(optimiser);
        }
    }


    HashSet<HumanoidConstraintAndAnimationOptimiser> GetBiggestGroup()
    {
        smallestOrLargestGroupSize = 0;

        for (int i = 0; i < groups.Length; i++)
        {
            if(groups[i].Count > smallestOrLargestGroupSize)
            {
                smallestOrLargestGroupSize = groups[i].Count;
                smallestOrLargestGroup = groups[i];
            }
        }

        return smallestOrLargestGroup;
    }

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

}
