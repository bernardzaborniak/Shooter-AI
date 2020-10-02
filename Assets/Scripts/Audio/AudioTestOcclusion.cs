using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTestOcclusion : MonoBehaviour
{
    public FMODCustomAudioSource FMODCustomAudioSource;
    [FMODUnity.EventRef]
    public string music;
    public System.Guid musicGUID;
    [FMODUnity.EventRef]
    public string oneShot;
    public System.Guid oneShotGUID;


    public bool oneShotTest;
    float nextShotTime;
    float shotInterval = 0.15f;


    // Start is called before the first frame update
    void Start()
    {
        musicGUID = RuntimeManager.PathToGUID(music);
        oneShotGUID = RuntimeManager.PathToGUID(oneShot);

        if (!oneShotTest)
        {
            FMODCustomAudioSource.Play(musicGUID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oneShotTest)
        {
            if(Time.time > nextShotTime)
            {
                nextShotTime = Time.time + shotInterval;
                FMODCustomAudioSource.PlayOneShot(oneShotGUID);
            }
        }

    }
}
