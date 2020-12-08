using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AIVisualisationManager : MonoBehaviour
{
    #region Fields
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

    [Header("References")]
    public Transform camTransform;
    public TacticalPointsManager tacticalPointsManager;

    [Header("Visualisation Settings")]
    [Tooltip("Locks the current tactical points visualisers, new ones dont show up and old ones dont dissapear")]
    public bool lockTPVisualisers;
    public Settings settings;

    [Header("Visualisation Optimisation")]
    [Space(5)]
    public float ratingRingCullDistance;
    float ratingRingCullDistanceSquared;

    //public int coverRatingRingPoolSize;
    public TacticalPointVisualiser[] tacticalPointVisualisers;

    Queue<TacticalPointVisualiser> tacticalPointVisualisersNotInUse;
    HashSet<TacticalPointVisualiser> tacticalPointVisualisersInUse;
    HashSet<TacticalPoint> tacticalPointsBeingCurrentlyVisualised;

    //for changing the point material
    string takenBoolName = "Boolean_C1FD8F9C";

    // For Optimising Update Times in edit mode
    public float updateVisualisersInEditModeInterval = 0.5f;
    float nextUpdateVisualisersTime;

    [Header("For Selecting Soldiers")]
    public GameEntity currentSelectedSoldier;
    public GameObject selectedSoldierVisualiser;
    public LayerMask selectingSoldiersLayerMask;

    #endregion


    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
        tacticalPointVisualisersNotInUse = new Queue<TacticalPointVisualiser>();
        tacticalPointVisualisersInUse = new HashSet<TacticalPointVisualiser>();
        tacticalPointsBeingCurrentlyVisualised = new HashSet<TacticalPoint>();

        selectedSoldierVisualiser.SetActive(false);

        for (int i = 0; i < tacticalPointVisualisers.Length; i++)
        {
            tacticalPointVisualisers[i].gameObject.SetActive(false);
            tacticalPointVisualisersNotInUse.Enqueue(tacticalPointVisualisers[i]);
        }

        // Square Distances for optimisation purposes.
        ratingRingCullDistanceSquared = ratingRingCullDistance * ratingRingCullDistance;
    }

    private void OnDisable()
    {

    }

    void Update()
    {
        #region Select/ Deselect Soldier & visalise

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 200,selectingSoldiersLayerMask))
            {
                Debug.Log("hit: " + hit.collider.gameObject.name);
                //MoveTo(hit.point);
                if (hit.transform.gameObject.GetComponent<GameEntity>())
                {
                    currentSelectedSoldier = hit.transform.gameObject.GetComponent<GameEntity>();
                }
                else
                {
                    currentSelectedSoldier = null;
                }

            }
            else
            {
                currentSelectedSoldier = null;
            }
        }

        if (currentSelectedSoldier != null)
        {
            selectedSoldierVisualiser.SetActive(true);
            selectedSoldierVisualiser.transform.position = currentSelectedSoldier.transform.position;
        }
        else
        {
            selectedSoldierVisualiser.SetActive(false);
        }

        #endregion


        // Update is called only in Play Mode.
        if (!lockTPVisualisers)
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

#if UNITY_EDITOR
    private void OnRenderObject()
    {
        // This is called only in Edit Mode.
        // On GUI Updates more often in Edit mode than update, ensures smooth text alignment
        if (!lockTPVisualisers)
        {
            if (!Application.isPlaying)
            {
                if (EditorApplication.timeSinceStartup > nextUpdateVisualisersTime)
                {
                    nextUpdateVisualisersTime = (float)EditorApplication.timeSinceStartup + updateVisualisersInEditModeInterval;
                    UpdateVisualisersShown(true); //maybe this should be done every x frames too? But how to achieve this in Edit mode?
                }
            }

        }
    }
