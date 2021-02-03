﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BenitosAI
{

    [ExecuteInEditMode]
    public class AIVisualisationManager : MonoBehaviour
    {
        #region Fields
        [System.Serializable]
        public class Settings
        {
            public AIVisualisationManager manager;

            [Header("Visualising Tactical Points")]
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

            [Header("Visualising Sensing/Blackboard Information")]
            [SerializeField] bool showEnemiesInWorld;
            public bool ShowEnemiesInWorld
            {
                get
                {
                    return showEnemiesInWorld;
                }
                set
                {
                    showEnemiesInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

            [SerializeField] bool showFriendliesInWorld;
            public bool ShowFriendliesInWorld
            {
                get
                {
                    return showFriendliesInWorld;
                }
                set
                {
                    showFriendliesInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

            [SerializeField] bool showTPCoverInWorld;
            public bool ShowTPCoverInWorld
            {
                get
                {
                    return showTPCoverInWorld;
                }
                set
                {
                    showTPCoverInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

            [SerializeField] bool showTPOpenFieldInWorld;
            public bool ShowTPOpenFieldInWorld
            {
                get
                {
                    return showTPOpenFieldInWorld;
                }
                set
                {
                    showTPOpenFieldInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

            [SerializeField] bool showTPCoverPeekInWorld;
            public bool ShowTPCoverPeekInWorld
            {
                get
                {
                    return showTPCoverPeekInWorld;
                }
                set
                {
                    showTPCoverPeekInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

            [SerializeField] bool showTPCurrentlyUsedInWorld;
            public bool ShowTPCurrentlyUsedInWorld
            {
                get
                {
                    return showTPCurrentlyUsedInWorld;
                }
                set
                {
                    showTPCurrentlyUsedInWorld = value;
                    manager.UpdateBlackboardInfoVisualisersInWorldSpace();
                }
            }

        }

        [Header("References")]
        public UnityTemplateProjects.SimpleCameraController cameraController;
        public TacticalPointsManager tacticalPointsManager;
        public AIVisualisationUI aIVisualisationUI;

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

        //for updating sensing UI
        AIController_Blackboard selectedSoldierBlackboard;
        AIController_Blackboard selectedSoldiersBlackboardLastFrame;
        float lastUpdateSensingFrameCount;


        [Header("For Visualising Sensing/Blackboard Information in World Space")]
        /*public Transform showEnemiesInWorldBox;
        public Transform showFriendliesInWorldBox;
        public Transform showTPCoverInWorldBox;
        public Transform showTPOpenFieldInWorldBox;
        public Transform showTPCoverPeekInWorldBox;
        public Transform showTPCurrentlyUsedInWorldBox;*/
        public Transform visualisedBlackboardInfoParent;

        [SerializeField] GameObject enemyInfoVisualiserPrefab;
        [SerializeField] GameObject friendlyInfoVisualiserPrefab;
        [SerializeField] GameObject tPCoverInfoVisualiserPrefab;
        [SerializeField] GameObject tPOpenFieldInfoVisualiserPrefab;
        [SerializeField] GameObject tPCoverPeekInfoVisualiserPrefab;
        [SerializeField] GameObject tPCurrentlyUsedInfoVisualiserPrefab;

        HashSet<AI_Vis_SensedBlackboardInfoVisualiser> visualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();

        /*HashSet<AI_Vis_SensedBlackboardInfoVisualiser> enemyInfoVisualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();
        HashSet<AI_Vis_SensedBlackboardInfoVisualiser> friendlyInfoVisualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();
        HashSet<AI_Vis_SensedBlackboardInfoVisualiser> tPCoverInfoVisualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();
        HashSet<AI_Vis_SensedBlackboardInfoVisualiser> tPOpenFieldInfoVisualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();
        HashSet<AI_Vis_SensedBlackboardInfoVisualiser> tPCoverPeekInfoVisualisersInUse = new HashSet<AI_Vis_SensedBlackboardInfoVisualiser>();
        AI_Vis_SensedBlackboardInfoVisualiser tPCurrentlyUsedInfoVisualiserInUse;*/


        //AI_Vis_SensedBlackboardInfoVisualiser

        #region Variables Cached to reduce garbage

        //--------- UpdateVisualisersShown -------

        HashSet<TacticalPointVisualiser> visualisersToRemoveFromUse = new HashSet<TacticalPointVisualiser>();

        // --------------------

        #endregion

        #endregion

        void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
        {
            settings.manager = this;

            tacticalPointVisualisersNotInUse = new Queue<TacticalPointVisualiser>();
            tacticalPointVisualisersInUse = new HashSet<TacticalPointVisualiser>();
            tacticalPointsBeingCurrentlyVisualised = new HashSet<TacticalPoint>();

            if (selectedSoldierVisualiser)
            {
                selectedSoldierVisualiser.SetActive(false);
            }

            for (int i = 0; i < tacticalPointVisualisers.Length; i++)
            {
                tacticalPointVisualisers[i].gameObject.SetActive(false);
                tacticalPointVisualisersNotInUse.Enqueue(tacticalPointVisualisers[i]);
            }

            // Square Distances for optimisation purposes.
            ratingRingCullDistanceSquared = ratingRingCullDistance * ratingRingCullDistance;
        }

        void Update()
        {
            #region Select/ Deselect Soldier & visalise

            if (Input.GetMouseButtonDown(0))
            {
                if (!Utility.DoesMouseClickOnThisPositionHitUIElement(Input.mousePosition))
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit, 200, selectingSoldiersLayerMask))
                    {
                        Debug.Log("hit: " + hit.collider.gameObject.name);
                        //MoveTo(hit.point);
                        if (hit.transform.gameObject.GetComponent<GameEntity>())
                        {
                            currentSelectedSoldier = hit.transform.gameObject.GetComponent<GameEntity>();
                            selectedSoldierBlackboard = currentSelectedSoldier.transform.GetChild(1).GetComponent<AIController_Blackboard>(); //not the nicest way to get this sensing component;
                            lastUpdateSensingFrameCount = selectedSoldierBlackboard.lastFrameCountSensingInfoWasUpdated;
                        }
                        else
                        {
                            currentSelectedSoldier = null;
                            selectedSoldierBlackboard = null;
                            lastUpdateSensingFrameCount = 0;
                        }

                    }
                    else
                    {
                        currentSelectedSoldier = null;
                        selectedSoldierBlackboard = null;
                        lastUpdateSensingFrameCount = 0;
                    }
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
                selectedSoldierBlackboard = null;
            }

            if (Application.isPlaying)
            {
                UpdateBlackboardVisualisers();
                //UpdateBlackboardInfoVisualisersInWorldSpace();
                selectedSoldiersBlackboardLastFrame = selectedSoldierBlackboard;
            }


            #endregion


            // Update is called only in Play Mode.
            if (!lockTPVisualisers)
            {
                if (Application.isPlaying)
                {
                    if (Time.frameCount % 12 == 0)
                    {
                        UpdateTPointVisualisersShown(false);
                    }
                }
            }

        }

#if UNITY_EDITOR
        void OnRenderObject()
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
                        UpdateTPointVisualisersShown(true); //maybe this should be done every x frames too? But how to achieve this in Edit mode?
                    }
                }

            }
        }
#endif

        void UpdateTPointVisualisersShown(bool inSceneView)
        {
            #region Prepare Variables

            Vector3 camPos = cameraController.transform.position;
            Quaternion camRot = cameraController.transform.rotation;
            Vector3 camForward = cameraController.transform.forward;


#if UNITY_EDITOR
            if (inSceneView)
            {
                try  //we do a try cause sometimes this causes a nullpointer exception if the correct window isnt focused
                {
                    camPos = SceneView.lastActiveSceneView.camera.transform.position;
                    camRot = SceneView.lastActiveSceneView.rotation;
                    camForward = SceneView.lastActiveSceneView.camera.transform.forward;
                }
                catch (Exception e) { }
            }
#endif


            #endregion

            #region 1. Go through visualisers in use and check if they shouldnt be enqueued again, cause their distance is too great 

            visualisersToRemoveFromUse.Clear();

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
                    if (Vector3.Angle(-camForward, (camPos - visualiserInUse.transform.position)) > 40)
                    {
                        visualisersToRemoveFromUse.Add(visualiserInUse);
                    }
                    else
                    {
                        // If not, Check if visualiser should be disabled tgether with point renderer according to distance.
                        float currentDistanceSquared = (visualiserInUse.transform.position - camPos).sqrMagnitude;

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
                            float currentDistanceSquared = (point.transform.position - camPos).sqrMagnitude;

                            if (tacticalPointVisualisersNotInUse.Count > 0)
                            {
                                if (currentDistanceSquared < ratingRingCullDistanceSquared)
                                {
                                    // Glue an Visualiser to another Point 

                                    // manage collections
                                    TacticalPointVisualiser visualiser = tacticalPointVisualisersNotInUse.Dequeue();
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
            else if (point.tacticalPointType == TacticalPointType.CoverPeekPoint)
            {
                if (settings.showCoverShootPoints) return true;
            }

            return false;
        }

        void UpdateBlackboardVisualisers() //both ui and in world space
        {
            // Update UI only if needed
            // -> everytime a soldier is selected or deselected and everytime sensing was updated by the soldier
            UnityEngine.Profiling.Profiler.BeginSample("UpdateBlackboardUI");


            //if deselected soldier (soldier died will not be active here, cause than the reference to selectedsoldiersblackbard last frame is false too -> so we just delete every frame
            if (selectedSoldierBlackboard == null)// && selectedSoldiersBlackboardLastFrame != null)
            {
                aIVisualisationUI.UpdateSensingUIItems(null);
                UpdateBlackboardInfoVisualisersInWorldSpace();
            }
            //if selected soldier
            else if (selectedSoldierBlackboard != null && selectedSoldiersBlackboardLastFrame == null)
            {
                aIVisualisationUI.UpdateSensingUIItems(selectedSoldierBlackboard);
                UpdateBlackboardInfoVisualisersInWorldSpace();
            }
            //if selected different soldier
            else if (selectedSoldierBlackboard != selectedSoldiersBlackboardLastFrame)
            {
                aIVisualisationUI.UpdateSensingUIItems(selectedSoldierBlackboard);
                UpdateBlackboardInfoVisualisersInWorldSpace();

            }
            //else only update ui everytime the sensing component updated
            else if (selectedSoldierBlackboard != null)
            {
                if (lastUpdateSensingFrameCount != selectedSoldierBlackboard.lastFrameCountSensingInfoWasUpdated)
                {
                    aIVisualisationUI.UpdateSensingUIItems(selectedSoldierBlackboard);
                    UpdateBlackboardInfoVisualisersInWorldSpace();
                    lastUpdateSensingFrameCount = selectedSoldierBlackboard.lastFrameCountSensingInfoWasUpdated;
                }
            }

            AlignBlackboardInfoVisualisersToCamera();
            UnityEngine.Profiling.Profiler.EndSample();

        }

        public void AlignBlackboardInfoVisualisersToCamera()
        {
            Vector3 camForward = cameraController.transform.forward;

            foreach (AI_Vis_SensedBlackboardInfoVisualiser visualiser in visualisersInUse)
            {
                visualiser.UpdateVisualiser(camForward);
            }
        }

        public void UpdateBlackboardInfoVisualisersInWorldSpace()
        {
            UnityEngine.Profiling.Profiler.BeginSample("UpdateBlackboardInfoVisualisersInWorldSpace");
            //Debug.Log("UpdateBlackboardInfoVisualisersInWorldSpace");


            //1. Delete all current
            foreach (AI_Vis_SensedBlackboardInfoVisualiser visualiser in visualisersInUse)
            {
                Destroy(visualiser.gameObject);
            }
            visualisersInUse.Clear();


            //2. Instantiate new ones

            if (selectedSoldierBlackboard != null)
            {
                GameObject instantiatedObject;
                AI_Vis_SensedBlackboardInfoVisualiser instantiatedVisualiser;

                if (settings.ShowEnemiesInWorld)
                {
                    for (int i = 0; i < selectedSoldierBlackboard.enemyInfos.Length; i++)
                    {
                        instantiatedObject = Instantiate(enemyInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForEnemyEntityInfo(selectedSoldierBlackboard.enemyInfos[i]);

                        visualisersInUse.Add(instantiatedVisualiser);
                    }
                }


                if (settings.ShowFriendliesInWorld)
                {
                    for (int i = 0; i < selectedSoldierBlackboard.friendlyInfos.Length; i++)
                    {
                        instantiatedObject = Instantiate(friendlyInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForFriendlyEntityInfo(selectedSoldierBlackboard.friendlyInfos[i]);

                        visualisersInUse.Add(instantiatedVisualiser);
                    }
                }

                if (settings.ShowTPCoverInWorld)
                {
                    for (int i = 0; i < selectedSoldierBlackboard.tPCoverInfos.Length; i++)
                    {
                        instantiatedObject = Instantiate(tPCoverInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForTPointCoverInfo(selectedSoldierBlackboard.tPCoverInfos[i]);

                        visualisersInUse.Add(instantiatedVisualiser);
                    }
                }

                if (settings.ShowTPOpenFieldInWorld)
                {
                    for (int i = 0; i < selectedSoldierBlackboard.tPOpenFieldInfos.Length; i++)
                    {
                        instantiatedObject = Instantiate(tPOpenFieldInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForTPointOpenFieldInfo(selectedSoldierBlackboard.tPOpenFieldInfos[i]);

                        visualisersInUse.Add(instantiatedVisualiser);
                    }
                }

                if (settings.ShowTPCoverPeekInWorld)
                {
                    for (int i = 0; i < selectedSoldierBlackboard.tPCoverPeekInfos.Length; i++)
                    {
                        instantiatedObject = Instantiate(tPCoverPeekInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForTPointCoverPeekInfo(selectedSoldierBlackboard.tPCoverPeekInfos[i]);

                        visualisersInUse.Add(instantiatedVisualiser);
                    }
                }

                if (settings.ShowTPCurrentlyUsedInWorld)
                {
                    if (selectedSoldierBlackboard.GetCurrentlyUsedTacticalPoint())
                    {
                        instantiatedObject = Instantiate(tPCurrentlyUsedInfoVisualiserPrefab, visualisedBlackboardInfoParent);
                        instantiatedVisualiser = instantiatedObject.GetComponent<AI_Vis_SensedBlackboardInfoVisualiser>();
                        instantiatedVisualiser.SetUpForCurrentlyUsedTPoint(selectedSoldierBlackboard.GetCurrentlyUsedTacticalPoint());

                        visualisersInUse.Add(instantiatedVisualiser);
                    }                 
                }
            }
            UnityEngine.Profiling.Profiler.EndSample();
        }

        public void FrameCameraOnObject(Transform objectToFrameOn)
        {
            cameraController.FrameOnObject(objectToFrameOn);
        }
    }

}