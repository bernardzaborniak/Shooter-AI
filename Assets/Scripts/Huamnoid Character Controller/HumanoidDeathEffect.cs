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

        for (int i = 0; i < boneTransformCopyHelpers.Length; i++)
        {
            boneTransformCopyHelpers[i].corpseBone.position = boneTransformCopyHelpers[i].originalBone.position;
            boneTransformCopyHelpers[i].corpseBone.rotation = boneTransformCopyHelpers[i].originalBone.rotation;
        }

        if (copyFingerBones)
        {
            for (int i = 0; i < fingersBoneTransformCopyHelpers.Length; i++)
            {
                fingersBoneTransformCopyHelpers[i].corpseBone.position = fingersBoneTransformCopyHelpers[i].originalBone.position;
                fingersBoneTransformCopyHelpers[i].corpseBone.rotation = fingersBoneTransformCopyHelpers[i].originalBone.rotation;
            }
        }
    }

    void SetVelocityOfRagdoll(Vector3 movementVelocityAtTimeOfDeath, Vector3 angularVelocityAtTimeOfDeath)
    {
        for (int i = 0; i < ragdollRigidbodys.Length; i++)
        {
            ragdollRigidbodys[i].velocity = movementVelocityAtTimeOfDeath;
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
            
            for (int i = 0; i < ragdollRigidbodys.Length; i++)
            {

                ragdollRigidbodys[i].AddForceAtPosition(damageInfo.force, damageInfo.damageDealPoint, ForceMode.Impulse);
                //rb.AddExplosionForce(damageInfo.force.magnitude, damageInfo.damageDealPoint, 10);
            }
        } 
    }

    void OptimiseDeathEffect()
    {
        //1. destroy rigidbodies
        for (int i = 0; i < ragdollRigidbodys.Length; i++)
        {
            CharacterJoint cj = ragdollRigidbodys[i].gameObject.GetComponent<CharacterJoint>();
            if (cj)
            {
                Destroy(cj);
            }

            Destroy(ragdollRigidbodys[i]);
        }

        //2. bake skinned mesh into normal mesh
        Mesh staticMesh = new Mesh();
        skinnedMeshRender.BakeMesh(staticMesh);
        Destroy(skinnedMeshRender);

        MeshFilter mF = bakedMeshRendererGO.AddComponent<MeshFilter>();
        mF.mesh = staticMesh;
        MeshRenderer mr = bakedMeshRendererGO.AddComponent<MeshRenderer>();
        mr.material = skinnedMeshRender.material;

        if (destroySkeletonAndCollidersToImprovePerformance)
        {
            Destroy(skeleton);
        }




    }
}
