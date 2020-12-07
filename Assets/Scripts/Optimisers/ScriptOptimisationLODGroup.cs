using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScriptOptimisationLODGroup
{
    #region Fields

    public string name;
    [Min(0)]
    public float updateInterval = 1 / 30;
    float nextGeneralUpdateTime;

    HashSet<IScriptOptimiser> objectsInLODGroup = new HashSet<IScriptOptimiser>();
    [Min(1)]
    [Tooltip("roughly updateInterval * 90 at 90 fps, or times 60 on 60 fps system")]
    public int updateGroups = 3;
    HashSet<IScriptOptimiser>[] groups;

    float[] groupsUpdateDelays;
    float[] nextGroupUnscaledUpdateTimes;
    public bool[] groupsUpdatedThisCycle;

    //for determining biggestAndSmallestGroup
    HashSet<IScriptOptimiser> smallestOrLargestGroup;
    int smallestOrLargestGroupSize;

    [Header("Debug")]
    public int[] debugGroupSizes;


    #endregion

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

