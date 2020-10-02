using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetUpAudioSOHelper : MonoBehaviour
{
    /*public PlayerAudioData playerAudioData;
    public GunAudioData[] gunAudioData;
    public EnvironmentAudioData environmentAudioData;*/
    public AudioData[] audioData;

    void Awake()
    {
        /*playerAudioData.SetUpGUIDS();
        for (int i = 0; i < gunAudioData.Length; i++)
        {
            gunAudioData[i].SetUpGUIDS();
        }
        environmentAudioData.SetUpGUIDS();*/
        for (int i = 0; i < audioData.Length; i++)
        {
            audioData[i].SetUpGUIDS();
        }
    }

 
}
