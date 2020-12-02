using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//TODO Move this enum to the tactical point calsss later on

[ExecuteInEditMode]
public class TacticalPointVisualiser : MonoBehaviour
{
    #region Fields

    public TacticalPoint pointToVisualise;


    [Header("Cover Distance Rating Coloring")]
    [Space(5)]
    [Tooltip("Remap between worst ad best to the worst & best color, best distance should be smaller than worst")]
    public float bestDistance = 0;
    public float worstDistance = 30;

    public Color bestDistanceColor;
    public Color worstDistanceColor;


    [Header("Cover Quality Rating Coloring")]
    [Space(5)]
    [Tooltip("Remap between worst ad best to the worst & best color, best quality hsould be bigger than worst")]
    public float bestQuality = 1;
    public float worstQuality = 0;

    public Color bestQualityColor;
    public Color worstQualityColor;


    [Header("Point Visualisation")]
    public Renderer pointRenderer;
    string takenBoolName = "Boolean_C1FD8F9C";


    [Header("Text References")]
    public GameObject tmp_standingDistanceRatingParent;
    public TextMeshPro[] tmp_standingDistanceRating;
    public GameObject tmp_standingQualityRatingParent;
    public TextMeshPro[] tmp_standingQualityRating;
    [Space(5)]
    public GameObject tmp_crouchedDistanceRatingParent;
    public TextMeshPro[] tmp_crouchedDistanceRating;
    public GameObject tmp_crouchedQualityRatingParent;
    public TextMeshPro[] tmp_crouchedQualityRating;

    [Header("Material References")]
    public Renderer standingDistanceRenderer;
    public Renderer standingQualityRenderer;
    public Renderer crouchedDistanceRenderer;
    public Renderer crouchedQualityRenderer;

    MaterialPropertyBlock propertyBlock; //Used to hange material values in a performant way

    string[] propertyNames = new string[] {
        "Color_AEBF42CB" ,
        "Color_C03A6417" ,
        "Color_EB143371" ,
        "Color_35C83910" ,
        "Color_2AF4D302" ,
        "Color_2B1DAD94" ,
        "Color_6FDD353B" ,
        "Color_74F223A0"
    };

    [Header("Raycast Visualisation")]
    [Tooltip("this should have the length of 8 - corresponding to the driections")]
    public bool showCrouchedRaycasts;
    public bool showStandingRaycasts;
    public bool[] showRaycastsSubSettings;


    #endregion


    void OnEnable()
    {
        VisualisationManager.Instance.AddTacticalPointVisualiser(this);

        //update values and colors on enable too

        //TODO also dont use updateVisualiser from the visualiationManager, use ONTacticalPointValuesChanged instead, which is called by the tactical point

        //UpdateVisualiser  will only take care of the text alignement
        UpdateCoverRingMaterialsAndText();
    }

    void OnDisable()
    {
        VisualisationManager.Instance.RemoveTacticalPointVisualise(this);
    }

    public void EnableVisualiser()
    {

    }

    public void DisableVisualiser()
    {

    }

    public void UpdateVisualiser(Quaternion cameraRot, VisualisationManager.Settings visualisationSetting, bool cullText = false)
    {
        //enables/disables components based on rating and aligns the text rotation to camera

        #region Determine whether the point should be drawn
        bool drawPoint = false;
        if (pointToVisualise.tacticalPointType == TacticalPointType.OpenFieldPoint)
        {
            if (visualisationSetting.showOpenFieldPoints) drawPoint = true;
        }
        else if (pointToVisualise.tacticalPointType == TacticalPointType.CoverPoint)
        {
            if (visualisationSetting.showCoverPoints) drawPoint = true;
        }
        else if (pointToVisualise.tacticalPointType == TacticalPointType.CoverShootPoint)
        {
            if (visualisationSetting.showCoverShootPoints) drawPoint = true;
        }
        #endregion

        if (drawPoint)
        {
            pointRenderer.enabled = true;


            //Distance
            if (visualisationSetting.showCoverDistanceRating)
            {
                standingDistanceRenderer.enabled = true;
                crouchedDistanceRenderer.enabled = true;

                if (visualisationSetting.showCoverDistanceRatingNumbers && !cullText)
                {
                    tmp_standingDistanceRatingParent.SetActive(true);
                    tmp_crouchedDistanceRatingParent.SetActive(true);

                    //align to camera
                    for (int i = 0; i < 8; i++)
                    {
                        tmp_standingDistanceRating[i].transform.rotation = cameraRot;
                        tmp_crouchedDistanceRating[i].transform.rotation = cameraRot;
                    }
                }
                else
                {
                    tmp_standingDistanceRatingParent.SetActive(false);
                    tmp_crouchedDistanceRatingParent.SetActive(false);
                }
            }
            else
            {
                standingDistanceRenderer.enabled = false;
                crouchedDistanceRenderer.enabled = false;

                tmp_standingDistanceRatingParent.SetActive(false);
                tmp_crouchedDistanceRatingParent.SetActive(false);
            }

            //Quality
            if (visualisationSetting.showCoverQualityRating)
            {
                standingQualityRenderer.enabled = true;
                crouchedQualityRenderer.enabled = true;
                if (visualisationSetting.showCoverQualityRatingNumbers && !cullText)
                {
                    tmp_standingQualityRatingParent.SetActive(true);
                    tmp_crouchedQualityRatingParent.SetActive(true);

                    //align to camera
                    for (int i = 0; i < 8; i++)
                    {
                        tmp_standingQualityRating[i].transform.rotation = cameraRot;
                        tmp_crouchedQualityRating[i].transform.rotation = cameraRot;
                    }
                }
                else
                {
                    tmp_standingQualityRatingParent.SetActive(false);
                    tmp_crouchedQualityRatingParent.SetActive(false);
                }
            }
            else
            {
                standingQualityRenderer.enabled = false;
                crouchedQualityRenderer.enabled = false;

                tmp_standingQualityRatingParent.SetActive(false);
                tmp_crouchedQualityRatingParent.SetActive(false);

            }

            // Set Taken Bool
            propertyBlock = new MaterialPropertyBlock();
            pointRenderer.GetPropertyBlock(propertyBlock);
            if (pointToVisualise.IsPointFull())
            {
                propertyBlock.SetFloat(takenBoolName, 1f);
            }
            else
            {
                propertyBlock.SetFloat(takenBoolName, 0f);

            }
            pointRenderer.SetPropertyBlock(propertyBlock);
        }
        else
        {
            pointRenderer.enabled = false;


                tmp_standingDistanceRatingParent.SetActive(false);
                tmp_crouchedDistanceRatingParent.SetActive(false);
                tmp_standingQualityRatingParent.SetActive(false);
                tmp_crouchedQualityRatingParent.SetActive(false);

                standingDistanceRenderer.enabled = false;
                standingQualityRenderer.enabled = false;
                crouchedDistanceRenderer.enabled = false;
                crouchedQualityRenderer.enabled = false;

        }




    }


