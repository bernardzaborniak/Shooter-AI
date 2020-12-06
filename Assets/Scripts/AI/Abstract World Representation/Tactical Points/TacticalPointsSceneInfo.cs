using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TacticalPointsSceneInfo.asset", menuName = "AI/AbstractWorldRepresentation")]
public class TacticalPointsSceneInfo : ScriptableObject
{
    // 2 Lists are used for Serialization - Saving the data between sessions - just like lightmap
    [SerializeField] List<PointCoverRating> pointRatingValues = new List<PointCoverRating>();
    [SerializeField] List<PointCastRaysContainer> raycastUsedValues = new List<PointCastRaysContainer>();

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset; // Scriptable Object was reset when another scene was opened, this seems to prevent the resetting of this data.
    }

    public void ResetInfo()
    {
        pointRatingValues.Clear();
        raycastUsedValues.Clear();

#if UNITY_EDITOR
        EditorUtility.SetDirty(this); // Set dirty is necessary so the changes wont reset on Scene Reload or EnterPlayMode.
#endif
    }

    public void AddPointInfo(int pointID, PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer)
    {
        Debug.Log("add point info: " + pointID);
        pointRatingValues.Add(pointCoverRating);
        raycastUsedValues.Add(pointCastRaysContainer);

#if UNITY_EDITOR
        EditorUtility.SetDirty(this); // Set dirty is necessary so the changes wont reset on Scene Reload or EnterPlayMode.
#endif
    }

    public PointCoverRating GetCoverRating(int pointID)
    {
        return pointRatingValues[pointID];
    }

    public PointCastRaysContainer GetRaysCast(int pointID)
    {
        return raycastUsedValues[pointID];
    }
}
