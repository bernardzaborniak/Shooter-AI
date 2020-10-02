using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Is responsible for music - dont destroyOnload? - so we can have loading screen music?
public class MusicManager : MonoBehaviour
{
    public bool disableMusicForDevelopment;
    public MusicAudioData musicAudioData;
    public FMODCustomAudioSource audioSource;

    int currentActionMusic = 1;


    #region singleton code
    public static MusicManager Instance;

    void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;

            DontDestroyOnLoad(this);
        }  
    }

    #endregion

    private void Update()
    {
        // Stop the music if in slowmo.
        /*if(Time.timeScale != 1)
        {
            musicAudioSource.Stop();
        }
        else
        {
            musicAudioSource.Play();
        }*/

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            currentActionMusic = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            currentActionMusic = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            currentActionMusic = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            currentActionMusic = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            currentActionMusic = 5;
        }
    }

    public void StartPlayingAmbientMusic()
    {
        if (!disableMusicForDevelopment)
        {
            audioSource.Play(musicAudioData.ambientMusic);
        }
        
    }

    public void StartPlayingActionMusic()
    {
        if (!disableMusicForDevelopment)
        {
            if(currentActionMusic == 1)
            {
                audioSource.Play(musicAudioData.parcourActionMusic1);
            }
            else if (currentActionMusic == 2)
            {
                audioSource.Play(musicAudioData.parcourActionMusic2);
            }
            else if (currentActionMusic == 3)
            {
                audioSource.Play(musicAudioData.parcourActionMusic3);
            }
            else if (currentActionMusic == 4)
            {
                audioSource.Play(musicAudioData.parcourActionMusic4);
            }
            else if (currentActionMusic == 5)
            {
                audioSource.Play(musicAudioData.parcourActionMusic5);
            }
        }
    }

    public void StartPlayingLoadingMusic()
    {
        audioSource.Play(musicAudioData.loadingMusic);
    }
}
