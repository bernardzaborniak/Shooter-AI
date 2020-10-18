using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Post : MonoBehaviour
{
    public bool used;
    public GameEntity usingEntity;

    [Header("Debug")]
    public Mesh cylinderMeshForGizmos;
    public float gizmoScale;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetPostPosition()
    {
        return transform.position;
    }
}
