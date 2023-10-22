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

        _audioData.UpdateAudioVolumes(_audioData.BGMVolume, _audioData.SFXVolume);
        _audioData.UpdateBackgroundMusic(_audioData.BackgroundMusic);
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
        _audioSource.PlayOneShot(audioClip, volume * _audioData.SFXVolume);
        //udioSource.PlayClipAtPoint(audioClip, position, volume * _audioData.SFXVolume);
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

    public void PlayDeathSound()
    {
        PlaySound(_audioData.SFXClips.DeathSFX, transform.position);
    }

    public void PlayGameOverSFX()
    {
        PlaySound(_audioData.SFXClips.GameOverSFX, transform.position);
    }

    public void PlayJumpSound()
    {
        PlaySoundArray(_audioData.SFXClips.JumpSFX, transform.position);
    }

    public void PlayInteractSFX()
    {
        PlaySoundArray(_audioData.SFXClips.InteractSFX, transform.position);
    }
}
