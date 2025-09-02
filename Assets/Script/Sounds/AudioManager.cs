using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    private static bool _isQuitting = false;

    public static AudioManager Instance
    {
        get
        {
            if (_isQuitting) return null;
            if (instance == null) CreateOrGetInstance();
            return instance;
        }
    }
    private AudioSource audioSource;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterVolumeParam = "MusicVolume";
    [SerializeField] private SoundList[] soundList;

    private static void CreateOrGetInstance()
    {
        instance = FindObjectOfType<AudioManager>();
        if (instance == null)
        {
            GameObject go = new GameObject("AudioManager");
            instance = go.AddComponent<AudioManager>();
            DontDestroyOnLoad(go);
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (!Application.isPlaying) return;
        audioSource = GetComponent<AudioSource>();
    }

  
    public void PlaySound(SoundType sound, Action<float> setVolumeCallback = null)
    {
        AudioClip[] clips = instance.soundList[(int)sound].Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip randomClip = clips[UnityEngine.Random.Range(0, clips.Length)];
        float volume = 1f;
        setVolumeCallback?.Invoke(volume);
        instance.audioSource.PlayOneShot(randomClip, volume);
    }

    public void SetVolume(float volume, string volumeGroup)
    {
        if (audioMixer == null) return;

        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(volumeGroup, dB);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);
        for (int i = 0; i < soundList.Length; i++)
        {
            soundList[i].name = names[i];
        }
    }
#endif
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
    public AudioClip[] Sounds { get => sounds; }
}