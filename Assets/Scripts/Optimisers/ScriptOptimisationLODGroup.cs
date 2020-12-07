using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ScriptOptimisationLODGroup
{
    #region Fields

    public string name;
    [Min(1)]
    [Tooltip("Update Interval In Frames")]
    public int updateCycleFrameInterval;//= 1 / 30;
    int nextStartUpdateCycleFrameCount;

    HashSet<IScriptOptimiser> objectsInLODGroup = new HashSet<IScriptOptimiser>();
    //[Tooltip("roughly updateInterval * 90 at 90 fps, or times 60 on 60 fps system")]
    int updateGroupsAmount; //= 3;
    HashSet<IScriptOptimiser>[] updateGroups;

    int[] groupsUpdateFrameDelays;

    int currentUpdateCycleFrame;

    //for determining biggestAndSmallestGroup
    HashSet<IScriptOptimiser> smallestUpdateGroup;
    int smallestGroupSize;

    [Header("Debug")]
    public int[] debugGroupSizes;


    #endregion

    public void SetUpLODGroup()
    {
        updateGroupsAmount = updateCycleFrameInterval;
        updateGroups = new HashSet<IScriptOptimiser>[updateGroupsAmount];

        for (int i = 0; i < updateGroups.Length; i++)
        {
            updateGroups[i] = new HashSet<IScriptOptimiser>();
        }

        groupsUpdateFrameDelays = new int[updateGroupsAmount];
        debugGroupSizes = new int[updateGroupsAmount];

        int individualUpdateGroupDelay = 0;

        for (int i = 0; i < groupsUpdateFrameDelays.Length; i++)
        {
            groupsUpdateFrameDelays[i] = individualUpdateGroupDelay;
            individualUpdateGroupDelay++;
        }

        nextStartUpdateCycleFrameCount = 0;
        currentUpdateCycleFrame = 0;
    }

    public void UpdateLODGroup()
    {

        if(Time.frameCount >= nextStartUpdateCycleFrameCount)
        {
            nextStartUpdateCycleFrameCount = Time.frameCount + updateCycleFrameInterval;
            currentUpdateCycleFrame = 0;
        }



        //Debug.Log("group size: " + updateGroups[currentUpdateCycleFrame].Count + " currentUpdateCycleFrame: " + currentUpdateCycleFrame);//
        foreach (IScriptOptimiser optimiser in updateGroups[currentUpdateCycleFrame])
        {
            //Debug.Log("update optimiser");
            optimiser.UpdateOptimiser();
        }


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


    HashSet<IScriptOptimiser> GetSmallestGroup()
    {
        smallestGroupSize = int.MaxValue;

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

