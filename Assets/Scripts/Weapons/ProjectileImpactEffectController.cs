using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileImpactEffectController : MonoBehaviour
{
    public float stayActiveDelay;
    public ParticleSystem visualsParticleSystem;

    public void EnableImpactEffect(Vector3 position, Vector3 normal)
    {
        //Deparent
        transform.SetParent(null);

        //DIsable after Delay
        Invoke("DisableAfterDelay", stayActiveDelay);

        //Set Position
        transform.position = position;

        //Set Allginment
        if (normal != Vector3.zero)
        {
            transform.up = normal;
        }

        // Play Particle
        visualsParticleSystem.Play();
    }

    public void Reset(Transform parent)
    {
        gameObject.SetActive(true);
        transform.SetParent(parent);
        transform.localPosition = new Vector3(0, 0, 0);
    }

    void DisableAfterDelay()
    {
        gameObject.SetActive(false);
    }
}
