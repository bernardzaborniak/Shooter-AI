using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//TODO Move this enum to the tactical point calsss later on
public enum TacticalPointType
{
    CoverPoint,
    CoverShootPoint,
    OpenFieldPoint
}

public class TacticalPointVisualiser : MonoBehaviour
{
    public bool visualiserEnabled;

    public TacticalPointType tacticalPointType;

    [Space(5)]
    public float[] standingDistanceRating;
    public TextMeshPro[] tmp_standingDistanceRating;
    [Space(5)]
    public float[] standingQualityRating;
    public TextMeshPro[] tmp_standingQualityRating;

    [Space(5)]
    public float[] crouchedDistanceRating;
    public TextMeshPro[] tmp_crouchedDistanceRating;
    [Space(5)]
    public float[] crouchedDistanceQuality;
    public TextMeshPro[] tmp_crouchedDistanceQuality;

    [Header("Cover Distance Rating Coloring")]
    [Tooltip("Remap between worst ad best to the worst & best color, best distance should be smaller than worst")]
    public float bestDistance = 0;
    public float worstDistance = 30;

    public Color bestDistanceColor;
    public Color worstDistanceColor;

    [Header("Cover Quality Rating Coloring")]
    [Tooltip("Remap between worst ad best to the worst & best color, best quality hsould be bigger than worst")]
    public float bestQuality = 1;
    public float worstQuality = 0;

    public Color bestQualityColor;
    public Color worstQualityColor;

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

    public void UpdateVisualiser(Vector3 cameraForward)
    {

        //Standing Distance
        UpdateRatingRing(cameraForward, standingDistanceRating, tmp_standingDistanceRating, standingDistanceRenderer, worstDistance, bestDistance, worstDistanceColor, bestDistanceColor);

       /* //Standing Quality
        clampedRatingQuality = Mathf.Clamp(coverRatingStandingQuality[i], worstQuality, bestQuality);
        normalizedRatingQuality = Utility.Remap(clampedRatingQuality, worstQuality, bestQuality, 0, 1);
        currentMappedQualityCol = Color.Lerp(worstQualityColor, bestQualityColor, normalizedRatingQuality);

        coverRatingStandingNumbersQuality[i].text = clampedRatingQuality.ToString();
        coverRatingStandingNumbersQuality[i].color = currentMappedQualityCol;

        //Crouched Distance
        clampedRatingDistance = Mathf.Clamp(coverRatingCrouchedDistance[i], bestDistance, worstDistance);
        normalizedRatingDistance = Utility.Remap(clampedRatingDistance, worstDistance, bestDistance, 0, 1);
        currentMappedDistanceCol = Color.Lerp(worstDistanceColor, bestDistanceColor, normalizedRatingDistance);

        coverRatingCrouchedNumbersDistance[i].text = clampedRatingDistance.ToString();
        coverRatingCrouchedNumbersDistance[i].color = currentMappedDistanceCol;

        //Crouched Quality
        clampedRatingQuality = Mathf.Clamp(coverRatingCrouchedQuality[i], worstQuality, bestQuality);
        normalizedRatingQuality = Utility.Remap(clampedRatingQuality, worstQuality, bestQuality, 0, 1);
        currentMappedQualityCol = Color.Lerp(worstQualityColor, bestQualityColor, normalizedRatingQuality);

        coverRatingCrouchedNumbersDistance[i].text = clampedRatingQuality.ToString();
        coverRatingCrouchedNumbersDistance[i].color = currentMappedQualityCol;

        //2. set the cover color according to numbers & set the text Elements text,color 6 align them*/



    }

    void UpdateRatingRing(Vector3 alignTextForward, float[] rating, TextMeshPro[] text, Renderer ratingVisRenderer, float worstValue, float bestValue, Color worstColor, Color bestColor)
    {
        float clampedRating;
        float normalizedRating;
        Color currentMappedCol;

        propertyBlock = new MaterialPropertyBlock();
        standingDistanceRenderer.GetPropertyBlock(propertyBlock);

        for (int i = 0; i < 8; i++)
        {
            clampedRating = Mathf.Clamp(rating[i], bestValue, worstValue);
            normalizedRating = Utility.Remap(clampedRating, worstValue, bestValue, 0, 1);
            currentMappedCol = Color.Lerp(worstColor, bestColor, normalizedRating);

            text[i].text = clampedRating.ToString();
            text[i].color = currentMappedCol;
            text[i].transform.forward = alignTextForward;

            propertyBlock.SetColor(propertyNames[i], currentMappedCol);
        }
        standingDistanceRenderer.SetPropertyBlock(propertyBlock);
    }

}
