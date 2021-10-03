using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AllMusicData")]
public class AllMusicData : ScriptableObject
{
    [System.Serializable]
    public class MusicData
    {
        [SerializeField] public TextAsset _textAsset;
        [SerializeField, FMODUnity.EventRef] public string _music;
        public string Name => _textAsset.name;
        public string MusicRef => _music;
    }

    [SerializeField] public List<MusicData> _musics = new List<MusicData>();

    public MusicData GetMusic(int index) => _musics[index];
}
