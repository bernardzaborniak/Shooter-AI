using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider))]
public class TacticalPointsGeneratorBox : MonoBehaviour
{
    HashSet<GameObject> tacticalPointsToDestroy = new HashSet<GameObject>();
    [Range(0,5)]
    public int testInt;

    TacticalPointsManager manager;

    [Header("Generation Params")]
    public float gridSize;
    public float generatedPointRadius;
    public int generatedPointCapacity;
    BoxCollider generatorBoundingBox;

    private void Start()
    {
        generatorBoundingBox = GetComponent<BoxCollider>();
    }

    public void Generate()
    {
        Debug.Log("Generate clicked");

        tacticalPointsToDestroy.Clear();
        foreach (Transform generatedPoint in transform) //theyre all children
        {
            tacticalPointsToDestroy.Add(generatedPoint.gameObject);
        }

        foreach (GameObject generatedPoint in tacticalPointsToDestroy) //theyre all children
        {
            DestroyImmediate(generatedPoint);
        }


        //get bounding box / collider box edges
        Vector3 lowerLeftPosition = generatorBoundingBox.center - generatorBoundingBox.size/2;
        Vector3 upperRightPosition = generatorBoundingBox.center + generatorBoundingBox.size / 2;
        lowerLeftPosition.y = 0;
        upperRightPosition.y = 0;


        Debug.Log("x from " + lowerLeftPosition.x + " to " + upperRightPosition.x);
        Debug.Log("x from " + lowerLeftPosition.z + " to " + upperRightPosition.z);
        //go through a grid
        for (float x = lowerLeftPosition.x; x <= upperRightPosition.x; x = x + gridSize)
        {
            for (float y = lowerLeftPosition.z; y <= upperRightPosition.z; y = y + gridSize)
            {
                //snap position to navmesh
                //maxSnapDistanceToNavmesh

                NavMeshHit hit;
                if(NavMesh.SamplePosition(transform.TransformPoint(new Vector3(x, 0, y)), out hit, manager.maxSnapDistanceToNavmesh, NavMesh.AllAreas))
                {
                    GameObject spawnedPoint = Instantiate(manager.openFieldPointPrefab, transform);
                    spawnedPoint.transform.position = hit.position;

                    TacticalPoint tacticalPoint = spawnedPoint.GetComponent<TacticalPoint>();
                    tacticalPoint.capacity = generatedPointCapacity;
                    tacticalPoint.radius = generatedPointRadius;

                }


            }
        }
    }

    public void BakeCoverDistanceRating()
    {
        Debug.Log("BakeCoverDistanceRating clicked");
    }

    public void BakeCoverQualityRating()
    {
        Debug.Log("BakeCoverQualityRating clicked");
    }

    void OnEnable()
    {
        TacticalPointsManager.Instance.AddTacticalPointsGeneratorBox(this);
        manager = TacticalPointsManager.Instance;
    }

    void OnDisable()
    {
        TacticalPointsManager.Instance.RemoveTacticalPointsGeneratorBox(this);
    }

}
