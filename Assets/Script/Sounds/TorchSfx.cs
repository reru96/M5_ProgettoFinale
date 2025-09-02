using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TorchSfx : MonoBehaviour
{
    [SerializeField] private SoundType sfxMusic;
    [SerializeField] private float volume = 1f;
    [SerializeField] private string soundGroup = "MusicVolume";
    [SerializeField] private float radius = 5f;
    [SerializeField] private LayerMask player;
    private AudioManager manager;

    private bool playerIsNear = false;
    private bool musicIsPlaying = false;
    void Update()
    {
        playerIsNear = CheckPlayerIsNear();

        if (playerIsNear)
        {
           
            AudioManager.Instance.PlaySound(sfxMusic, v => AudioManager.Instance.SetVolume(volume, soundGroup));
            musicIsPlaying = true;
        }
        
        if(!playerIsNear && musicIsPlaying)
        {
            musicIsPlaying = false;
        }

    }

    public bool CheckPlayerIsNear()
    {
        return Physics.CheckSphere(transform.position, radius, player);
    }  
}