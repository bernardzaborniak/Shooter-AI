using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/EnvironmentAudioData", fileName = "Environment Audio Data")]
public class EnvironmentAudioData : AudioData
{
    [Header("Interactables")]

    [FMODUnity.EventRef]
    [SerializeField]
    string ziplineSoundPath;
    public Guid ziplineSound;

    [Header("Destroyables")]

    [FMODUnity.EventRef]
    [SerializeField]
    string enableTrainingTargetSoundPath;
    public Guid enableTrainingTargetSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string disableTrainingTargetSoundPath;
    public Guid disableTrainingTargetSound;


    public override void SetUpGUIDS()
    {
        ziplineSound = RuntimeManager.PathToGUID(ziplineSoundPath);
        enableTrainingTargetSound = RuntimeManager.PathToGUID(enableTrainingTargetSoundPath);
        disableTrainingTargetSound = RuntimeManager.PathToGUID(disableTrainingTargetSoundPath);
    }
}
