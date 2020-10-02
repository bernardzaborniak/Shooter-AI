using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.PlayerLoop;

public class FMODCustomAudioSource : MonoBehaviour
{
    //has a main instance but can also create a lot of oneshot instances, manages all instances it spawns, sets the pitch & occlusion parameter of them

    #region Fields

    private FMOD.Studio.EventInstance mainInstance;   
    private HashSet<FMOD.Studio.EventInstance> oneShotInstances = new HashSet<FMOD.Studio.EventInstance>();
    private HashSet<FMOD.Studio.EventInstance> oneShotInstancesToDelete = new HashSet<FMOD.Studio.EventInstance>();

    [Tooltip("Used for the doppler Effect by FMOD, can be left null")]
    public Rigidbody audioSourcesRigidbody; 

    public bool pitchAffectedByTime = true;

    public bool occlusionEnabled = true;
    [Range(0.0f, 10.0f)]
    public float occlusionIntensity = 1f;

    private string occlusionParameterName = "Occlusion";
    private float currentOcclusion = 0.0f;
    private float nextOcclusionUpdate = 0.0f;

    #endregion


    void Update()
    {
        #region - Calculate Occlusion - 

        if (occlusionEnabled)
        {
            if (Time.time > nextOcclusionUpdate)
            {
                nextOcclusionUpdate = Time.time + FmodResonanceAudio.occlusionDetectionInterval;
                currentOcclusion = occlusionIntensity * FmodResonanceAudio.ComputeOcclusion(transform);
            }
        }
        #endregion

        #region - Manage Main Instance - 

        if (mainInstance.isValid())
        {
            if (pitchAffectedByTime)
            {
                mainInstance.setPitch(Time.timeScale);
            }

            if (occlusionEnabled)
            {
                mainInstance.setParameterByName(occlusionParameterName, currentOcclusion);
            }
        }
        #endregion

        #region - Manage OneShot Instances  -

        FMOD.Studio.PLAYBACK_STATE playbackState;

        foreach (FMOD.Studio.EventInstance oneShotInstance in oneShotInstances)
        {
            oneShotInstance.getPlaybackState(out playbackState);
            if(playbackState == FMOD.Studio.PLAYBACK_STATE.STOPPED)
            {
                oneShotInstance.release();
                oneShotInstancesToDelete.Add(oneShotInstance);
            }
            else
            {
                if (pitchAffectedByTime)
                {
                    oneShotInstance.setPitch(Time.timeScale);
                }

                if (occlusionEnabled)
                {
                    oneShotInstance.setParameterByName(occlusionParameterName, currentOcclusion);
                }
            }
        }

        foreach (FMOD.Studio.EventInstance oneShotInstance in oneShotInstancesToDelete)
        {
            oneShotInstances.Remove(oneShotInstance);
        }

        oneShotInstancesToDelete.Clear();
        #endregion
    }

    #region Main Sound

    public void Play(Guid guid)
    {
            mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            mainInstance.release();
            mainInstance = RuntimeManager.CreateInstance(guid);
            RuntimeManager.AttachInstanceToGameObject(mainInstance, transform, audioSourcesRigidbody);
            mainInstance.start();
    }

    public void Stop(bool immediate = false)
    {
        if (immediate)
        {
            mainInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
        else
        {
            mainInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        mainInstance.release();

    }

    public void SetParameter(string name, float value, bool ignoreseekspeed = false)
    {
        if (mainInstance.isValid())
        {
            mainInstance.setParameterByName(name, value, ignoreseekspeed);
        }
    }

    public void SetParameter(FMOD.Studio.PARAMETER_ID id, float value, bool ignoreseekspeed = false)
    {
        if (mainInstance.isValid())
        {
            mainInstance.setParameterByID(id, value, ignoreseekspeed);
        }
    }

    #endregion

    #region One Shot

    public  void PlayOneShot(Guid guid)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        instance.start();
        oneShotInstances.Add(instance);
        //instance.release(); we are not releasing yet because of pitch
    }

    public void PlayOneShot(Guid guid, Vector3 position)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(position));
        instance.start();
        oneShotInstances.Add(instance);
    }

    public void PlayOneShotAttached(Guid guid)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        RuntimeManager.AttachInstanceToGameObject(instance, transform, audioSourcesRigidbody);
        instance.start();
        oneShotInstances.Add(instance);
    }

    public void PlayOneShotPlayOneShotAttached(Guid guid, Transform transform)
    {
        var instance = RuntimeManager.CreateInstance(guid);
        RuntimeManager.AttachInstanceToGameObject(instance, transform, audioSourcesRigidbody);
        instance.start();
        oneShotInstances.Add(instance);
    }

    public void PlayOneShot(Guid guid, string parameterName, float parameterValue)
    {
        var OneShotInstance = RuntimeManager.CreateInstance(guid);
        OneShotInstance.set3DAttributes(RuntimeUtils.To3DAttributes(transform.position));
        OneShotInstance.setParameterByName(parameterName, parameterValue);
        OneShotInstance.start();
        oneShotInstances.Add(mainInstance);
    }

    public void PlayOneShotAttached(Guid guid, string parameterName, float parameterValue)
    {
        var OneShotInstance = RuntimeManager.CreateInstance(guid);
        RuntimeManager.AttachInstanceToGameObject(OneShotInstance, transform, audioSourcesRigidbody);
        OneShotInstance.setParameterByName(parameterName, parameterValue);
        OneShotInstance.start();
        oneShotInstances.Add(mainInstance);
    }

    #endregion

    void OnDestroy()
    {
        Stop();
    }

    private void OnDisable()
    {
        Stop();
    }

}
