using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Curves constructed according to Daves Mark's GDC talk: "Architecture Tricks: Managing Behaviors in Time, Space, and Depth" 
// found here: https://www.gdcvault.com/play/1018040/Architecture-Tricks-Managing-Behaviors-in


[System.Serializable]
public class ConsiderationCurve 
{
    public enum CurveType
    {
        Binary,
        Linear,
        Quadratic,
        Logistic, //like a smoothdamp
        Logit //as your moving away from the centerpoint you get more and more extreme reactions
    }

    public CurveType curveType;


    [Header("Binary Values")]
    [Range(0,1)]
    [ConditionalEnumHide("curveType", 0)]
    [SerializeField] float bi_threshold = 0.5f;
    [ConditionalEnumHide("curveType", 0)]
    [SerializeField] bool bi_inverse;

    [Header("Linear Values")]
    [ConditionalEnumHide("curveType", 1)]
    [SerializeField] float lin_Slope = 1;
    [ConditionalEnumHide("curveType", 1)]
    [SerializeField] float lin_Shift;

    [Header("Quadratic Values")]
    [ConditionalEnumHide("curveType", 2)]
    [SerializeField] float q_Slope = 1;
    [ConditionalEnumHide("curveType", 2)]
    [SerializeField] float q_Exponent = 2;
    [ConditionalEnumHide("curveType", 2)]
    [SerializeField] float q_VertShift;
    [ConditionalEnumHide("curveType", 2)]
    [SerializeField] float q_HorizShift;

    [Header("Logistic Values")]
    [ConditionalEnumHide("curveType", 3)]
    [SerializeField] float logistic_Slope = 0.15f;
    [ConditionalEnumHide("curveType", 3)]
    [SerializeField] float logistic_XShift;
    [ConditionalEnumHide("curveType", 3)]
    [SerializeField] float logistic_YShift;
    [ConditionalEnumHide("curveType", 3)]
    [SerializeField] float logistic_YScalar = 1;

    [Header("Logit Values")] //I cant really describe what thoose values do
    [Range(-0.3f, 0.3f)]
    [ConditionalEnumHide("curveType", 4)]
    [SerializeField] float logit_A = 0.035f;
    [Range(-0.3f, 0.3f)]
    [ConditionalEnumHide("curveType", 4)]
    [SerializeField] float logit_B = 0.075f;









    public float GetRemappedValueBinary(float input)
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

    public float GetRemappedValueQuadratic(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
        return Mathf.Clamp
            (
                q_Slope * Mathf.Pow(input- q_HorizShift, q_Exponent) + q_VertShift,
                0,
                1
            );
    }

    public float GetRemappedValueLinear(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
        return Mathf.Clamp
            (
                lin_Slope * input  + lin_Shift, 
                0, 
                1
            );
    }


    public float GetRemappedValueLogistic(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
  
        return Mathf.Clamp
            (
                (logistic_YScalar / (1 + Mathf.Pow(Mathf.Epsilon, -logistic_Slope * (input - (logistic_XShift + 0.5f))))) + logistic_YShift,
                0, 
                1
            );
    }

    public float GetRemappedValueLogit(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too

        return Mathf.Clamp
            (
                (Mathf.Log(input / (1 - input), Mathf.Epsilon) + logit_A) / logit_B,
                0, 
                1
            );
    }


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

            //here color the current output pixel black
            if (curveType == CurveType.Linear)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueLinear(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1), Color.black);
            }
            else if(curveType == CurveType.Quadratic)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueQuadratic(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1), Color.black);
            }
            else if(curveType == CurveType.Logistic)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueLogistic(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1), Color.black);
            }
            else if(curveType == CurveType.Logit)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueLogit(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1), Color.black);
            }
            else if(curveType == CurveType.Binary)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueBinary(x / (textureWidth * 1f)) * textureHeight), 0, textureHeight - 1), Color.black);
            }
        }

        texture.Apply();

    }

}
