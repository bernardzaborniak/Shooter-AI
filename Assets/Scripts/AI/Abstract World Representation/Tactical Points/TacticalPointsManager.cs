using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class TacticalPointsManager : MonoBehaviour
{
    #region Fields

    [Tooltip("For Now this scriptable object needs to be created by hand and referenced here - I create it in the scene folder just where lighting data and similar are saved")]
    public TacticalPointsSceneInfo tacticalPointsSceneInfo;

    public GameObject openFieldPointPrefab;
    public float standingCoverHeight;
    public float crouchedCoverHeight;
    [Tooltip("The Distance of the navmesh.SamplePosition distance")]
    public float maxSnapDistanceToNavmesh;
    public int raycastsPerCoverRating;
    public LayerMask raycastLayerMask;


#if UNITY_EDITOR
    public HashSet<TacticalPointsGeneratorBox> tacticalPointGenerators = new HashSet<TacticalPointsGeneratorBox>();
#endif
    public HashSet<TacticalPoint> tacticalPoints = new HashSet<TacticalPoint>();

    [Tooltip("If an distance equals Infinity, we take this distance instead for better calculation")]
    public float maxCoverRayLength;

    // Singleton Instance
    public static TacticalPointsManager Instance;

    // For Optimising Update Times in edit mode
    public float updateRatingsInEditModeInterval = 2;
    float nextUpdateRatingsTime;



    #endregion

    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        Instance = this;
    }

    private void Start()
    {
        UpdatePointRatings();
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (!Application.isPlaying)
        {
            // Sets the references for new instantiated CoverShootPoint children of CoverPoint
            foreach (TacticalPoint point in tacticalPoints)
            {
                point.UpdateCoverShootPoints();
            }

            // Update the ratings every x seconds, otherwise it has to be done with a button after reloading, which is annoying.
            if(EditorApplication.timeSinceStartup > nextUpdateRatingsTime)
            {
                nextUpdateRatingsTime = (float)EditorApplication.timeSinceStartup + updateRatingsInEditModeInterval;
                UpdatePointRatings();
            }
        }
    }
#endif

    #region Add & Remove From Managed Collections

#if UNITY_EDITOR
    public void AddTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Add(generator);
    }

    public void RemoveTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Remove(generator);
    }
#endif

    public void AddTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Add(point);
    }

    public void RemoveTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Remove(point);
    }

    #endregion


#if UNITY_EDITOR
    public void GenerateAll()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.Generate();
        }
    }
#endif


#if UNITY_EDITOR
    public void BakeAllCoverRatings()
    {
        tacticalPointsSceneInfo.ResetInfo();

        int id = 0;
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.SetPointReferenceID(id);
            id++;

            PointCoverRating pointCoverRating = new PointCoverRating();
            PointCastRaysContainer pointCastRaysContainer = new PointCastRaysContainer();

            point.BakeCoverRatings(ref pointCoverRating, ref pointCastRaysContainer, crouchedCoverHeight, standingCoverHeight, raycastsPerCoverRating, raycastLayerMask, maxCoverRayLength);

            tacticalPointsSceneInfo.AddPointInfo(point.GetPointReferenceID(), pointCoverRating, pointCastRaysContainer);
        }
    }
#endif

    public void UpdatePointRatings()
    {
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.UpdateRatings(tacticalPointsSceneInfo.GetCoverRating(point.GetPointReferenceID()), tacticalPointsSceneInfo.GetRaysCast(point.GetPointReferenceID()));
        }
    }

    public void ResetAllPointRotations()
    {
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.ResetRotation();
        }
    }

}
