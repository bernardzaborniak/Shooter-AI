using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavmeshShowPathfindingTriangles : MonoBehaviour
{
    Vector3[] vertices;
    int[] indices;
    HashSet<Vector3> triangleMiddles = new HashSet<Vector3>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            NavMeshTriangulation triangulation = NavMesh.CalculateTriangulation();

            vertices = new Vector3[(triangulation.vertices.Length)];

            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] = triangulation.vertices[i];
            }


            triangleMiddles.Clear();
            int triangleIndex = 0;

            Vector3[] currentTriangleVertices = new Vector3[3];
            foreach (int indice in triangulation.indices)
            {
                Debug.Log("index: " + triangleIndex);
                if (triangleIndex == 3)
                {
                    Debug.Log("add: "); //TODO adding things to the set seems to always add the same middle?
                    triangleIndex = 0;

                    Vector3 newTriangleMiddle = Vector3.zero;
                    for (int i = 0; i < 3; i++)
                    {
                        newTriangleMiddle += currentTriangleVertices[i];
                    }
                    Debug.Log("newTriangleMiddle * (1 / 3): " + (newTriangleMiddle * (1f / 3f)));
                    triangleMiddles.Add(newTriangleMiddle * (1f / 3f));

                    currentTriangleVertices = new Vector3[3];
                    currentTriangleVertices[triangleIndex] = vertices[indice];
                }
                else
                {
                    currentTriangleVertices[triangleIndex] = vertices[indice];
                    
                }

                triangleIndex++;
                //indices.Add(indice);
                //vertices.Add(vertex);
            }
            Debug.Log("triangleMiddles: " + triangleMiddles.Count);


        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawCube(vertices[i], new Vector3(0.2f, 2, 0.2f));
        }


        Gizmos.color = Color.yellow;
        foreach (Vector3 middle in triangleMiddles)
        {
            Gizmos.DrawCube(middle, new Vector3(0.2f, 2, 0.2f));
        }
    }
}
