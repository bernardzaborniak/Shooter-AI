using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration", fileName = "AI Decision Consideration")]
public class Consideration : ScriptableObject
{
    public string considerationName;
    [TextArea]
    public string description;

    // Input

    //Respnonse Cirve parameters & Type

    // Input parameters ( min, max, tags, etc..)
    [Space(20)]
    public ConsiderationCurve considerationCurve;

    [Header("Curve Visualisation")]
    public int horizontalVisualisationTextureResolution = 250;
    public int verticalVisualisationTextureResolution = 250;



}