    void UpdateRatingRing(bool quality, Renderer renderer, float[] rating, GameObject textParent, TextMeshPro[] text, Renderer ratingVisRenderer, float worstValue, float bestValue, Color worstColor, Color bestColor)
    {
        Color currentMappedCol;

        propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);

        for (int i = 0; i < 8; i++)
        {

            if (quality)
            {
                currentMappedCol = GetMappedQualityRatingColor(rating[i]);
            }
            else
            {
                currentMappedCol = GetMappedDistanceRatingColor(rating[i]);
            }


            text[i].text = rating[i].ToString("F1");
            text[i].color = currentMappedCol;

            propertyBlock.SetColor(propertyNames[i], currentMappedCol);
        }
        renderer.SetPropertyBlock(propertyBlock);
    }

    Color GetMappedDistanceRatingColor(float distance)
    {
        float clampedRating = Mathf.Clamp(distance, bestDistance, worstDistance);
        float normalizedRating = Utility.Remap(clampedRating, worstDistance, bestDistance, 0, 1);
        return Color.Lerp(worstDistanceColor, bestDistanceColor, normalizedRating);
    }

    Color GetMappedQualityRatingColor(float quality)
    {
        float clampedRating = Mathf.Clamp(quality, worstQuality, bestQuality);
        float normalizedRating = Utility.Remap(clampedRating, worstQuality, bestQuality, 0, 1);
        return Color.Lerp(worstQualityColor, bestQualityColor, normalizedRating);
    }

    public void UpdateCoverRingMaterialsAndText()
    {
        UpdateRatingRing(false, standingDistanceRenderer, pointToVisualise.coverRating.standingDistanceRating, tmp_standingDistanceRatingParent, tmp_standingDistanceRating, standingDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(false, crouchedDistanceRenderer, pointToVisualise.coverRating.crouchedDistanceRating, tmp_crouchedDistanceRatingParent, tmp_crouchedDistanceRating, crouchedDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(true, standingQualityRenderer, pointToVisualise.coverRating.standingQualityRating, tmp_standingQualityRatingParent, tmp_standingQualityRating, standingQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);
        UpdateRatingRing(true, crouchedQualityRenderer, pointToVisualise.coverRating.crouchedQualityRating, tmp_crouchedQualityRatingParent, tmp_crouchedQualityRating, crouchedQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);
    }




    private void OnDrawGizmos()
    {
        //crouched
        if (showCrouchedRaycasts)
        {
            for (int dir = 0; dir < 8; dir++)
            {
                if (showRaycastsSubSettings[dir])
                {

                    foreach (RaycastUsedToGenerateCoverRating raycast in pointToVisualise.GetRaycastsUsedForGeneratingRating().GetAllRaysOfDirection(0, dir))
                    {
                        Gizmos.color = GetMappedDistanceRatingColor(raycast.distance);
                        Gizmos.DrawLine(raycast.start, raycast.end);
                    }
                }
            }
        }

        //standing
        if (showStandingRaycasts)
        {
            for (int dir = 0; dir < 8; dir++)
            {
                if (showRaycastsSubSettings[dir])
                {
                    foreach (RaycastUsedToGenerateCoverRating raycast in pointToVisualise.GetRaycastsUsedForGeneratingRating().GetAllRaysOfDirection(1, dir))
                    {
                        Gizmos.color = GetMappedDistanceRatingColor(raycast.distance);
                        Gizmos.DrawLine(raycast.start, raycast.end);
                    }
                }

            }
        }

    }
}


