using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private SoundType levelMusic;
    [SerializeField] private float volume;
    [SerializeField] private string soundGroup = "MusicVolume";  

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySound(levelMusic, v => AudioManager.Instance.SetVolume(volume, soundGroup));
        }
    }
}
