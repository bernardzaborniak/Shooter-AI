using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/GunAudioData", fileName = "Gun Audio Data")]
public class GunAudioData : AudioData
{
    [Header("Audio")]

    [EventRef]
    [SerializeField]
    string shootSoundPath;
    public Guid shootSound;

    [EventRef]
    [SerializeField]
    string ejectShellSoundPath;
    public Guid ejectShellSound;

    [EventRef]
    [SerializeField]
    string openEjectionPortSoundPath;
    public Guid openEjectionPortSound;

    [EventRef]
    [SerializeField]
    string closeEjectionPortSoundPath;
    public Guid closeEjectionPortSound;

    [EventRef]
    [SerializeField]
    string pullTriggerNoAmmoSoundPath;
    public Guid pullTriggerNoAmmoSound;

    [EventRef]
    [SerializeField]
    string releaseMagazineSoundPath;
    public Guid releaseMagazineSound;

    [EventRef]
    [SerializeField]
    string insertMagazineSoundPath;
    public Guid insertMagazineSound;


    public override void SetUpGUIDS()
    {
        shootSound = RuntimeManager.PathToGUID(shootSoundPath);
        ejectShellSound = RuntimeManager.PathToGUID(ejectShellSoundPath);
        openEjectionPortSound = RuntimeManager.PathToGUID(openEjectionPortSoundPath);
        closeEjectionPortSound = RuntimeManager.PathToGUID(closeEjectionPortSoundPath);
        pullTriggerNoAmmoSound = RuntimeManager.PathToGUID(pullTriggerNoAmmoSoundPath);
        releaseMagazineSound = RuntimeManager.PathToGUID(releaseMagazineSoundPath);
        insertMagazineSound = RuntimeManager.PathToGUID(insertMagazineSoundPath);
    }
}
