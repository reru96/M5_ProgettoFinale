using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMusic : MonoBehaviour
{
    [SerializeField] private AudioClip levelMusic;
    [SerializeField] private float volume;
    [SerializeField] private string soundGroup = "SfxVolume";
    [SerializeField] private int number = 2;

    private void Start()
    {
        if (AudioManager.Instance != null && levelMusic != null)
        {
            AudioManager.Instance.PlayMusic(levelMusic, number);
            AudioManager.Instance.SetVolume(volume, soundGroup);
        }
    }
}
