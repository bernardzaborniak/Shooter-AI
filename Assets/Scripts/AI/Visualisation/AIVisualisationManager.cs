using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class AIVisualisationManager : MonoBehaviour
{
    public Transform camTransform;
    public TacticalPointsManager tacticalPointsManager;
    //public HashSet<TacticalPointVisualiser> tacticalPointVisualisers = new HashSet<TacticalPointVisualiser>();
    [Tooltip("Locks the current visualisers, new ones dont show up and old ones dont dissapear")]
    public bool lockVisualisers;

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
        foreach (Transform visTransform in transform) //theyre all children
        {
            visualisersToDestroy.Add(visTransform.gameObject);
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
        Quaternion camRot;
        Vector3 camPos;
        if (inSceneView)
        {
             camPos = SceneView.lastActiveSceneView.camera.transform.position;
            camRot = SceneView.lastActiveSceneView.rotation;
        }
        else
        {
            camPos = camTransform.position;
            camRot = camTransform.rotation;
        }

        float currentDistanceSquared;
        TacticalPointVisualiser visualiser;

        //Debug.Log("tacticalPointVisualisersNotInUse: " + tacticalPointVisualisersNotInUse.Count);

        //1. go through visualisers in use and check if they shouldnt be enqueued again 
        HashSet<TacticalPointVisualiser> visualisersToRemoveFromUse = new HashSet<TacticalPointVisualiser>();
        //Debug.Log("tacticalPointVisualisersInUse: " + tacticalPointVisualisersInUse.Count);
        //Debug.Log("tacticalPointsBeingCurrentlyVisualised: " + tacticalPointsBeingCurrentlyVisualised.Count);
        foreach (TacticalPointVisualiser visualiserInUse in tacticalPointVisualisersInUse)
        {
            //check if visualiser should be disabled tgether with point renderer according to settings
            if (!ShowPointAccordingToSettings(visualiserInUse.pointToVisualise))
            {
                visualisersToRemoveFromUse.Add(visualiserInUse);
            }
            else
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
        }

        foreach (TacticalPointVisualiser visualiserToRemoveFromUse in visualisersToRemoveFromUse)
        {
            visualiserToRemoveFromUse.gameObject.SetActive(false);
            visualiserToRemoveFromUse.transform.SetParent(transform);


            tacticalPointVisualisersInUse.Remove(visualiserToRemoveFromUse);
            tacticalPointsBeingCurrentlyVisualised.Remove(visualiserToRemoveFromUse.pointToVisualise);
            visualiserToRemoveFromUse.pointToVisualise = null;

            tacticalPointVisualisersNotInUse.Enqueue(visualiserToRemoveFromUse); //bring it back into the queue which can be used
        }

        //Debug.Log("visualiser update: tacticalPointsManager.tacticalPoints size" + tacticalPointsManager.tacticalPoints.Count);
        foreach (TacticalPoint point in tacticalPointsManager.tacticalPoints)
        {
            //1. check if point is shown at all

            if (ShowPointAccordingToSettings(point))
            {
                point.pointRenderer.enabled = true;

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
                            visualiser.transform.position = point.transform.position;
                            visualiser.gameObject.SetActive(true);

                        }
                        else
                        {
                            //visualiser.UpdateVisualiser(camRot, settings, true);
                        }
                    }
                }
            }
            else
            {
                point.pointRenderer.enabled = false;
            }
 
        }
    }

    bool ShowPointAccordingToSettings(TacticalPoint point)
    {
        if (point.tacticalPointType == TacticalPointType.OpenFieldPoint)
        {
            if (settings.showOpenFieldPoints) return true;
        }
        else if (point.tacticalPointType == TacticalPointType.CoverPoint)
        {
            if (settings.showCoverPoints) return true;
        }
        else if (point.tacticalPointType == TacticalPointType.CoverShootPoint)
        {
            if (settings.showCoverShootPoints) return true;
        }

        return false;
    }


    void Update()
    {
        if (!lockVisualisers)
        {
            if (Application.isPlaying)
            {
                if (Time.frameCount % 12 == 0)
                {
                    UpdateVisualisersShown(false);
                }
            }
        }
        
    }

    //On GUI Updates more often in Edit mode than update, ensures smooth text alignment
    private void OnRenderObject()
    {
        if (!lockVisualisers)
        {
            if (!Application.isPlaying)
            {
                //if (Time.frameCount % 12 == 0)
                //{
                //Debug.Log("vis render ");
                UpdateVisualisersShown(true);
                //}
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
