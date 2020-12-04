using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TacticalPointsManager : MonoBehaviour
{
    public TacticalPointsSceneInfo tacticalPointsSceneInfo;

    public GameObject openFieldPointPrefab;
    public float standingCoverHeight;
    public float crouchedCoverHeight;
    public float maxSnapDistanceToNavmesh;
   // public int distanceRaycastsPerPoint;
    public int raycastsPerCoverRating;
    //public float maxRaycastDistance;
    public LayerMask raycastLayerMask;

    public HashSet<TacticalPointsGeneratorBox> tacticalPointGenerators = new HashSet<TacticalPointsGeneratorBox>();
    public HashSet<TacticalPoint> tacticalPoints = new HashSet<TacticalPoint>();

    [Tooltip("If an distance equals Infinity, we take this distance instead for bettr calculation")]
    public float maxCoverRayLength;

   // public TacticalPoint[] pointes;


    public static TacticalPointsManager Instance;

    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        Debug.Log("Manager on Enable: " + gameObject);
        Instance = this;

        //Get All Points
        //tacticalPoints.Clear();

        //TacticalPoint[] points = FindObjectsOfType<TacticalPoint>();
        //pointes = points;

        //Get All Point Generators
    }

    private void Start()
    {
        UpdatePointRatings();
    }

    private void Update()
    {
        if(Time.frameCount%10 == 0)
        {
            if (!Application.isPlaying)
            {
                foreach (TacticalPoint point in tacticalPoints)
                {
                    point.UpdateCoverShootPoints();
                }
            }
        } 
    }


    public void AddTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Add(generator);
    }

    public void RemoveTacticalPointsGeneratorBox(TacticalPointsGeneratorBox generator)
    {
        tacticalPointGenerators.Remove(generator);
    }

    public void AddTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Add(point);
    }

    public void RemoveTacticalPoint(TacticalPoint point)
    {
        tacticalPoints.Remove(point);
    }

    public void GenerateAll()
    {
        foreach (TacticalPointsGeneratorBox generator in tacticalPointGenerators)
        {
            generator.Generate();
        }
    }

    public void BakeAllCoverRatings()
    {
        Debug.Log("manager  bake all cover ratings called");
        tacticalPointsSceneInfo.ResetInfo();

        int id = 0;
        foreach (TacticalPoint point in tacticalPoints)
        {
            point.pointReferenceID = id;
            id++;

            PointCoverRating pointCoverRating = new PointCoverRating();
            PointCastRaysContainer pointCastRaysContainer = new PointCastRaysContainer();

            point.BakeCoverRatings(ref pointCoverRating, ref pointCastRaysContainer, crouchedCoverHeight, standingCoverHeight, raycastsPerCoverRating, raycastLayerMask, maxCoverRayLength);

            tacticalPointsSceneInfo.AddPointInfo(point.pointReferenceID, pointCoverRating, pointCastRaysContainer);
        }
    }

    public void UpdatePointRatings()
    {
        Debug.Log("manager UpdatePointRatings");

        foreach (TacticalPoint point in tacticalPoints)
        {
            point.UpdateRatings(tacticalPointsSceneInfo.GetCoverRating(point.pointReferenceID), tacticalPointsSceneInfo.GetRaysCast(point.pointReferenceID));
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
