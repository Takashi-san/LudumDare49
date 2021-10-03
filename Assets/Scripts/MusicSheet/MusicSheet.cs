using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class MusicSheet {
    public string Name {
        get => _name;
        set { _name = value; }
    }
    public Dictionary<SuitType, List<MusicNote>> SuitSheets => _suitSheets;
    public string FileName;
    public AllMusicData.MusicData MusicData;
    string _name;
    Dictionary<SuitType, List<MusicNote>> _suitSheets = new Dictionary<SuitType, List<MusicNote>>();
    
    public void AddNote(SuitType p_suitType, MusicNoteType p_noteType, int p_hitTime) {
        MusicNote musicNote = new MusicNote();
        musicNote.noteType = p_noteType;
        musicNote.hitTime = p_hitTime;

        if (!_suitSheets.ContainsKey(p_suitType)) {
            _suitSheets.Add(p_suitType, new List<MusicNote>());
        }
        _suitSheets[p_suitType].Add(musicNote);
    }

    public void PrintValues() {
        Debug.Log($"{_name} =============================");
        foreach (var sheet in _suitSheets) {
            Debug.Log($"{sheet.Key} -----------------");
            foreach (var note in sheet.Value) {
                Debug.Log($"{note.noteType} - {note.hitTime}");
            }
        }
    }
}

[Serializable]
public struct MusicNote {
    public MusicNoteType noteType;
    public int hitTime;
}