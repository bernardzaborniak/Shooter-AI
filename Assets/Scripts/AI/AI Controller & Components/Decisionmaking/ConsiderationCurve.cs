using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Curves constructed according to Daves Mark's GDC talk: "Architecture Tricks: Managing Behaviors in Time, Space, and Depth" 
// found here: https://www.gdcvault.com/play/1018040/Architecture-Tricks-Managing-Behaviors-in


[System.Serializable]
public class ConsiderationCurve 
{

    [Tooltip("slope / slope of the line at inflection point")]
    [SerializeField] float m;
    [Tooltip("exponent / vertical size of the curve")]
    [SerializeField] float k;
    [Tooltip("y-intercept (vertical shift)")]
    [SerializeField] float b;
    [Tooltip("x-intercept / x-intercept at the inflection point (horizontal shift)")]
    [SerializeField] float c;

    [Header("Binary Values")]
    [Range(0,1)]
    [SerializeField] float bi_threshold;
    [SerializeField] bool bi_inverse;

    [Header("Linear Values")]
    [SerializeField] float lin_Slope;
    [SerializeField] float lin_Shift;

    [Header("Quadratic Values")]
    [SerializeField] float q_Slope;
    [SerializeField] float q_Exponent;
    [SerializeField] float q_VertShift;
    [SerializeField] float q_HorizShift;




    public enum CurveType
    {
        Linear,
        Quadratic,
        Logistic,
        Logit,
        Binary
    }

    public CurveType curveType;


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


        return Mathf.Clamp(q_Slope * Mathf.Pow(input- q_HorizShift, q_Exponent) + q_VertShift, 0,1);
    }

    public float GetRemappedValueLinear(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too


        return Mathf.Clamp(lin_Slope * input  + lin_Shift, 0, 1);
    }


    public float GetRemappedValueLogistic(float input)
    {
        // Input should be between 0 and 1, return is between 0 and 1 too
        //return Mathf.Clamp((k*(1/(1+Mathf.Pow(1000*Mathf.Epsilon*m,-1*input+c))))+b, 0, 1);
        //return Mathf.Clamp((k*(1/(1+1000*Mathf.Epsilon* Mathf.Pow(m,-1*input+c))))+b, 0, 1);

       // float bruch = 1 / (1+ Mathf.Pow(Mathf.Epsilon* m, -1 * input + c));
        float bruch = 1 / (1+ Mathf.Pow(Mathf.Epsilon, -input + c));
        //float bruch = 1 / (1+ Mathf.Pow(Mathf.Epsilon, -input + c));
        //float value = (k * (bruch) + b);
        
        
        //float value = 1 / (1 + Mathf.Pow(Mathf.Epsilon * m, -input + c));
        float value = 1f / (1 + Mathf.Pow(Mathf.Epsilon * m, -input + c));
        return Mathf.Clamp(value, 0, 1);
    }

    public void GetCurveVisualisationTexture(ref Texture2D texture)
    {
        int textureResolution = texture.height;


        for (int x = 0; x < textureResolution; x++)
        {
            for (int y = 0; y < textureResolution; y++)
            {
                //colour everything white
                texture.SetPixel(x, y, Color.white);
            }

            //here color the current output pixel black

            if (curveType == CurveType.Linear)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueLinear(x / (textureResolution * 1f)) * textureResolution), 0, textureResolution - 1), Color.black);
            }
            else if(curveType == CurveType.Quadratic)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueQuadratic(x / (textureResolution * 1f)) * textureResolution), 0, textureResolution - 1), Color.black);
            }
            else if(curveType == CurveType.Logistic)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueLogistic(x / (textureResolution * 1f)) * textureResolution), 0, textureResolution - 1), Color.black);
            }
            else if(curveType == CurveType.Binary)
            {
                texture.SetPixel(x, Mathf.Clamp((int)(GetRemappedValueBinary(x / (textureResolution * 1f)) * textureResolution), 0, textureResolution - 1), Color.black);
            }
        }

        texture.Apply();

    }

}
