using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayTimeline : MonoBehaviour {
    [SerializeField] GameObject _notePrefab = null;
    
    SuitType _suitType;
    int _musicLength;
    List<MusicNote> _suitSheet = new List<MusicNote>();
    Dictionary<int, UIGameplayNote> _noteDict = new Dictionary<int, UIGameplayNote>();

    public void Setup(SuitType p_suitType, List<MusicNote> p_suitSheet, int p_musicLength) {
        _suitType = p_suitType;
        _suitSheet = p_suitSheet;
        _musicLength = p_musicLength;

        SetupNotes();
    }

    public void DestroyNote(MusicNote p_musicNote) {
        if (_noteDict.ContainsKey(p_musicNote.hitTime)) {
            _noteDict[p_musicNote.hitTime]?.DestroyNote();
        }
    }

    public void MusicProgressUpdate(int p_musicProgress) {
        SetNotesPosition(p_musicProgress);
    }

    void SetupNotes() {
        CreateNotes();
        SetNotesPosition(0);
    }

    void CreateNotes() {
        foreach (var musicNote in _suitSheet) {
            UIGameplayNote uiNote = Instantiate(_notePrefab, Vector3.zero, Quaternion.identity, transform).GetComponent<UIGameplayNote>();
            uiNote.Setup(_suitType, musicNote, _musicLength);
            _noteDict.Add(musicNote.hitTime, uiNote);
        }
    }

    void SetNotesPosition(int p_musicProgress) {
        foreach (var note in _noteDict) {
            note.Value?.MusicProgressUpdate(p_musicProgress);
        }
    }
}
