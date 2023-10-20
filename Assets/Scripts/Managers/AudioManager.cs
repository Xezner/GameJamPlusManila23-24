using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : SingletonPersistent<AudioManager>
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Audio Data Scriptable Object")]
    [SerializeField] private AudioDataScriptableObject _audioData;

    [Header("Game State Data Scriptable Object")]
    [SerializeField] private GameStateDataScriptableObject _gameStateData;

    // Start is called before the first frame update
    void Start()
    {
        //Subscribe to the events
        _audioData.OnVolumeUpdate += Instance_OnVolumeUpdate;
        _audioData.OnBGMUpdate += Instance_OnBGMUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        if(_gameStateData.CurrentGameState == GameState.IsPaused)
        {
            return;
        }
    }

    //Method to call when OnVolumeUpdate event is invoked
    private void Instance_OnVolumeUpdate(object sender, System.EventArgs e)
    {
        _audioSource.volume = _audioData.DefaultBGMVolume * _audioData.BGMVolume;
    }

    //Method to call when OnBGMUpdate is invoked event is invoked
    private void Instance_OnBGMUpdate(object sender, AudioDataScriptableObject.OnBGMUpdateEventArgs bgmUpdateEvent)
    {
        ChangeBackgroundMusic(bgmUpdateEvent.BackgroundMusic);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * _audioData.SFXVolume);
    }
    private void PlaySoundArray(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, volume);
    }

    private void ChangeBackgroundMusic(AudioClip backgroundMusic)
    {
        _audioSource.Stop();
        _audioSource.clip = backgroundMusic;
        _audioSource.Play();
    }
}
