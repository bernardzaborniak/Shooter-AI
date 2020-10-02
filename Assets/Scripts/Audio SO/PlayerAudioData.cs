using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/PlayerAudioData", fileName = "Player Audio Data")]
public class PlayerAudioData : AudioData
{

    #region Movement Sounds
    [Header("Audio - Movement")]

    [EventRef]
    [SerializeField]
    string jumpSoundPath;
    public Guid jumpSound;

    [EventRef]
    [SerializeField]
    string standinUpSoundPath;
    public Guid standinUpSound;

    [EventRef]
    [SerializeField]
    string runningUpWallSoundPath;
    public Guid runningUpWallSound;

    [EventRef]
    [SerializeField]
    string slideSoundPath;
    public Guid slideSound;

    [EventRef]
    [SerializeField]
    string runningSoundPath;
    public Guid runningSound;

    [EventRef]
    [SerializeField]
    string wallrunningSoundPath;
    public Guid wallrunningSound;

    #endregion

    #region Hand Sounds
    [Header("Audio - Hands")]

    [FMODUnity.EventRef]
    [SerializeField]
    string grabWeaponSoundPath;
    public Guid grabWeaponSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string releaseWeaponSoundPath;
    public Guid releaseWeaponSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string grabClimbingPointSoundPath;
    public Guid grabClimbingPointSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string releaseClimbingPointSoundPath;
    public Guid releaseClimbingPointSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string grabGrabbableSoundPath;
    public Guid grabGrabbableSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string releaseGrabbableSoundPath;
    public Guid releaseGrabbableSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string grabHolsteredWeaponSoundPath;
    public Guid grabHolsteredWeaponSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string holsterWeaponSoundPath;
    public Guid holsterWeaponSound;

    #endregion

    #region Shooting Feedback
    [Header("Audio - Shooting Feedback")]

    [FMODUnity.EventRef]
    [SerializeField]
    string hitmarkerSoundPath;
    public Guid hitmarkerSound;

    [FMODUnity.EventRef]
    [SerializeField]
    string killSoundPath;
    public Guid killSound;

    #endregion

    public override void SetUpGUIDS()
    {
        Debug.Log("Set Up Player Audio Data");

        jumpSound = RuntimeManager.PathToGUID(jumpSoundPath);
        standinUpSound = RuntimeManager.PathToGUID(standinUpSoundPath);
        runningUpWallSound = RuntimeManager.PathToGUID(runningUpWallSoundPath);
        slideSound = RuntimeManager.PathToGUID(slideSoundPath);
        runningSound = RuntimeManager.PathToGUID(runningSoundPath);
        wallrunningSound = RuntimeManager.PathToGUID(wallrunningSoundPath);

        grabWeaponSound = RuntimeManager.PathToGUID(grabWeaponSoundPath);
        releaseWeaponSound = RuntimeManager.PathToGUID(releaseWeaponSoundPath);
        grabClimbingPointSound = RuntimeManager.PathToGUID(grabClimbingPointSoundPath);
        releaseClimbingPointSound = RuntimeManager.PathToGUID(releaseClimbingPointSoundPath);
        grabGrabbableSound = RuntimeManager.PathToGUID(grabGrabbableSoundPath);
        releaseGrabbableSound = RuntimeManager.PathToGUID(releaseGrabbableSoundPath);
        grabHolsteredWeaponSound = RuntimeManager.PathToGUID(grabHolsteredWeaponSoundPath);
        holsterWeaponSound = RuntimeManager.PathToGUID(holsterWeaponSoundPath);

        hitmarkerSound = RuntimeManager.PathToGUID(hitmarkerSoundPath);
        killSound = RuntimeManager.PathToGUID(killSoundPath);
    }
}
