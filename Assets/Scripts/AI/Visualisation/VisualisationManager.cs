using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class VisualisationManager : MonoBehaviour
{
    public Transform camTransform;
    public HashSet<TacticalPointVisualiser> tacticalPointVisualisers = new HashSet<TacticalPointVisualiser>();
   
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
        [Space(5)]
        public bool showCoverRatingRaycasts;

    }

    public Settings settings;

    #region Singleton Code
    public static VisualisationManager Instance;

    //void Awake()
    void OnEnable()   //switched it to OnEnable, cause it also triggers in EditMode unlike Awake
    {
       /* if (Instance != null)
        {
            DestroyImmediate(Instance);
        }
        else   it kept destroying itself in the editor :(
        {*/
            Instance = this;
       // }
    }
    #endregion

    void Update()
    {
        #region 1. Update Tactical Point Visualisers

        if (Application.isPlaying)
        {
            Quaternion camRot = camTransform.rotation;

            foreach (TacticalPointVisualiser visualiser in tacticalPointVisualisers)
            {
                visualiser.UpdateVisualiser(camRot, settings);
            }
        }

        #endregion
    }

    //On GUI Updates more often in Edit mode than update, ensures smooth text alignment
    private void OnRenderObject()
    {
        if (!Application.isPlaying)
        {
            Quaternion camRot = SceneView.lastActiveSceneView.rotation;

            foreach (TacticalPointVisualiser visualiser in tacticalPointVisualisers)
            {
                visualiser.UpdateVisualiser(camRot, settings);
            }
        }
    }

    public void AddTacticalPointVisualiser(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Add(visualiser);
    }

    public void RemoveTacticalPointVisualise(TacticalPointVisualiser visualiser)
    {
        tacticalPointVisualisers.Remove(visualiser);
    }

}
