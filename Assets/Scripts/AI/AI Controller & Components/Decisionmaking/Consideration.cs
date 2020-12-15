using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Consideration", fileName = "AI Decision Consideration")]
public class Consideration : ScriptableObject
{
    public string considerationName;
    public string description;

    // Input

    //Respnonse Cirve parameters & Type

    // Input parameters ( min, max, tags, etc..)
    public ConsiderationCurve considerationCurve;


}
