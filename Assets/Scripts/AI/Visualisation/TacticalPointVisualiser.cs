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
    public TextMeshPro[] tmp_standingDistanceRating;
    public TextMeshPro[] tmp_standingQualityRating;
    [Space(5)]
    public TextMeshPro[] tmp_crouchedDistanceRating;
    public TextMeshPro[] tmp_crouchedQualityRating;

    [Header("Material References")]
    public Renderer standingDistanceRenderer;
    public Renderer standingQualityRenderer;
    public Renderer crouchedDistanceRenderer;
    public Renderer crouchedQualityRenderer;
    MaterialPropertyBlock propertyBlock;// = new MaterialPropertyBlock();

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

    #endregion


    private void OnEnable()
    {
        VisualisationManager.Instance.AddTacticalPointVisualiser(this);
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

    public void UpdateVisualiser(Quaternion cameraRot)
    {

        //Standing
        UpdateRatingRing(false, cameraRot, standingDistanceRenderer, pointToVisualise.coverRating.standingDistanceRating, tmp_standingDistanceRating, standingDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(true, cameraRot, standingQualityRenderer, pointToVisualise.coverRating.standingQualityRating, tmp_standingQualityRating, standingQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);
        
        //Crouched
        UpdateRatingRing(false, cameraRot, crouchedDistanceRenderer, pointToVisualise.coverRating.crouchedDistanceRating, tmp_crouchedDistanceRating, crouchedDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);
        UpdateRatingRing(true, cameraRot, crouchedQualityRenderer, pointToVisualise.coverRating.crouchedQualityRating, tmp_crouchedQualityRating, crouchedQualityRenderer, worstQuality, bestQuality, worstQualityColor, bestQualityColor);

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

    void UpdateRatingRing(bool quality, Quaternion alignTextRot, Renderer renderer, float[] rating, TextMeshPro[] text, Renderer ratingVisRenderer, float worstValue, float bestValue, Color worstColor, Color bestColor)
    {
        float clampedRating;
        float normalizedRating;
        Color currentMappedCol;

        propertyBlock = new MaterialPropertyBlock();
        renderer.GetPropertyBlock(propertyBlock);

        for (int i = 0; i < 8; i++)
        {
            if (quality) //0 to 1
            {
                clampedRating = Mathf.Clamp(rating[i], worstValue, bestValue);
            }
            else
            {
                clampedRating = Mathf.Clamp(rating[i], bestValue, worstValue);
            }


            normalizedRating = Utility.Remap(clampedRating, worstValue, bestValue, 0, 1);
            currentMappedCol = Color.Lerp(worstColor, bestColor, normalizedRating);

            text[i].text = rating[i].ToString();
            text[i].color = currentMappedCol;
            text[i].transform.rotation = alignTextRot;

            propertyBlock.SetColor(propertyNames[i], currentMappedCol);
        }
        renderer.SetPropertyBlock(propertyBlock);
    }

}
