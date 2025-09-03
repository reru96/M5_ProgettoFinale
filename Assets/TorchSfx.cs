using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class TorchSfx : MonoBehaviour
{
    [SerializeField] private string sfxKey = "torch_fire"; 
    [SerializeField, Range(0f, 1f)] private float volume = 1f;
    [SerializeField, Range(0f, 1f)] private float spatialBlend = 1f; 

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = spatialBlend;
        audioSource.volume = volume;

        var clip = AudioManager.Instance.GetClip(sfxKey);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
     
    }

    public void Play()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void Stop()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}
