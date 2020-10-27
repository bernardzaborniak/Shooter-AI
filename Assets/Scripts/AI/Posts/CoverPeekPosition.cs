using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoverPeekPosition : MonoBehaviour
{
    // he position where a uit can peek out of its cover post
    public PositionRating positionRating;

    [Header("Debug")]
    public Mesh cylinderMeshForGizmos;
    public float gizmoScale;


    //for now we only use this to test the character controller
    public int stanceType; //0 is standing, 1 is crouching

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPeekPosition()
    {
        return transform.position;
    }

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1,0.45f,0f,0.8f);
        //Gizmos.DrawMesh(cylinderMeshForGizmos, 0, transform.position, transform.rotation, new Vector3(gizmoScale, 0.015f, gizmoScale));
        Gizmos.DrawMesh(cylinderMeshForGizmos, 0, transform.position, transform.rotation, new Vector3(0.1f, 0.015f, 0.1f));
    }
#endif
}
