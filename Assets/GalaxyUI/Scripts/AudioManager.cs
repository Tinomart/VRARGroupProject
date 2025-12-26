using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour 
{
    public static AudioManager Instance;

    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource galaxyAmbienceSource;
    [SerializeField] private AudioSource galaxySpawnSource;
    [SerializeField] private AudioSource galaxyDespawnSource;
    [SerializeField] private AudioSource starAmbienceSource;
    [SerializeField] private AudioSource starActivateSource;
    [SerializeField] private AudioSource starTravelSource;
    
    public static AudioSource AmbienceSource;
    public static AudioSource GalaxyAmbienceSource;
    public static AudioSource GalaxySpawnSource;
    public static AudioSource GalaxyDespawnSource;
    public static AudioSource StarAmbienceSource;
    public static AudioSource StarActivateSource;
    public static AudioSource StarTravelSource;
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private List<AudioSource> _tempAudioSources = new List<AudioSource>();
    void Awake() 
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

        InitializeAudioSources();

    }

    void InitializeAudioSources()
    {
        AmbienceSource = ambienceSource;
        GalaxyAmbienceSource = galaxyAmbienceSource;
        GalaxySpawnSource = galaxySpawnSource;
        GalaxyDespawnSource = galaxyDespawnSource;
        StarAmbienceSource = starAmbienceSource;
        StarActivateSource = starActivateSource;
        StarTravelSource = starTravelSource;
    }
    
    public static void PlayAudioFrom(AudioSource audioSource, GameObject gameObject, bool isScaled = true)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.SetParent(gameObject.transform);
        tempGO.transform.localPosition = Vector3.zero;
    
        AudioSource tempSource = tempGO.AddComponent<AudioSource>();
    
        // Copy settings from your reference AudioSource
        tempSource.clip = audioSource.clip;
        tempSource.volume = audioSource.volume;
        tempSource.spatialBlend = audioSource.spatialBlend;
        tempSource.pitch = audioSource.pitch;
        tempSource.panStereo = audioSource.panStereo;
        tempSource.reverbZoneMix = audioSource.reverbZoneMix;
        tempSource.dopplerLevel = audioSource.dopplerLevel;
        tempSource.spread = audioSource.spread;
        tempSource.minDistance = audioSource.minDistance;
        tempSource.maxDistance = audioSource.maxDistance;
        tempSource.rolloffMode = audioSource.rolloffMode;
        tempSource.loop = audioSource.loop;
        tempSource.priority = audioSource.priority;
        
        if (isScaled)
        {
            ScaledAudioSource scaledSource = tempGO.AddComponent<ScaledAudioSource>();
            scaledSource.audioSource = tempSource;
        }
        // ... copy any other settings you need
    
        tempSource.Play();
        if (!tempSource.loop)
        {
            Destroy(tempGO, audioSource.clip.length);
        }
        else
        {
            Instance._tempAudioSources.Add(tempSource);
        }
        
    }

    public static void RemoveTempAudioSourceFrom(GameObject gameObject)
    {
        GameObject tempAudio = gameObject.transform.Find("TempAudio").gameObject;
        if (tempAudio)
        {
            Destroy(tempAudio);
        }
    }

    public static void ResetTempAudio()
    {
        foreach (AudioSource tempAudio in Instance._tempAudioSources)
        {
            Destroy(tempAudio.gameObject);
        }
    }
    
    public static void PauseAudio()
    {
        foreach (AudioSource tempAudio in Instance._tempAudioSources)
        {
            tempAudio.Pause();
        }
        foreach (AudioSource audio in Instance._audioSources)
        {
            audio.Pause();
        }
    }

    public static void ResumeAudio()
    {
        foreach (AudioSource tempAudio in Instance._tempAudioSources)
        {
            tempAudio.Play();
        }
        foreach (AudioSource audio in Instance._audioSources)
        {
            audio.Play();
        }
    }

    
}
