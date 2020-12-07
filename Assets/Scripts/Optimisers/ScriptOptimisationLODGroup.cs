using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScriptOptimisationLODGroup
{
    #region Fields

    public string name;
    [Min(1)]
    //public float updateInterval = 1 / 30;
    [Tooltip("Update Interval In Frames")]
    public int updateCycleFrameInterval;//= 1 / 30;
    //float nextGeneralUpdateTime;
    int nextStartUpdateCycleFrameCount;

    HashSet<IScriptOptimiser> objectsInLODGroup = new HashSet<IScriptOptimiser>();
    //[Min(1)]
    //[Tooltip("roughly updateInterval * 90 at 90 fps, or times 60 on 60 fps system")]
    int updateGroupsAmount; //= 3;
    HashSet<IScriptOptimiser>[] updateGroups;

    //float[] groupsUpdateDelays;
    int[] groupsUpdateFrameDelays;
    //float[] nextGroupUnscaledUpdateTimes;
    //public bool[] groupsUpdatedThisCycle;

    int currentUpdateCycleFrame;

    //for determining biggestAndSmallestGroup
    HashSet<IScriptOptimiser> smallestUpdateGroup;
    int smallestGroupSize;

    [Header("Debug")]
    public int[] debugGroupSizes;
    //public int[] debugGroupSizesFilledInOtherLoop; //


    #endregion

    public void SetUpLODGroup()
    {
        updateGroupsAmount = updateCycleFrameInterval;
        updateGroups = new HashSet<IScriptOptimiser>[updateGroupsAmount];

        for (int i = 0; i < updateGroups.Length; i++)
        {
            updateGroups[i] = new HashSet<IScriptOptimiser>();
        }

        //groupsUpdateFrameDelays = new float[updateGroupsAmount];
        groupsUpdateFrameDelays = new int[updateGroupsAmount];
        debugGroupSizes = new int[updateGroupsAmount];
        //debugGroupSizesFilledInOtherLoop = new int[updateGroupsAmount]; //
        //nextGroupUnscaledUpdateTimes = new float[updateGroupsAmount];
        //groupsUpdatedThisCycle = new bool[updateGroupsAmount];

        //float delay = 0;
        int individualUpdateGroupDelay = 0;

        for (int i = 0; i < groupsUpdateFrameDelays.Length; i++)
        {
            //individualUpdateGroupDelay = (updateCycleFrameInterval / updateGroupsAmount) * i;
            groupsUpdateFrameDelays[i] = individualUpdateGroupDelay;
            individualUpdateGroupDelay++;
            //nextGroupUnscaledUpdateTimes[i] = individualUpdateGroupDelay;
        }

        //nextGeneralUpdateTime = 0;
        nextStartUpdateCycleFrameCount = 0;
        currentUpdateCycleFrame = 0;
    }

    public void UpdateLODGroup()
    {
        Debug.Log("Update groudp ------------------------------------------------" + name);

        if(Time.frameCount >= nextStartUpdateCycleFrameCount)
        {
            nextStartUpdateCycleFrameCount = Time.frameCount + updateCycleFrameInterval;
            currentUpdateCycleFrame = 0;
        }

        /*if (Time.unscaledTime > nextGeneralUpdateTime)
       {
           nextGeneralUpdateTime = Time.unscaledTime + updateCycleFrameInterval;

           for (int i = 0; i < nextGroupUnscaledUpdateTimes.Length; i++)
           {
               nextGroupUnscaledUpdateTimes[i] = Time.unscaledTime + groupsUpdateFrameDelays[i];

               groupsUpdatedThisCycle[i] = false;
           }
       }*/

        Debug.Log("group size: " + updateGroups[currentUpdateCycleFrame].Count);//
        foreach (IScriptOptimiser optimiser in updateGroups[currentUpdateCycleFrame])
        {
            Debug.Log("update optimiser");
            optimiser.UpdateOptimiser();
        }


        /*for (int i = 0; i < nextGroupUnscaledUpdateTimes.Length; i++)
        {
            if (!groupsUpdatedThisCycle[i])
            {
                if (Time.unscaledTime > nextGroupUnscaledUpdateTimes[i])
                {
                    Debug.Log("group update: " + i + "---------------------");//
                    Debug.Log("group size: " + i + " : " + updateGroups[i].Count);//

                    groupsUpdatedThisCycle[i] = true;

                    int debugCounter = 0; //
                    foreach (IScriptOptimiser item in updateGroups[i])
                    {
                        debugCounter++; //
                        Debug.Log("UpdateOptimiser");
                        item.UpdateOptimiser();
                    }
                    debugGroupSizesFilledInOtherLoop[i] = debugCounter; //
                    Debug.Log("group size 2: " + i + " : " + debugCounter);//

                }
            }
        }*/

        for (int i = 0; i < updateGroups.Length; i++)
        {
            debugGroupSizes[i] = updateGroups[i].Count;
        }

        //after x seconds, change the number of things in each group if needed - if one group gets too big difference >1



        currentUpdateCycleFrame++;

       
    }

    public void AddOptimiser(IScriptOptimiser optimiser)
    {
        GetSmallestGroup().Add(optimiser);
        objectsInLODGroup.Add(optimiser);
    }

    public void RemoveOptimiser(IScriptOptimiser optimiser)
    {
        for (int i = 0; i < updateGroups.Length; i++)
        {
            updateGroups[i].Remove(optimiser);
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
        smallestGroupSize = int.MaxValue;
        //smallestOrLargestGroup = groups[0];

        for (int i = 0; i < updateGroups.Length; i++)
        {
            if (updateGroups[i].Count < smallestGroupSize)
            {
                smallestGroupSize = updateGroups[i].Count;
                smallestUpdateGroup = updateGroups[i];
            }
        }

        return smallestUpdateGroup;
    }

    public bool ContainsOptimiser(IScriptOptimiser optimiser)
    {
        return objectsInLODGroup.Contains(optimiser);
    }

    public void ClearGroup()
    {
        for (int i = 0; i < updateGroups.Length; i++)
        {
            updateGroups[i].Clear();
        }
        objectsInLODGroup.Clear();
    }

    public int GetLODGroupSize()
    {
        return objectsInLODGroup.Count;
    }

}

