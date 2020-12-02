using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunBloomTestScript : MonoBehaviour
{
    public float spread;
    //float halfSpread;

    public Transform[] rays;

    // Start is called before the first frame update
    void Start()
    {
         
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Shoot1000RaysTheOldMethod();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            ShootRaysRandomly();
        }
    }

    void Shoot1000RaysTheOldMethod()
    {
        //shootPoint.rotation* Quaternion.Euler(Random.Range(-bloom, bloom), Random.Range(-bloom, bloom), 0f))

        /*rays[0].transform.position = transform.position;
        rays[1].transform.position = transform.position;
        rays[2].transform.position = transform.position;
        rays[3].transform.position = transform.position;

        rays[0].rotation = transform.rotation * Quaternion.Euler(-bounds, -bounds, 0f);
        rays[1].rotation = transform.rotation * Quaternion.Euler(bounds, -bounds, 0f);
        rays[2].rotation = transform.rotation * Quaternion.Euler(-bounds, bounds, 0f);
        rays[3].rotation = transform.rotation * Quaternion.Euler(bounds, bounds, 0f);*/

        rays[0].transform.position = transform.position;
        rays[1].transform.position = transform.position;
        rays[2].transform.position = transform.position;
        rays[3].transform.position = transform.position;
        rays[4].transform.position = transform.position;
        rays[5].transform.position = transform.position;
        rays[6].transform.position = transform.position;
        rays[7].transform.position = transform.position;

        Debug.Log("Mathf.Tan(spread): " + Mathf.Tan(spread*Mathf.Deg2Rad));

        Vector2 normalizedDirection = new Vector2(0f, 1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[0].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(1f, 1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[1].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(1f, 0f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[2].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(1f, -1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[3].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(0f, -1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[4].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(-1f, -1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[5].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(-1f, 0f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[6].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
        normalizedDirection = new Vector2(-1f, 1f).normalized * Mathf.Tan(spread * Mathf.Deg2Rad);
        rays[7].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f)));
    }

    /*Quaternion GetRotationWithSpreadAdded(float spreadAngle)
    {
        Utility.CalculateRandomBloomInConeShapeAroundDirection(transform,spreadAngle);

        //tan(alpha) = b/a  -> tan(alpha) * a = b
        //a  = 1, b varies
         float unitSphereRadius = Mathf.Tan(spreadAngle * Mathf.Deg2Rad);

         Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius;

         return Quaternion.LookRotation(transform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f)));
    }*/

    void ShootRaysRandomly()
    {
        for (int i = 0; i < rays.Length; i++)
        {
            rays[i].rotation = Utility.CalculateRandomBloomInConeShapeAroundTransformForward(transform, spread);
        }

    }

    private void OnDrawGizmos()
    {
        
    }
}