#endif

    void UpdateVisualisersShown(bool inSceneView)
    {
        #region Prepare Variables

        Vector3 camPos = camTransform.position;
        Quaternion camRot = camTransform.rotation;
        Vector3 camForward = camTransform.forward;

#if UNITY_EDITOR
        if (inSceneView)
        {
            camPos = SceneView.lastActiveSceneView.camera.transform.position;
            camRot = SceneView.lastActiveSceneView.rotation;
            camForward = SceneView.lastActiveSceneView.camera.transform.forward;
        }
#endif

        float currentDistanceSquared;
        TacticalPointVisualiser visualiser;

        #endregion

        #region 1. Go through visualisers in use and check if they shouldnt be enqueued again, cause their distance is too great 

        HashSet<TacticalPointVisualiser> visualisersToRemoveFromUse = new HashSet<TacticalPointVisualiser>();

        foreach (TacticalPointVisualiser visualiserInUse in tacticalPointVisualisersInUse)
        {
            // Check if visualiser should be disabled tgether with point renderer according to settings.
            if (!ShowPointAccordingToSettings(visualiserInUse.pointToVisualise))
            {
                visualisersToRemoveFromUse.Add(visualiserInUse);
            }
            else
            {
                // If not, Check if the visualiser should be disabled because its no longer visible to the camera
                if (Vector3.Angle(-camForward, (camPos - visualiserInUse.transform.position))>40)
                {
                    visualisersToRemoveFromUse.Add(visualiserInUse);
                }
                else
                {
                    // If not, Check if visualiser should be disabled tgether with point renderer according to distance.
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
        }

        foreach (TacticalPointVisualiser visualiserToRemoveFromUse in visualisersToRemoveFromUse)
        {
            visualiserToRemoveFromUse.gameObject.SetActive(false);

            tacticalPointVisualisersInUse.Remove(visualiserToRemoveFromUse);
            tacticalPointsBeingCurrentlyVisualised.Remove(visualiserToRemoveFromUse.pointToVisualise);
            visualiserToRemoveFromUse.pointToVisualise = null;

            tacticalPointVisualisersNotInUse.Enqueue(visualiserToRemoveFromUse); //bring it back into the queue which can be used
        }

        #endregion

        #region 2. Go through all points and check if their renderer needs to be enabled/disabled and if some are near enough to give them a visualiser

        foreach (TacticalPoint point in tacticalPointsManager.tacticalPoints)
        {
            // 1. check if point is shown at all.
            if (ShowPointAccordingToSettings(point))
            {
                point.pointRenderer.enabled = true;


                #region Set Taken Bool

                if (Application.isPlaying) // Only in play Mode soldiers can "take" a point.
                {
                    MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
                    point.pointRenderer.GetPropertyBlock(propertyBlock);
                    if (point.IsPointFull())
                    {
                        propertyBlock.SetFloat(takenBoolName, 1f);
                    }
                    else
                    {
                        propertyBlock.SetFloat(takenBoolName, 0f);

                    }
                    point.pointRenderer.SetPropertyBlock(propertyBlock);
                }

                #endregion

                //Check if its visible to the camera
                if (Vector3.Angle(-camForward, (camPos - point.transform.position)) < 40)
                {

                    if (!tacticalPointsBeingCurrentlyVisualised.Contains(point))
                    {
                        currentDistanceSquared = (point.transform.position - camPos).sqrMagnitude;

                        if (tacticalPointVisualisersNotInUse.Count > 0)
                        {
                            if (currentDistanceSquared < ratingRingCullDistanceSquared)
                            {
                                // Glue an Visualiser to another Point 

                                // manage collections
                                visualiser = tacticalPointVisualisersNotInUse.Dequeue();
                                tacticalPointsBeingCurrentlyVisualised.Add(point);
                                tacticalPointVisualisersInUse.Add(visualiser);

                                // logic & graphic update
                                visualiser.pointToVisualise = point;
                                visualiser.UpdateVisualiser(camRot, settings);
                                visualiser.UpdateCoverRingMaterialsAndText();

                                // positioning & activating
                                //I dont parent the object, as it is more performance intinsive and can lead to potential errors
                                visualiser.transform.position = point.transform.position;
                                visualiser.gameObject.SetActive(true);

                            }
                        }
                    }
                }

            }
            else
            {
                point.pointRenderer.enabled = false;
            }
        }

        #endregion
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
}
