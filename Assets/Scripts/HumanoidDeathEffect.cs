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


    //public Rigidbody

    void Start()
    {
        //gameObject.SetActive(false);
    }

    void Update()
    {
        
    }

    public void EnableDeathEffect()//Vector3 movementForceAtTimeOfDeath, Vector3 impactForceOfKillingDamage, Vector3 killingDamagePosition)
    {
        Debug.Log("Enable death effect");
        transform.SetParent(null);

        CopyOriginalBonePositionsToCorpse();

        gameObject.SetActive(true);

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
}
