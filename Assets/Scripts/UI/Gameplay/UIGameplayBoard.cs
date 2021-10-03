using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIGameplayBoard : MonoBehaviour {
    [Header("Timeline")]
    [SerializeField] GameObject _timelinePrefab = null;
    [SerializeField] RectTransform _timelineParent = null;

    [Header("Input")]
    [SerializeField] GameObject _inputPrefab = null;
    [SerializeField] RectTransform _inputParent = null;
    
    MusicSheet _musicSheet;
    int _musicLength;
    Dictionary<SuitType, UIGameplayTimeline> _timelineDict = new Dictionary<SuitType, UIGameplayTimeline>();
    Dictionary<SuitType, UIGameplayInput> _inputDict = new Dictionary<SuitType, UIGameplayInput>();
    // Dictionary<SuitType, UIGameplayInput> _inputFeedbackDict = new Dictionary<SuitType, UIGameplayInput>();
    Action<SuitType> _inputAction;

    public void Setup(MusicSheet p_musicSheet, int p_musicLength, Action<int> p_musicProgressAction, Action<SuitType, bool> p_inputAction, Action<SuitType, MusicNote> p_destroyNoteAction) {
        _musicSheet = p_musicSheet;
        _musicLength = p_musicLength;
        p_inputAction += NewInput;
        p_destroyNoteAction += DestroyNote;
        p_musicProgressAction += MusicProgressUpdate;

        SetupTimelines();
        SetupInputs();
    }

    void SetupTimelines() {
        foreach (var suit in _musicSheet.SuitSheets) {
            UIGameplayTimeline timeline = Instantiate(_timelinePrefab, Vector3.zero, Quaternion.identity, _timelineParent).GetComponent<UIGameplayTimeline>();
            timeline.Setup(suit.Key, suit.Value, _musicLength);
            _timelineDict.Add(suit.Key, timeline);
        }
    }

    void SetupInputs() {
        foreach (var suit in _musicSheet.SuitSheets) {
            GameObject inputPrefab = Instantiate(_inputPrefab, Vector3.zero, Quaternion.identity, _inputParent);
            
            UIGameplayInput input = inputPrefab.GetComponent<UIGameplayInput>();
            input.Setup(suit.Key);
            _inputDict.Add(suit.Key, input);

            // add feedback
        }
    }

    void MusicProgressUpdate(int p_musicProgress) {
        foreach (var timeline in _timelineDict) {
            timeline.Value.MusicProgressUpdate(p_musicProgress);
        }
    }

    void NewInput(SuitType p_suitType, bool p_isholding) {
        if (_inputDict.ContainsKey(p_suitType)) {
            _inputDict[p_suitType].DoInput(p_isholding);
        }
    }

    void DestroyNote(SuitType p_suitType, MusicNote p_musicNote) {
        if (_timelineDict.ContainsKey(p_suitType)) {
            _timelineDict[p_suitType].DestroyNote(p_musicNote);
        }
    }
}
