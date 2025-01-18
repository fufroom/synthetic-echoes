using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [System.Serializable]
    public class Sound
    {
        public string name;                      // Name of the sound
        public List<AudioClip> clips;           // List of audio clips
        public bool playInLeftChannel = true;   // Play in the left channel
        public bool playInRightChannel = true;  // Play in the right channel
    }

    [Header("Audio Setup")]
    public List<Sound> sounds;
    public AudioMixerGroup audioMixerGroup;

    private Dictionary<string, Sound> soundDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        InitializeSoundDictionary();
    }

    private void InitializeSoundDictionary()
    {
        soundDictionary = new Dictionary<string, Sound>();
        foreach (var sound in sounds)
        {
            soundDictionary[sound.name] = sound;
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out Sound sound))
        {
            if (sound.clips.Count > 0)
            {
                // Randomly select a clip
                AudioClip clipToPlay = sound.clips[Random.Range(0, sound.clips.Count)];

                // Spawn a new GameObject to handle playback
                GameObject soundPlayer = new GameObject($"Sound_{soundName}");
                AudioSource audioSource = soundPlayer.AddComponent<AudioSource>();

                // Configure the AudioSource
                audioSource.clip = clipToPlay;
                audioSource.outputAudioMixerGroup = audioMixerGroup;
                audioSource.panStereo = GetStereoPan(sound);

                // Play the sound and destroy the GameObject after it finishes
                audioSource.Play();
                Destroy(soundPlayer, clipToPlay.length);
            }
        }
        else
        {
            Debug.LogWarning($"Sound '{soundName}' not found!");
        }
    }

    private float GetStereoPan(Sound sound)
    {
        if (sound.playInLeftChannel && !sound.playInRightChannel)
            return -1.0f; // Left channel only
        if (!sound.playInLeftChannel && sound.playInRightChannel)
            return 1.0f;  // Right channel only
        return 0.0f;      // Both channels
    }

    public void SetVolume(float volume)
    {
        audioMixerGroup.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20); // Converts [0, 1] to decibel scale
    }
}
