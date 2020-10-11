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
    public BoneTransformCopyHelper[] fingersBoneTransformCopyHelpers;
    [Tooltip("We may want to ignore the fingers to improve performance")]
    public bool copyFingerBones;

    public Rigidbody[] ragdollRigidbodys;
    [Tooltip("angular Velocity will only be applied to the hips]")]
    public Rigidbody rigidBodyToApplyAngularVelocityTo;

    public float delayAfterWhichToDeactivateRigidBodies;
    float deactivateTime;

    public GameObject bakedMeshRendererGO;
    public SkinnedMeshRenderer skinnedMeshRender;
    public GameObject skeleton;
    public bool destroySkeletonAndCollidersToImprovePerformance;


    void Start()
    {
        //gameObject.SetActive(false);
    }

    void Update()
    {
        if(Time.time > deactivateTime)
        {
            OptimiseDeathEffect();
            Destroy(this);
        }

    }

    public void EnableDeathEffect(Vector3 movementVelocityAtTimeOfDeath, Vector3 angularVelocityAtTimeOfDeath, ref DamageInfo damageInfo)
    {
        transform.SetParent(null);

        // TODO /Refactor, dont use seperate methods, do only one go through rigidbodys

        CopyOriginalBonePositionsToCorpse();
        SetVelocityOfRagdoll(movementVelocityAtTimeOfDeath, angularVelocityAtTimeOfDeath/5);

        gameObject.SetActive(true);

        ApplyImpactForce(ref damageInfo);


        deactivateTime = Time.time + delayAfterWhichToDeactivateRigidBodies;
    }

    void CopyOriginalBonePositionsToCorpse()
    {
        // The corpse may be missing come bones if the head or somehting was blown off
        foreach(BoneTransformCopyHelper helper in boneTransformCopyHelpers)
        {
            helper.corpseBone.position = helper.originalBone.position;
            helper.corpseBone.rotation = helper.originalBone.rotation;
        }

        if (copyFingerBones)
        {
            foreach (BoneTransformCopyHelper helper in fingersBoneTransformCopyHelpers)
            {
                helper.corpseBone.position = helper.originalBone.position;
                helper.corpseBone.rotation = helper.originalBone.rotation;
            }
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
        
        //Debug.Log("damageDealPoint: " + damageInfo.damageDealPoint);

        if (damageInfo.force != Vector3.zero)
        {
            //rigidBodyToApplyAngularVelocityTo.AddForceAtPosition(damageInfo.force * 100000, damageInfo.damageDealPoint, ForceMode.Impulse);
            //Debug.Log("damageInfo.force: " + damageInfo.force);
            foreach (Rigidbody rb in ragdollRigidbodys)
            {

                rb.AddForceAtPosition(damageInfo.force, damageInfo.damageDealPoint, ForceMode.Impulse);
                //rb.AddExplosionForce(damageInfo.force.magnitude, damageInfo.damageDealPoint, 10);
            }
        } 
    }

    void OptimiseDeathEffect()
    {
        //1. destroy rigidbodies
        foreach (Rigidbody rb in ragdollRigidbodys)
        {
            CharacterJoint cj = rb.gameObject.GetComponent<CharacterJoint>();
            if (cj)
            {
                Destroy(cj);
            }

            Destroy(rb);
        }

        //2. bake skinned mesh into normal mesh
        Mesh staticMesh = new Mesh();
        skinnedMeshRender.BakeMesh(staticMesh);
        Material mat = skinnedMeshRender.material;
        Destroy(skinnedMeshRender);

        MeshFilter mF = bakedMeshRendererGO.AddComponent<MeshFilter>();
        mF.mesh = staticMesh;
        MeshRenderer mr = bakedMeshRendererGO.AddComponent<MeshRenderer>();
        mr.material = mat;

        if (destroySkeletonAndCollidersToImprovePerformance)
        {
            Destroy(skeleton);
        }




    }
}
