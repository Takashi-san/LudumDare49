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

    public void Setup(MusicSheet p_musicSheet, int p_musicLength, GameplayManager gameplayManager) {
        _musicSheet = p_musicSheet;
        _musicLength = p_musicLength;
        gameplayManager.OnInputReceived += NewInput;
        gameplayManager._onDestroyNote += DestroyNote;
        gameplayManager._onTimelineChange += MusicProgressUpdate;
        gameplayManager.OnStageComplete += GameFinished;

        SetupTimelines();
        SetupInputs();
    }

    void SetupTimelines() {
        //foreach (var suit in _musicSheet.SuitSheets) {
        foreach (SuitType suit in System.Enum.GetValues(typeof(SuitType)))
        {
            if (!_musicSheet.SuitSheets.ContainsKey(suit))
                continue;

            UIGameplayTimeline timeline = Instantiate(_timelinePrefab, Vector3.zero, Quaternion.identity, _timelineParent).GetComponent<UIGameplayTimeline>();
            timeline.Setup(suit, _musicSheet.SuitSheets[suit], _musicLength);
            _timelineDict.Add(suit, timeline);
        }
    }

    void SetupInputs() {
        foreach (SuitType suit in System.Enum.GetValues(typeof(SuitType)))
        {
            if (!_musicSheet.SuitSheets.ContainsKey(suit))
                continue;

            GameObject inputPrefab = Instantiate(_inputPrefab, Vector3.zero, Quaternion.identity, _inputParent);
            UIGameplayInput input = inputPrefab.GetComponent<UIGameplayInput>();
            input.Setup(suit);
            _inputDict.Add(suit, input);

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

    void GameFinished(bool p_won) {
        gameObject.SetActive(false);
    } 
}
