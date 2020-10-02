using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FMODUnity;

[CreateAssetMenu(menuName = "SO/Audio/GameFlowAndUIAudioData", fileName = "GameFlow And UI Audio Data")]
public class GameFlowAndUIAudioData : AudioData
{
    [EventRef]
    [SerializeField]
    string respawnSoundPath;
    public Guid respawnSound;

    public override void SetUpGUIDS()
    {
        respawnSound = RuntimeManager.PathToGUID(respawnSoundPath);
    }
}

