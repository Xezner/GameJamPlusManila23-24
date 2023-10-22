using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioDataScriptableObject", menuName = "Scriptable Objects/Audio Data Scriptable Object")]
public class AudioDataScriptableObject : ScriptableObject
{
    [Header("Audio Data")]
    public float BGMVolume;
    public float SFXVolume;
    public float DefaultBGMVolume;

    [Header("Audio Clips")]
    [Header("BGM", order = 2)]
    public AudioClip BackgroundMusic;

    [Header("SFX", order = 2)]
    public AudioClips SFXClips;

    [Serializable]
    public struct AudioClips
    {
        public AudioClip[] JumpSFX;
        public AudioClip[] InteractSFX;
        public AudioClip DeathSFX;
        public AudioClip GameOverSFX;
        //add more audio clips here
    }

    public event EventHandler OnVolumeUpdate;
    public event EventHandler<OnBGMUpdateEventArgs> OnBGMUpdate;
    public class OnBGMUpdateEventArgs
    {
        public AudioClip BackgroundMusic;
    }

    private void Awake()
    {
        Debug.Log("ERROR");
        UpdateAudioVolumes(BGMVolume, SFXVolume);
        UpdateBackgroundMusic(BackgroundMusic);
    }

    //Call this method to invoke the OnVolumeUpdate event
    public void UpdateAudioVolumes(float bgmVolumeValue, float sfxVolumeValue)
    {
        BGMVolume = bgmVolumeValue; 
        SFXVolume = sfxVolumeValue;
        OnVolumeUpdate?.Invoke(this, EventArgs.Empty);
    }

    //Call this method to invoke the OnBGMUpdate event
    public void UpdateBackgroundMusic(AudioClip backgroundMusic) 
    {
        OnBGMUpdate?.Invoke(this, new OnBGMUpdateEventArgs
        {
            BackgroundMusic = backgroundMusic
        });
    }
}
