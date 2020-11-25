using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*#if UNITY_EDITOR
using UnityEditor;
#endif*/


/*public class TP_CoverPoint : TacticalPoint
{
   

    public PositionRating positionRating;
    [Space(10)]
    public TP_CoverShootPoint[] PeekPositions; //or ShotPositions

  

    //for now we only use this to test the character controller
    public int stanceType; //0 is standing, 1 is crouching


    



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    

#if UNITY_EDITOR
    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (used)
        {
            Gizmos.color = new Color(0.8f, 0f, 0f, 0.8f);

        }
        else
        {
            Gizmos.color = new Color(0f, 0.8f, 0f, 0.8f);
        }
        Gizmos.DrawMesh(cylinderMeshForGizmos,0,transform.position,transform.rotation,new Vector3(gizmoScale, 0.02f, gizmoScale));
        //Handles.Label(transform.position + Vector3.up*0.5f, "Cover Pos", EditorStyles.helpBox);
       // Handles.DrawSolidArc(transform.position, Vector3.up, transform.right, 150, 2);
    }
#endif
}
*/