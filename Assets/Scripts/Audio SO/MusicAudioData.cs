using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/MusicAudioData", fileName = "Music Audio Data")]
public class MusicAudioData : AudioData
{
    [EventRef]
    [SerializeField]
    string ambientMusicPath;
    public Guid ambientMusic;

    [EventRef]
    [SerializeField]
    string loadingMusicPath;
    public Guid loadingMusic;

    [EventRef]
    [SerializeField]
    string parcourActionMusic1Path;
    public Guid parcourActionMusic1;

    [EventRef]
    [SerializeField]
    string parcourActionMusic2Path;
    public Guid parcourActionMusic2;

    [EventRef]
    [SerializeField]
    string parcourActionMusic3Path;
    public Guid parcourActionMusic3;

    [EventRef]
    [SerializeField]
    string parcourActionMusic4Path;
    public Guid parcourActionMusic4;

    [EventRef]
    [SerializeField]
    string parcourActionMusic5Path;
    public Guid parcourActionMusic5;



    public override void SetUpGUIDS()
    {
        ambientMusic = RuntimeManager.PathToGUID(ambientMusicPath);
        loadingMusic = RuntimeManager.PathToGUID(loadingMusicPath);
        parcourActionMusic1 = RuntimeManager.PathToGUID(parcourActionMusic1Path);
        parcourActionMusic2 = RuntimeManager.PathToGUID(parcourActionMusic2Path);
        parcourActionMusic3 = RuntimeManager.PathToGUID(parcourActionMusic3Path);
        parcourActionMusic4 = RuntimeManager.PathToGUID(parcourActionMusic4Path);
        parcourActionMusic5 = RuntimeManager.PathToGUID(parcourActionMusic5Path);

    }
}
