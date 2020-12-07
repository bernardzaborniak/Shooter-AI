using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ScriptOptimisationManager : MonoBehaviour
{
    #region Fields

    [Tooltip("LOD groups are assigned based on distance and angle from the playerCamera")]
    public Transform playerTransform;
    public float playerViewConeAngle;

    //have a group of all optimisers this manager manages
    protected HashSet<IScriptOptimiser> optimisersRegisteredInManager = new HashSet<IScriptOptimiser>();

    // [Tooltip("every x seconds all optimisers are sorted into LOD groups based on distance or angle from camera")]
    // public float sortIntoLODGroupsInterval;
    [Tooltip("every x frames all optimisers are sorted into LOD groups based on distance or angle from camera - for 90 fps - write 90 in this field")]
    public int sortIntoLODGroupsFrameInterval;
    protected int nextSortIntoLODGroupsFrameCount;


    [Header("LOD Groups")]
    [Space(10)]
    public ScriptOptimisationLODGroup[] LODGroups;

    [Header("Debug")]
    public int[] LODGroupSizes;

    #endregion

    // Although this class looks like a singleton, it is never used, only classes deriving from this class are used.
    // Only declare classes which derive from this script as singleton.

    private void Start()
    {
        SetUpOptimisationManager();
    }

    protected virtual void SetUpOptimisationManager()
    {

        for (int i = 0; i < LODGroups.Length; i++)
        {
            LODGroups[i].SetUpLODGroup();
        }

        //nextSortIntoLODGroupsTime = 0.01f;
        nextSortIntoLODGroupsFrameCount = 0;
        LODGroupSizes = new int[LODGroups.Length];
    }

    private void Update()
    {
        UpdateOptimisationManager();
    }

    protected virtual void UpdateOptimisationManager()
    {
        for (int i = 0; i < LODGroups.Length; i++)
        {
            LODGroups[i].UpdateLODGroup();
            LODGroupSizes[i] = LODGroups[i].GetLODGroupSize();
        }

        /* if(Time.unscaledTime > nextSortIntoLODGroupsTime)
         {
             nextSortIntoLODGroupsTime = Time.unscaledTime + sortIntoLODGroupsInterval;
             SortOptimisersIntoLODGroups();
         }*/
        if (Time.frameCount > nextSortIntoLODGroupsFrameCount)
        {
            nextSortIntoLODGroupsFrameCount = Time.frameCount + sortIntoLODGroupsFrameInterval;
            SortOptimisersIntoLODGroups();
        }

    }

    public virtual void AddOptimiser(IScriptOptimiser optimiser)
    {
        optimisersRegisteredInManager.Add(optimiser);
    }

    public virtual void RemoveOptimiser(IScriptOptimiser optimiser)
    {
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

    public virtual void SortOptimisersIntoLODGroups()
    {

    }

}

