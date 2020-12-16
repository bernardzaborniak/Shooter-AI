using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// too learned from https://learn.unity.com/tutorial/property-drawers-and-custom-inspectors#
public class Tool_AlignWithGround : MonoBehaviour
{
    [MenuItem ("Tools/Transform Tools/Align with ground %t")]
    static void Align()
    {
        Transform[] transforms = Selection.transforms;
        
        foreach (Transform transform in transforms)
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, - Vector3.up, out hit))
            {
                Vector3 targetPosition = hit.point;            
                transform.position = targetPosition;
                Vector3 targetRotation = new Vector3(hit.normal.x, transform.eulerAngles.y, hit.normal.z);
                transform.eulerAngles = targetRotation;
            }
        }
    }

    [MenuItem("Tools/Transform Tools/Align with ground with bounds %t")]
    static void AlignWithBounds()
    {
        Transform[] transforms = Selection.transforms;

        foreach (Transform transform in transforms)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -Vector3.up, out hit))
            {
                Vector3 targetPosition = hit.point;
                if(transform.gameObject.GetComponent<MeshFilter>() != null)
                {
                    Bounds bounds = transform.gameObject.GetComponent<MeshFilter>().sharedMesh.bounds;
                    targetPosition.y += bounds.extents.y;
                }
                transform.position = targetPosition;
                Vector3 targetRotation = new Vector3(hit.normal.x, transform.eulerAngles.y, hit.normal.z);
                transform.eulerAngles = targetRotation;
            }
        }
    }

}
