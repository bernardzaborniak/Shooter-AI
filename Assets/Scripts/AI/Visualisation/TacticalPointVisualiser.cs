using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Responsible for visualising the rating of a cover point.
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


    //[Header("Point Visualisation")]
    // public Renderer pointRenderer;
    // string takenBoolName = "Boolean_C1FD8F9C";


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

    #region cached variables

    // ----- UpdateRatingRing --------
    Color currentMappedCol;
    MaterialPropertyBlock propertyBlock; //Used to hange material values in a performant way
    //-------------

    // ----- GetMappedColor --------
    float clampedRating;
    float normalizedRating;
    //--------

    #endregion


    #endregion

    public void UpdateVisualiser(Quaternion cameraRot, AIVisualisationManager.Settings visualisationSettings)
    {
        //Distance Rating
        if (visualisationSettings.showCoverDistanceRating)
        {
            standingDistanceRenderer.enabled = true;
            crouchedDistanceRenderer.enabled = true;

            if (visualisationSettings.showCoverDistanceRatingNumbers)
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

        //Quality Rating
        if (visualisationSettings.showCoverQualityRating)
        {
            standingQualityRenderer.enabled = true;
            crouchedQualityRenderer.enabled = true;
            if (visualisationSettings.showCoverQualityRatingNumbers)
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
    }

    public void UpdateCoverRingMaterialsAndText()
    {
        // Updates the colors of the directions and the text inside the text components & text color.
        UpdateRatingRing(false, standingDistanceRenderer, pointToVisualise.coverRating.standingDistanceRating, tmp_standingDistanceRatingParent, tmp_standingDistanceRating, standingDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(false, crouchedDistanceRenderer, pointToVisualise.coverRating.crouchedDistanceRating, tmp_crouchedDistanceRatingParent, tmp_crouchedDistanceRating, crouchedDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(true, standingQualityRenderer, pointToVisualise.coverRating.standingQualityRating, tmp_standingQualityRatingParent, tmp_standingQualityRating, standingQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);
        UpdateRatingRing(true, crouchedQualityRenderer, pointToVisualise.coverRating.crouchedQualityRating, tmp_crouchedQualityRatingParent, tmp_crouchedQualityRating, crouchedQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);
    }

    void UpdateRatingRing(bool quality,  Renderer renderer,  float[] rating,  GameObject textParent, TextMeshPro[] text, Renderer ratingVisRenderer, float worstValue, float bestValue, Color worstColor, Color bestColor)
    {
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
        clampedRating = Mathf.Clamp(distance, bestDistance, worstDistance);
        normalizedRating = Utility.Remap(clampedRating, worstDistance, bestDistance, 0, 1);
        return Color.Lerp(worstDistanceColor, bestDistanceColor, normalizedRating);
    }

    Color GetMappedQualityRatingColor(float quality)
    {
        clampedRating = Mathf.Clamp(quality, worstQuality, bestQuality);
        normalizedRating = Utility.Remap(clampedRating, worstQuality, bestQuality, 0, 1);
        return Color.Lerp(worstQualityColor, bestQualityColor, normalizedRating);
    }

    private void OnDrawGizmos()
    {
        // The Gizmos Visualise the Raycasts

        if (pointToVisualise)
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
}


