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
            Shoot1000RaysTheNewMethod();
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


        Vector2 normalizedDirection = new Vector2(0f, 1f).normalized * spread;
        rays[0].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(1f, 1f).normalized * spread;
        rays[1].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(1f, 0f).normalized * spread;
        rays[2].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(1f, -1f).normalized * spread;
        rays[3].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(0f, -1f).normalized * spread;
        rays[4].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(-1f, -1f).normalized * spread;
        rays[5].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(-1f, 0f).normalized * spread;
        rays[6].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
        normalizedDirection = new Vector2(-1f, 1f).normalized * spread;
        rays[7].rotation = Quaternion.LookRotation(transform.TransformDirection(new Vector3(normalizedDirection.x, normalizedDirection.y, 1f).normalized));
    }

    Quaternion GetRotationWithSpreadAdded(float spreadAngle)
    {
        //tan(alpha) = b/a  -> tan(alpha) * a = b
        //a  = 1, b varies
        float unitSphereRadius = Mathf.Tan(spreadAngle);// * 1;

        Vector2 insideUnitCircle = Random.insideUnitCircle * unitSphereRadius;

        return Quaternion.LookRotation(transform.TransformDirection(new Vector3(insideUnitCircle.x, insideUnitCircle.y, 1f)));
    }

    void Shoot1000RaysTheNewMethod()
    {
        // Take a forward transform, rotate it by the grids y and x offset, later roatte it around y axies - the quaternion order of application is inverse (right to left).
        //Vector3 rotatedRaycastDirectionInLocalSpace = Quaternion.AngleAxis(i * 45, transform.up) * Quaternion.Euler(x + directionGridSize / 2, y + directionGridSize / 2, 0f) * Vector3.forward;

        rays[0].transform.position = transform.position;
        rays[1].transform.position = transform.position;
        rays[2].transform.position = transform.position;
        rays[3].transform.position = transform.position;

        /*rays[0].rotation = Quaternion.Euler(-bounds, -bounds, 0f);
        rays[1].rotation = transform.rotation * Quaternion.Euler(Random.Range(-bounds, bounds), Random.Range(-bounds, bounds), 0f);
        rays[2].rotation = transform.rotation * Quaternion.Euler(Random.Range(-bounds, bounds), Random.Range(-bounds, bounds), 0f);
        rays[3].rotation = transform.rotation * Quaternion.Euler(Random.Range(-bounds, bounds), Random.Range(-bounds, bounds), 0f);*/

    }

    private void OnDrawGizmos()
    {
        
    }
}
