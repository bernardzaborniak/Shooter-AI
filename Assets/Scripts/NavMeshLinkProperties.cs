using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public enum NavMeshLinkType
{
    DefaultLinearLink,
    JumpOverObstacle,
    JumpOverHole,
    //Maybe to add later or they are the same as jump over hole
    JumpDown,
    JumpUp
}

[RequireComponent(typeof(NavMeshLink))]
public class NavMeshLinkProperties : MonoBehaviour
{
    public NavMeshLinkType navMeshLinkType;
    [Tooltip("only used if type is JumpOverObstacle")]
    public float obstacleHeight;
}
