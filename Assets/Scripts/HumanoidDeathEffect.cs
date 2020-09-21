using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanoidDeathEffect : MonoBehaviour
{
    [Serializable]
    public class BoneTransformCopyHelper
    {
        public Transform originalBone;
        public Transform corpseBone;
    }

    public BoneTransformCopyHelper[] boneTransformCopyHelpers;

    public Rigidbody[] ragdollRigidbodys;
    [Tooltip("angular Velocity will only be applied to the hips]")]
    public Rigidbody rigidBodyToApplyAngularVelocityTo;


    //public Rigidbody

    void Start()
    {
        //gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void EnableDeathEffect(Vector3 movementVelocityAtTimeOfDeath, Vector3 angularVelocityAtTimeOfDeath, ref DamageInfo damageInfo)
    {
        transform.SetParent(null);

        // TODO /Refactor, dont use seperate methods, do only one go through rigidbodys

        CopyOriginalBonePositionsToCorpse();
        SetVelocityOfRagdoll(movementVelocityAtTimeOfDeath, angularVelocityAtTimeOfDeath/5);

        gameObject.SetActive(true);

        ApplyImpactForce(ref damageInfo);

       

    }

    void CopyOriginalBonePositionsToCorpse()
    {
        // The corpse may be missing come bones if the head or somehting was blown off
        foreach(BoneTransformCopyHelper helper in boneTransformCopyHelpers)
        {
            helper.corpseBone.position = helper.originalBone.position;
            helper.corpseBone.rotation = helper.originalBone.rotation;
        }
    }

    void SetVelocityOfRagdoll(Vector3 movementVelocityAtTimeOfDeath, Vector3 angularVelocityAtTimeOfDeath)
    {
        foreach(Rigidbody rb in ragdollRigidbodys)
        {
            rb.velocity = movementVelocityAtTimeOfDeath;
        }

        rigidBodyToApplyAngularVelocityTo.angularVelocity = angularVelocityAtTimeOfDeath;
    }

    void ApplyImpactForce(ref DamageInfo damageInfo)
    {
        
        Debug.Log("damageDealPoint: " + damageInfo.damageDealPoint);

        if (damageInfo.force != Vector3.zero)
        {
            //rigidBodyToApplyAngularVelocityTo.AddForceAtPosition(damageInfo.force * 100000, damageInfo.damageDealPoint, ForceMode.Impulse);
            Debug.Log("damageInfo.force: " + damageInfo.force);
            foreach (Rigidbody rb in ragdollRigidbodys)
            {

                rb.AddForceAtPosition(damageInfo.force, damageInfo.damageDealPoint, ForceMode.Impulse);
                //rb.AddExplosionForce(damageInfo.force.magnitude, damageInfo.damageDealPoint, 10);
            }
        } 
    }
}
