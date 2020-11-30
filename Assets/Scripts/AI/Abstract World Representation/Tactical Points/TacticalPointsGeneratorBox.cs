using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

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
                NavMeshHit hit;
                if(NavMesh.SamplePosition(transform.TransformPoint(new Vector3(x, 0, y)), out hit, manager.maxSnapDistanceToNavmesh, NavMesh.AllAreas))
                {
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(manager.openFieldPointPrefab);
                    //Get prefab object from path
                    Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
                    //Instantiate the prefab in the scene, as a sibling of current gameObject
                    GameObject spawnedPoint = PrefabUtility.InstantiatePrefab(prefab, transform.parent) as GameObject;
                    spawnedPoint.transform.SetParent(transform);
                    spawnedPoint.transform.position = hit.position;
                    spawnedPoint.transform.rotation = Quaternion.identity; //let them all have the same rotation for easier comparing

                    TacticalPoint tacticalPoint = spawnedPoint.GetComponent<TacticalPoint>();
                    tacticalPoint.capacity = generatedPointCapacity;
                    tacticalPoint.radius = generatedPointRadius;

                }
            }
        }  
    }

    public void BakeCoverRatings()
    {
        //Debug.Log("BakeCoverDistanceRating clicked");
        foreach (Transform generatedPoint in transform) //theyre all children
        {
            TacticalPoint point = generatedPoint.GetComponent<TacticalPoint>();
            point.BakeCoverRatings(manager.crouchedCoverHeight, manager.standingCoverHeight, manager.raycastsPerCoverRating, manager.raycastLayerMask);
        }
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
