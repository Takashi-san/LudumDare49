using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayTimeline : MonoBehaviour {
    [SerializeField] Image _image = null;
    [SerializeField] GameObject _notePrefab = null;

    [SerializeField] Sprite _yellowLine = null;
    [SerializeField] Sprite _purpleLine = null;
    [SerializeField] Sprite _blueLine = null;
    [SerializeField] Sprite _greenLine = null;
    
    SuitType _suitType;
    int _musicLength;
    List<MusicNote> _suitSheet = new List<MusicNote>();
    Dictionary<int, UIGameplayNote> _noteDict = new Dictionary<int, UIGameplayNote>();

    public void Setup(SuitType p_suitType, List<MusicNote> p_suitSheet, int p_musicLength) {
        _suitType = p_suitType;
        _suitSheet = p_suitSheet;
        _musicLength = p_musicLength;

        SetupLine();
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

    void SetupLine() {
        switch (_suitType) {
            case SuitType.Chord:
                _image.sprite = _yellowLine;
                break;
            
            case SuitType.Metal:
                _image.sprite = _blueLine;
                break;
            
            case SuitType.Wood:
                _image.sprite = _purpleLine;
                break;
            
            case SuitType.Percussion:
                _image.sprite = _greenLine;
                break;
        }
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
