using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


// Curves constructed according to Daves Mark's GDC talk: "Architecture Tricks: Managing Behaviors in Time, Space, and Depth" 
// found here: https://www.gdcvault.com/play/1018040/Architecture-Tricks-Managing-Behaviors-in


[System.Serializable]
public class CustomCurve 
{
    #region Fields

    public enum CurveType
    {
        Binary,
        Linear,
        Exponential,
        Logistic, //like a smoothdamp
        Logit //as your moving away from the centerpoint you get more and more extreme reactions
    }

    public CurveType curveType;


    [SerializeField] float bi_threshold = 0.5f;
    [SerializeField] bool bi_inverse;

    [SerializeField] float lin_Slope = 1;
    [SerializeField] float lin_Shift;

    [SerializeField] float ex_Slope = 1;
    [SerializeField] float ex_Exponent = 2;
    [SerializeField] float ex_VertShift;
    [SerializeField] float ex_HorizShift;

    [SerializeField] float logistic_Slope = 0.15f;
    [SerializeField] float logistic_XShift;
    [SerializeField] float logistic_YShift;
    [SerializeField] float logistic_YScalar = 1;

    [SerializeField] float logit_A = 0.035f;
    [SerializeField] float logit_B = 0.075f;

    // Used for Visualisation
    public Vector2[] curveVisualisationPositions;

    #endregion

    #region Getting Values Remapped By Curve

    public float GetRemappedValue(float input)
    {
        if (curveType == CurveType.Linear)
        {
            return Mathf.Clamp(GetRemappedValueLinear(input), 0, 1);
        }
        else if (curveType == CurveType.Exponential)
        {
            return Mathf.Clamp(GetRemappedValueExponential(input), 0, 1);
        }
        else if (curveType == CurveType.Logistic)
        {
            return Mathf.Clamp(GetRemappedValueLogistic(input), 0, 1);
        }
        else if (curveType == CurveType.Logit)
        {
            return Mathf.Clamp(GetRemappedValueLogit(input), 0, 1);
        }
        else if (curveType == CurveType.Binary)
        {
            return Mathf.Clamp(GetRemappedValueBinary(input), 0, 1);
        }

        return 0;
    }

    float GetRemappedValueBinary(float input)
    {
        if (!bi_inverse)
        {
            if (input > bi_threshold) return 1;
            else return 0;
        }
        else
        {
            if (input > bi_threshold) return 0;
            else return 1;
        }

    }

    float GetRemappedValueExponential(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
        /* return Mathf.Clamp
             (
                 ex_Slope * Mathf.Pow(input- ex_HorizShift, ex_Exponent) + ex_VertShift,
                 0,
                 1
             );*/   
        return ex_Slope * Mathf.Pow(input - ex_HorizShift, ex_Exponent) + ex_VertShift;
    }

    float GetRemappedValueLinear(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
        /* return Mathf.Clamp
             (
                 lin_Slope * input  + lin_Shift, 
                 0, 
                 1
             );*/
        return lin_Slope * input + lin_Shift;
    }

    float GetRemappedValueLogistic(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too

        /*return Mathf.Clamp
            (
                (logistic_YScalar / (1 + Mathf.Pow(Mathf.Epsilon, -logistic_Slope * (input - (logistic_XShift + 0.5f))))) + logistic_YShift,
                0, 
                1
            );*/

        return (logistic_YScalar / (1 + Mathf.Pow(Mathf.Epsilon, -logistic_Slope * (input - (logistic_XShift + 0.5f))))) + logistic_YShift;
    }

    float GetRemappedValueLogit(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too

        /* return Mathf.Clamp
             (
                 (Mathf.Log(input / (1 - input), Mathf.Epsilon) + logit_A) / logit_B,
                 0, 
                 1
             );*/
        return (Mathf.Log(input / (1 - input), Mathf.Epsilon) + logit_A) / logit_B;
    }

    #endregion

    public void UpdateCurveVisualisationKeyframes()
    {
        int keyFramesCount = 56;
        curveVisualisationPositions = new Vector2[keyFramesCount];

        for (int k = 0; k < (keyFramesCount-1); k++)  //count -1 cause we make sure that the last one has the value 1
        {
            float XPos = k *1f / (keyFramesCount-1);
            float yPos = 0;

            yPos = GetRemappedValue(XPos);

            curveVisualisationPositions[k] = new Vector2(XPos,yPos);
        }

        curveVisualisationPositions[keyFramesCount-1] = new Vector2(1, GetRemappedValue(1)); //the last one should have the x value 1
    }

    // Not Used anymore
    public void GetCurveVisualisationTexture(ref Texture2D texture)
    {
        int textureWidth = texture.width;
        int textureHeight = texture.height;

        int grid10XResolution = texture.width / 10;
        int grid10YResolution = texture.height / 10;
        Color grid10Color = new Color(0.9f, 0.9f, 0.9f);
        int grid2XResolution = texture.width / 2;
        int grid2YResolution = texture.height / 2;
        Color grid2Color = new Color(0.75f, 0.75f, 0.75f);


        for (int x = 0; x < textureWidth; x++)
        {
            for (int y = 0; y < textureHeight; y++)
            {
                if (x % grid10XResolution == 0 || y % grid10YResolution == 0)
                {
                    texture.SetPixel(x, y, grid10Color);
                }
                else if (x % grid2XResolution == 0 || x % grid2XResolution == 1 || y % grid2YResolution == 0 || y % grid2YResolution == 1)
                {
                    texture.SetPixel(x, y, grid2Color);
                }
                else
                {
                    texture.SetPixel(x, y, Color.white);
                }
            }

            int yPos = 0;
            //here color the current output pixel black
            if (curveType == CurveType.Linear)
            {
                yPos = Mathf.Clamp((int)(GetRemappedValueLinear(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1);
            }
            else if (curveType == CurveType.Exponential)
            {
                yPos = Mathf.Clamp((int)(GetRemappedValueExponential(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1);
            }
            else if (curveType == CurveType.Logistic)
            {
                yPos = Mathf.Clamp((int)(GetRemappedValueLogistic(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1);
            }
            else if (curveType == CurveType.Logit)
            {
                yPos = Mathf.Clamp((int)(GetRemappedValueLogit(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1);

            }
            else if (curveType == CurveType.Binary)
            {
                yPos = Mathf.Clamp((int)(GetRemappedValueBinary(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1);
            }

            texture.SetPixel(x, yPos, Color.black);

        }

        texture.Apply();

    }

}


