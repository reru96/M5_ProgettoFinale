using System;
using UnityEngine;
using UnityEngine.Audio;

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

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Sources")]
    [SerializeField] private AudioSource[] musicSource;
    [SerializeField] private AudioSource[] sfxSource;

    [Header("Sounds")]
    [SerializeField] private SoundList[] soundList;

    protected AudioManager() { }

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


        if (musicSource == null || musicSource.Length == 0)
        {
            musicSource = new AudioSource[1];
            musicSource[0] = gameObject.AddComponent<AudioSource>();
            musicSource[0].loop = true;
            if (audioMixer != null)
                musicSource[0].outputAudioMixerGroup = audioMixer.FindMatchingGroups("Music")[0];
        }

        if (sfxSource == null || sfxSource.Length == 0)
        {
            sfxSource = new AudioSource[1];
            sfxSource[0] = gameObject.AddComponent<AudioSource>();
            sfxSource[0].loop = false;
            if (audioMixer != null)
                sfxSource[0].outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
        }
    }

    private void OnApplicationQuit() => _isQuitting = true;

    private void OnDestroy()
    {
        if (instance == this) instance = null;
    }


    public void PlayMusic(AudioClip clip, int number, bool loop = true)
    {
        if (clip == null || number < 0 || number >= musicSource.Length) return;

        if (musicSource[number].clip == clip && musicSource[number].isPlaying) return;

        musicSource[number].clip = clip;
        musicSource[number].loop = loop;
        musicSource[number].Play();
    }

    public void StopMusic(int number)
    {
        if (number < 0 || number >= musicSource.Length) return;
        if (musicSource[number].isPlaying)
            musicSource[number].Stop();
    }

  
    public void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null || sfxSource.Length == 0) return;

        
        foreach (var source in sfxSource)
        {
            if (!source.isPlaying)
            {
                source.PlayOneShot(clip, volume);
                return;
            }
        }

        sfxSource[0].PlayOneShot(clip, volume);
    }

    public void PlaySound(SoundType sound, float volume = 1f)
    {
        if (soundList == null || soundList.Length == 0) return;

        int index = (int)sound;
        if (index < 0 || index >= soundList.Length) return;

        AudioClip[] clips = soundList[index].Sounds;
        if (clips == null || clips.Length == 0) return;

        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        PlaySound(clip, volume);
    }

   
    public void SetVolume(float sliderValue, string soundGroup)
    {
        if (audioMixer == null) return;
        float dB = (sliderValue <= 0f) ? -80f : Mathf.Log10(sliderValue) * 20f;
        audioMixer.SetFloat(soundGroup, dB);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (soundList == null) return;

        string[] names = Enum.GetNames(typeof(SoundType));
        Array.Resize(ref soundList, names.Length);

        for (int i = 0; i < soundList.Length; i++)
            soundList[i].name = names[i];
    }
#endif
}

[Serializable]
public struct SoundList
{
    [HideInInspector] public string name;
    [SerializeField] private AudioClip[] sounds;
    public AudioClip[] Sounds => sounds;
}