using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SceneMusicLibrary", menuName = "Scene Music Library")]
public class SceneMusicLibrary : ScriptableObject
{
 
        [System.Serializable]
        public class SceneMusicEntry
        {
            public string sceneName;
            public string musicKey; 
        }

    public List<SceneMusicEntry> sceneMusics = new List<SceneMusicEntry>();
}
