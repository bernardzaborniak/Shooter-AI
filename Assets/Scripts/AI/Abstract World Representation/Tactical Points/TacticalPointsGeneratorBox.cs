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

    public float minDistanceOfGeneratedPointToNavmeshVertex;
    Vector3[] navmeshVertices;

    private void Start()
    {
        generatorBoundingBox = GetComponent<BoxCollider>();
    }

    public void Generate()
    {
        Debug.Log("Generate clicked");
        #region Get Navmesh Vertices & Tris Middles
        //The vertices and middle points of the navmesh triangles are used, to improve the point posiitoning - a rigid grid isnt always the best

        NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

        // Save the Vertices
        navmeshVertices = new Vector3[triangulation.vertices.Length];
        for (int i = 0; i < triangulation.vertices.Length; i++)
        {
            navmeshVertices[i] = triangulation.vertices[i];
        }

        // Save the Triangle Middle Points
        HashSet<Vector3> navmeshTriangleMiddles = new HashSet<Vector3>();
        int triangleIndex = 0;
        Vector3[] currentTriangleVertices = new Vector3[3];
        foreach (int indice in triangulation.indices)
        {
            //the triangulation.indices have the indexes of the vertexes which for mtriangles together, always in groups of 3
            if (triangleIndex == 3)
            {
                triangleIndex = 0;

                Vector3 newTriangleMiddle = Vector3.zero;
                for (int i = 0; i < 3; i++)
                {
                    newTriangleMiddle += currentTriangleVertices[i];
                }
                navmeshTriangleMiddles.Add(newTriangleMiddle * (1f / 3f));

                currentTriangleVertices = new Vector3[3];
                currentTriangleVertices[triangleIndex] = navmeshVertices[indice];
            }
            else
            {
                currentTriangleVertices[triangleIndex] = navmeshVertices[indice];
            }
            triangleIndex++;
        }


        #endregion

        #region Destroy Previous Children

        tacticalPointsToDestroy.Clear();
        foreach (Transform generatedPoint in transform) //theyre all children
        {
            tacticalPointsToDestroy.Add(generatedPoint.gameObject);
        }

        foreach (GameObject generatedPoint in tacticalPointsToDestroy) //theyre all children
        {
            DestroyImmediate(generatedPoint);
        }

        #endregion

        #region Spawn new Points

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
                    Vector3 spawnPosition = hit.position;

                    // 1. check if its not too close to the edge of the navmesh , 
                    Vector3 closestEdge = Vector3.zero;
                    if(NavMesh.FindClosestEdge(spawnPosition, out hit, NavMesh.AllAreas))
                    {
                        closestEdge = hit.position;
                    }

                    Vector3 closestTriangleMiddle = Vector3.zero;
                    float closesTriMiddleSquaredDistance = Mathf.Infinity;
                    foreach (Vector3 middle in navmeshTriangleMiddles)
                    {
                        float currentSquaredDistance = (middle - spawnPosition).sqrMagnitude;

                        if(currentSquaredDistance< closesTriMiddleSquaredDistance)
                        {
                            closesTriMiddleSquaredDistance = currentSquaredDistance;
                            closestTriangleMiddle = middle;
                        }
                    }

                    // 2. if its coser to the edge than to a tri middle point -> move towards tri middle point
                    if ((closestEdge - spawnPosition).sqrMagnitude < closesTriMiddleSquaredDistance)
                    {
                        Vector3 directionTowardsNearestTriMiddle = closestTriangleMiddle - spawnPosition;
                        directionTowardsNearestTriMiddle.y = 0;
                        spawnPosition += directionTowardsNearestTriMiddle * 0.5f;
                    }

                    // 3. After moving towards middle point, resample on Navmesh again
                    if (NavMesh.SamplePosition(spawnPosition, out hit, manager.maxSnapDistanceToNavmesh, NavMesh.AllAreas))
                    {
                        spawnPosition = hit.position;
                    }



                    //4. Spawn while not loosing the prefab reference
                    string prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(manager.openFieldPointPrefab);
                    //Get prefab object from path
                    Object prefab = AssetDatabase.LoadAssetAtPath(prefabPath, typeof(Object));
                    //Instantiate the prefab in the scene, as a sibling of current gameObject
                    GameObject spawnedPoint = PrefabUtility.InstantiatePrefab(prefab, transform) as GameObject;
                    spawnedPoint.transform.position = spawnPosition;
                    spawnedPoint.transform.rotation = Quaternion.identity; //let them all have the same rotation for easier comparing

                    TacticalPoint tacticalPoint = spawnedPoint.GetComponent<TacticalPoint>();
                    tacticalPoint.capacity = generatedPointCapacity;
                    tacticalPoint.radius = generatedPointRadius;

                }
            }
        }

        #endregion
    }


    public void BakeCoverRatings()
    {
        //Debug.Log("BakeCoverDistanceRating clicked");
        foreach (Transform generatedPoint in transform) //theyre all children
        {
            TacticalPoint point = generatedPoint.GetComponent<TacticalPoint>();
            point.BakeCoverRatings(manager.crouchedCoverHeight, manager.standingCoverHeight, manager.raycastsPerCoverRating, manager.raycastLayerMask, manager.maxCoverRayLength);
        }
    }

    public void DeleteAllGeneratedPoints()
    {
        tacticalPointsToDestroy.Clear();
        foreach (Transform generatedPoint in transform) //theyre all children
        {
            tacticalPointsToDestroy.Add(generatedPoint.gameObject);
        }

        foreach (GameObject generatedPoint in tacticalPointsToDestroy) //theyre all children
        {
            DestroyImmediate(generatedPoint);
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
