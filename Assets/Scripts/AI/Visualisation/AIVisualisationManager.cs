using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class AIVisualisationManager : MonoBehaviour
{
    public Transform camTransform;
    public TacticalPointsManager tacticalPointsManager;
    //public HashSet<TacticalPointVisualiser> tacticalPointVisualisers = new HashSet<TacticalPointVisualiser>();

    [System.Serializable]
    public class Settings
    {
        [Header("Tactical Points")]
        public bool showOpenFieldPoints;
        public bool showCoverPoints;
        public bool showCoverShootPoints;
        [Space(5)]
        public bool showCoverDistanceRating;
        [ConditionalHide("showCoverDistanceRating")]
        public bool showCoverDistanceRatingNumbers;
        [Space(5)]
        public bool showCoverQualityRating;
        [ConditionalHide("showCoverQualityRating")]
        public bool showCoverQualityRatingNumbers;

    }

    public Settings settings;


    [Header("Visualisation Optimisation")]
    [Space(5)]
    //public float ratingTextCullDistance;
    //float ratingTextCullDistanceSquared;

    public float ratingRingCullDistance;
    float ratingRingCullDistanceSquared;

    public int coverRatingRingPoolSize;
    public GameObject tacticalPointVisualiserPrefab;
    Queue<TacticalPointVisualiser> tacticalPointVisualisersNotInUse;
    HashSet<TacticalPointVisualiser> tacticalPointVisualisersInUse;
    HashSet<TacticalPoint> tacticalPointsBeingCurrentlyVisualised;


    #region Singleton Code
    //public static AIVisualisationManager Instance;

    //void Awake()
    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        tacticalPointVisualisersNotInUse = new Queue<TacticalPointVisualiser>();
        tacticalPointVisualisersInUse = new HashSet<TacticalPointVisualiser>();
        tacticalPointsBeingCurrentlyVisualised = new HashSet<TacticalPoint>();

        //delete old ones
        HashSet<GameObject> visualisersToDestroy = new HashSet<GameObject>();
        foreach (TacticalPointVisualiser visualiser in transform) //theyre all children
        {
            visualisersToDestroy.Add(visualiser.gameObject);
        }
        foreach (GameObject visualiser in visualisersToDestroy) //theyre all children
        {
            DestroyImmediate(visualiser);
        }


        //Instantiate new Visualisers
        for (int i = 0; i < coverRatingRingPoolSize; i++)
        {
            TacticalPointVisualiser obj = Instantiate(tacticalPointVisualiserPrefab, transform).GetComponent<TacticalPointVisualiser>();
            obj.gameObject.SetActive(false);
            tacticalPointVisualisersNotInUse.Enqueue(obj);
        }

        // Square Distances
        //ratingTextCullDistanceSquared = ratingTextCullDistance * ratingTextCullDistance;
        ratingRingCullDistanceSquared = ratingRingCullDistance * ratingRingCullDistance;
    }
    #endregion

  

    void UpdateVisualisersShown(bool inSceneView)
    {
        Quaternion camRot = camTransform.rotation;
        Vector3 camPos;
        if (inSceneView)
        {
             camPos = SceneView.lastActiveSceneView.camera.transform.position;
        }
        else
        {
            camPos = camTransform.position;
        }

        float currentDistanceSquared;
        TacticalPointVisualiser visualiser;


        //1. go through visualisers in use and check if they shouldnt be enqueued again 
        HashSet<TacticalPointVisualiser> visualisersToRemoveFromUse = new HashSet<TacticalPointVisualiser>();
       // Debug.Log("tacticalPointVisualisersInUse: " + tacticalPointVisualisersInUse.Count);
        //Debug.Log("tacticalPointsBeingCurrentlyVisualised: " + tacticalPointsBeingCurrentlyVisualised.Count);
        foreach (TacticalPointVisualiser visualiserInUse in tacticalPointVisualisersInUse)
        {
            currentDistanceSquared = (visualiserInUse.transform.position - camPos).sqrMagnitude;

            if (currentDistanceSquared > ratingRingCullDistanceSquared)
            {
                visualisersToRemoveFromUse.Add(visualiserInUse);
            }
            else
            {
                visualiserInUse.UpdateVisualiser(camRot, settings);
            }
        }

        foreach (TacticalPointVisualiser visualiserToRemoveFromUse in visualisersToRemoveFromUse)
        {
            visualiserToRemoveFromUse.gameObject.SetActive(false);

            tacticalPointVisualisersInUse.Remove(visualiserToRemoveFromUse);
            tacticalPointsBeingCurrentlyVisualised.Remove(visualiserToRemoveFromUse.pointToVisualise);

            tacticalPointVisualisersNotInUse.Enqueue(visualiserToRemoveFromUse); //bring it back into the queue which can be used
        }

        //Debug.Log("visualiser update: tacticalPointsManager.tacticalPoints size" + tacticalPointsManager.tacticalPoints.Count);
        foreach (TacticalPoint point in tacticalPointsManager.tacticalPoints)
        {
            if (!tacticalPointsBeingCurrentlyVisualised.Contains(point))
            {
                currentDistanceSquared = (point.transform.position - camPos).sqrMagnitude;

                if (tacticalPointVisualisersNotInUse.Count > 0)
                {
                    if (currentDistanceSquared < ratingRingCullDistanceSquared)
                    {
                        // Glue an Visualiser to another Point 

                        //manage collections
                        visualiser = tacticalPointVisualisersNotInUse.Dequeue();         
                        tacticalPointsBeingCurrentlyVisualised.Add(point);
                        tacticalPointVisualisersInUse.Add(visualiser);

                        //logic & graphic update
                        visualiser.pointToVisualise = point;
                        visualiser.UpdateVisualiser(camRot, settings);
                        visualiser.UpdateCoverRingMaterialsAndText();

                        //positioning & activating
                        visualiser.transform.SetParent(point.transform);
                        visualiser.transform.localPosition = Vector3.zero;
                        visualiser.gameObject.SetActive(true);

                    }
                    else
                    {
                        //visualiser.UpdateVisualiser(camRot, settings, true);
                    }
                }
            }
            
           

        }
    }


    void Update()
    {
        if (Application.isPlaying)
        {
            if(Time.frameCount % 12 == 0)
            {
                UpdateVisualisersShown(false);
            }
        }
    }

    //On GUI Updates more often in Edit mode than update, ensures smooth text alignment
    private void OnRenderObject()
    {
        if (!Application.isPlaying)
        {
            if (Time.frameCount % 12 == 0)
            {
                UpdateVisualisersShown(true);
            }
        }
    }

    /*public void AddTacticalPointVisualiser(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Add(visualiser);
    }

    public void RemoveTacticalPointVisualise(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Remove(visualiser);
    }*/

}
