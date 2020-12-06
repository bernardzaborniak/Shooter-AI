using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;


// this script is from the internet to- it think i created it following a tutorial, but it isnt working too well
public class CustomIK : MonoBehaviour
{
    public int chainLength;

    public Transform target;
    public Transform pole;
    public Transform tip;

    //[Header("Solver Parameters")]
    Transform[] bones;
    Vector3[] bonePositions;
    float[] bonesLength;
    float completeLength;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    void Init()
    {
        bones = new Transform[chainLength + 1];
        bonePositions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];

        completeLength = 0;

        Transform currentBone = tip;
        for (int i = chainLength; i >=0; i--)
        {
            bones[i] = currentBone;

            if(i == chainLength)
            {

            }
            else
            {
                bonesLength[i] = (bones[i + 1].position - currentBone.position).magnitude;
                completeLength += bonesLength[i];
            }

            currentBone = currentBone.parent;
        }
    }

    void ResolveIK()
    {
        if(target == null)
        {
            return;
        }
        if(bonesLength.Length != chainLength)
        {
            Init();
        }

        //get positions
        for (int i = 0; i < bones.Length; i++)
        {
            bonePositions[i] = bones[i].position;
        }

        //calculations
        if((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            //just stretch it
            Vector3 direction = (target.position - bonePositions[0]).normalized;

            for (int i = 0; i < bonePositions.Length; i++)
            {
                bonePositions[i] = bonePositions[i - 1] + direction * bonesLength[i - 1];
            }
        }


        //set positions
        for (int i = 0; i < bonePositions.Length; i++)
        {
            bones[i].position = bonePositions[i];
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Transform currentBone = tip;

        for (int i = 0; i < chainLength && currentBone != null && currentBone.parent != null; i++)
        {
            
            float scale = Vector3.Distance(currentBone.position, currentBone.parent.position) * 0.1f;
            Handles.matrix = Matrix4x4.TRS(currentBone.position, Quaternion.FromToRotation(Vector3.up, currentBone.parent.position - currentBone.position), new Vector3(scale, Vector3.Distance(currentBone.position, currentBone.parent.position), scale));
            Handles.color = Color.green;
            Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);   
            currentBone = currentBone.parent;
        }
    }

#endif

}
