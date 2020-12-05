using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "TacticalPointsSceneInfo.asset", menuName = "AI/AbstractWorldRepresentation")]
public class TacticalPointsSceneInfo : ScriptableObject//, ISerializationCallbackReceiver
{
    //public Dictionary<int, PointCoverRating> pointRatings; //public so it is readable by custom editor
    //Dictionary<int, PointCastRaysContainer> raycastUsedPerPoint;

    //For Serialization

    //the keys are just indeger indexes
    //public List<int> pointRatingKeys = new List<int>(); 
    [SerializeField] List<PointCoverRating> pointRatingValues = new List<PointCoverRating>();
    //public List<int> raycastUsedKeys = new List<int>();
    [SerializeField] List<PointCastRaysContainer> raycastUsedValues = new List<PointCastRaysContainer>();

    public int testInt;

    private void OnEnable()
    {
        hideFlags = HideFlags.DontUnloadUnusedAsset;
    }

   /* public void OnBeforeSerialize()
    {
        //convert the dictionary into an array, which can be serialized
        pointRatingKeys.Clear();
        pointRatingValues.Clear();
        raycastUsedKeys.Clear();
        raycastUsedValues.Clear();

        foreach (var kvp in pointRatings)
        {
            pointRatingKeys.Add(kvp.Key);
            pointRatingValues.Add(kvp.Value);
        }

        foreach (var kvp in raycastUsedPerPoint)
        {
            raycastUsedKeys.Add(kvp.Key);
            raycastUsedValues.Add(kvp.Value);
        }

    }

    public void OnAfterDeserialize()
    {
        //convert the serialized arrays back into dictionary used ingame
        pointRatings = new Dictionary<int, PointCoverRating>();
        raycastUsedPerPoint = new Dictionary<int, PointCastRaysContainer>();

        for (int i = 0; i != Mathf.Min(pointRatingKeys.Count, pointRatingValues.Count); i++)
            pointRatings.Add(pointRatingKeys[i], pointRatingValues[i]);

        for (int i = 0; i != Mathf.Min(raycastUsedKeys.Count, raycastUsedValues.Count); i++)
            raycastUsedPerPoint.Add(raycastUsedKeys[i], raycastUsedValues[i]);

        pointRatingKeys.Clear();
        pointRatingValues.Clear();
        raycastUsedKeys.Clear();
        raycastUsedValues.Clear();

    }*/

    /*void OnGUI()
    {
        foreach (var kvp in pointRatings)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);

        foreach (var kvp in raycastUsedPerPoint)
            GUILayout.Label("Key: " + kvp.Key + " value: " + kvp.Value);
    }*/


    public void ResetInfo()
    {
        //SerializedObject so = new SerializedObject(this);
        //so.FindProperty("pointRatings").


        //pointRatings = new Dictionary<int, PointCoverRating>();
        //raycastUsedPerPoint = new Dictionary<int, PointCastRaysContainer>();

        //pointRatingKeys.Clear();
        pointRatingValues.Clear();
        //raycastUsedKeys.Clear();
        raycastUsedValues.Clear();


        EditorUtility.SetDirty(this);
    }

    public void AddPointInfo(int pointID, PointCoverRating pointCoverRating, PointCastRaysContainer pointCastRaysContainer)
    {
        Debug.Log("add point info: " + pointID);
        //pointRatings.Add(pointID, pointCoverRating);
        //raycastUsedPerPoint.Add(pointID, pointCastRaysContainer);
        pointRatingValues.Add(pointCoverRating);
        raycastUsedValues.Add(pointCastRaysContainer);


        EditorUtility.SetDirty(this);
    }

    public PointCoverRating GetCoverRating(int pointID)
    {
       // return pointRatings[pointID];
        return pointRatingValues[pointID];
    }

    public PointCastRaysContainer GetRaysCast(int pointID)
    {
        //return raycastUsedPerPoint[pointID];
        return raycastUsedValues[pointID];
    }

    public int GetDictionarySize()
    {
        //return pointRatings.Count;
        return pointRatingValues.Count;
    }

}
