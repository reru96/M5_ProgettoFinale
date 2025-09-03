using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Audio Library", menuName ="Audio")]
public class AudioLibrary : ScriptableObject
{
    [System.Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
    }

    public List<AudioEntry> clips = new List<AudioEntry>();
}
