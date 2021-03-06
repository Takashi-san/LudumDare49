using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
public class GameplayManager : MonoBehaviour
#if DEBUG
    , IDebugable
#endif
{
    public enum GameState
    {
        PRE_RUNNING,
        RUNNING,
        POS_RUNNING,
        PAUSE
    }

    GameState _gameState;

    [SerializeField] UIGameplayBoard _uIGameplayBoard;
    [SerializeField] InputManager _inputManager;
    [SerializeField] MusicSheetManager _musicSheetManager;
    [SerializeField] PauseMenuController _pauseMenu;

    [SerializeField] AllMusicData _allMusicData;
    [SerializeField] public int _musicIndex;
    [SerializeField] int _missInput;
    [SerializeField] int _goodInputDistance;
    [SerializeField] int _perfectInputDistance;
    [SerializeField] int _perfectPoints;
    [SerializeField] int _goodPoints;
    [SerializeField] int _simpleHitPoints;
    [SerializeField] int _missedPoints;
    [SerializeField] int _penaltyPoints;
    [SerializeField] int _maxLife;
    [SerializeField] MusicianSet[] _musicianSets;
    [SerializeField, FMODUnity.EventRef] string _preRunningSound;
    [SerializeField, FMODUnity.EventRef] string _goodFeedback;
    [SerializeField, FMODUnity.EventRef] string _fakeNoteFeedback;
    [SerializeField, FMODUnity.EventRef] string[] _badFeedback;
    [SerializeField] TextMeshProUGUI _timerText;
    int _currentLife;

    void AddPoints(int points)
    {
        _currentLife = Mathf.Clamp(_currentLife + points, -1, _maxLife);
        OnLifeChange?.Invoke(_currentLife);
        if (_currentLife < 0)
        {
            OnStageComplete?.Invoke(false);
            SetState(GameState.POS_RUNNING);
            foreach (MusicianSet m in _musicianSets)
                m.gameObject.SetActive(false);
        }
    }

    public System.Action<int> OnLifeChange;
    public System.Action<int> _onTimelineChange;
    public System.Action<SuitType, bool> OnInputReceived;
    public System.Action<SuitType, MusicNote> _onDestroyNote;
    public System.Action<bool> OnStageComplete;

    void Awake()
    {
#if DEBUG
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
        debugWindow.AddSession("Nearests:", DebugNearests);
#endif
        if (GameManager.Instance != null)
        {
            Debug.Log("Taken from GameManager");
            _musicIndex = GameManager.Instance.musicIndex;
        }
        
        _pauseMenu.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inputManager.AddGameplayListener(OnAction);
        _inputManager.AddListener(OnInputEvent);
    }

    private void OnDisable()
    {
        _inputManager.RemoveGameplayListener(OnAction);
        _inputManager.RemoveListener(OnInputEvent);
    }

    void OnInputEvent(InputAction.CallbackContext context)
    {
        if (context.action.name == "pause" && context.started)
        {
            Debug.Log($"context.started: {context.started}");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (_gameState != GameState.RUNNING && _gameState != GameState.PAUSE)
            return;

        SetState(_gameState != GameState.PAUSE ? GameState.PAUSE : GameState.RUNNING);
    }

    void Start()
    {
        SetState(GameState.PRE_RUNNING);
        
        AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
        MusicSheet musicSheet = MusicSheetManager.Instance.MusicSheetList.Find(s => s.FileName == musicData.Name);
        
        MusicSheetManager.Instance.MusicSheetsLoaded += OnMusicSheetsLoaded;
        if(musicSheet == null) {
            
        }
        else
        {
            OnMusicSheetsLoaded();
        }

        _currentLife = _maxLife;
        OnLifeChange?.Invoke(_currentLife);
    }

    void OnMusicSheetsLoaded()
    {
        AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
        MusicSheet musicSheet = MusicSheetManager.Instance.MusicSheetList.Find(s => s.FileName == musicData.Name);
        AudioManager.Instance.GetMusicInfo(musicData.MusicRef).getLength(out int length);

        _uIGameplayBoard.Setup(musicSheet, length, this);
    }

    Dictionary<SuitType, MusicNote> _nearestNotes = new Dictionary<SuitType, MusicNote>();

    float _timer = 3;
    private void Update()
    {
        
        switch (_gameState)
        {
            case GameState.PRE_RUNNING:
                _timer -= Time.deltaTime;
                _timerText.text = ((int)_timer).ToString();
                if (_timer < 0)
                {
                    _timerText.gameObject.SetActive(false);
                    SetState(GameState.RUNNING);
                }
                break;
            case GameState.RUNNING:
                AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.getTimelinePosition(out int timelinePosition);
                UpdateNearest(timelinePosition);

                foreach (KeyValuePair<SuitType, MusicNote> pair in _nearestNotes)
                    if (pair.Value.hitTime != 0 &&                                          //ended
                        pair.Value.hitTime < timelinePosition + _perfectInputDistance &&    //after perfect timing
                        pair.Value.noteType != MusicNoteType.Avoid)                         //not bad input
                        { 
                            AudioManager.Instance.MuteSuitType(pair.Key, true);
                            _musicianSets[(int)pair.Key].PlaySet(false);
                        }


                _onTimelineChange?.Invoke(timelinePosition);

                AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
                if (state == FMOD.Studio.PLAYBACK_STATE.STOPPED)
                {
                    OnStageComplete?.Invoke(true);
                    SetState(GameState.POS_RUNNING);
                }
                break;
            case GameState.POS_RUNNING:
                break;
        }

    }

    void UpdateNearest(int timelinePosition)
    {
        for (int i = 0; i < System.Enum.GetNames(typeof(SuitType)).Length; i++)
            UpdateNearest((SuitType)i, timelinePosition);
    }

    void UpdateNearest(SuitType suitType, int timelinePosition)
    {
        MusicNote note = _musicSheetManager.GetNearestNote(_allMusicData.GetMusic(_musicIndex).Name, suitType, timelinePosition, _missInput);
        if (note.hitTime == 0)
            return;

        if (_nearestNotes.TryGetValue(suitType, out MusicNote toStepNote))
        {
            
            if (note.hitTime > toStepNote.hitTime &&        // When hit by input, already updated to next
                toStepNote.hitTime != 0)                      // ended timeline
            {
                if (toStepNote.noteType != MusicNoteType.Avoid)
                {
                    Debug.Log($"Missed note {suitType} at {toStepNote.hitTime}, updating to {note.hitTime}");
                    AudioManager.Instance.PlayOneShotSound(_badFeedback[(int)suitType], transform.position);
                    AddPoints(_missedPoints);
                }

                _nearestNotes[suitType] = note;
            }
        }
        else
        {
            _nearestNotes.Add(suitType, note);
        }
    }

    void SetState(GameState newState)
    {
        Debug.Log($"State change to {newState}");
        GameState previous = _gameState;
        _gameState = newState;
        OnStateChange(previous, newState);
    }
    void OnStateChange(GameState previous, GameState newState)
    {
        switch (newState)
        {
            case GameState.PRE_RUNNING:
                AudioManager.Instance.PlayTrack(_preRunningSound, AudioManager.EAudioLayer.MUSIC, FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
                break;
            case GameState.RUNNING:
                if (previous == GameState.PRE_RUNNING)
                {
                    AudioManager.Instance.PlayTrack(_allMusicData.GetMusic(_musicIndex).MusicRef, AudioManager.EAudioLayer.MUSIC);
                }
                else
                {
                    AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.setPaused(false);
                    _pauseMenu.gameObject.SetActive(false);
                }
                break;
            case GameState.POS_RUNNING:
                break;
            case GameState.PAUSE:
                _pauseMenu.gameObject.SetActive(true);
                AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.setPaused(true);
                break;
        }
    }

    void OnAction(SuitType naipe, InputAction.CallbackContext context)
    {
        OnInputReceived?.Invoke(naipe, context.started || context.performed);
        int timelinePosition = AudioManager.Instance.GetMusicTimelinePosition();
        if (context.started)
        {
            if (_gameState == GameState.RUNNING)
                EvaluatePress(naipe, timelinePosition);
        }
        else if (context.canceled)
        {
            if (_gameState == GameState.RUNNING)
                EvaluateRelease(naipe, timelinePosition);
        }
    }

    void EvaluatePress(SuitType naipe, int timelinePosition)
    {
        if (!_nearestNotes.ContainsKey(naipe))
            return;

        MusicNote note = _nearestNotes[naipe];
        string debugString = $" {naipe.ToString()} timeline at {timelinePosition} nearest note {note.noteType.ToString()} at {note.hitTime}";
        if(Mathf.Abs(timelinePosition - note.hitTime) < _missInput)
        {
            if (note.noteType == MusicNoteType.Avoid)
            {
                Debug.Log("BOOOM" + debugString);
                AddPoints(_penaltyPoints);
                Debug.Log("deactivate audio");

                AudioManager.Instance.PlayOneShotSound(_fakeNoteFeedback, transform.position);
                AudioManager.Instance.MuteSuitType(naipe, true);
                _musicianSets[(int)naipe].PlaySet(false);
            }
            else
            {
                if (Mathf.Abs(timelinePosition - note.hitTime) < _goodInputDistance)
                {
                    if (Mathf.Abs(timelinePosition - note.hitTime) < _perfectInputDistance)
                    {
                        Debug.Log($"Perfect {debugString}");
                        AddPoints(_perfectPoints);
                    }
                    else
                    {
                        Debug.Log($"Good {debugString}");
                        AddPoints(_goodPoints);
                    }
                }
                else
                {
                    Debug.Log($"Bad {debugString}");
                    AddPoints(_simpleHitPoints);
                }

                Debug.Log("valid input, reactivate audio");
                AudioManager.Instance.MuteSuitType(naipe, false);
                _musicianSets[(int)naipe].PlaySet(true);
                AudioManager.Instance.PlayOneShotSound(_goodFeedback, transform.position);
            }

            AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
            MusicNote musicNote = _musicSheetManager.GetNearestNote(musicData.Name, naipe, _nearestNotes[naipe].hitTime, 0);
            
            _nearestNotes[naipe] = musicNote;

            _onDestroyNote?.Invoke(naipe, note);
        }
    }

    void EvaluateRelease(SuitType naipe, int timelinePosition){}


#if DEBUG
    public bool _debug = false;
    public bool debug { get => _debug; set => _debug = value; }
    public DebugWindow debugWindow { get; private set; }
    void OnGUI()
    {
        _debug = debugWindow.DrawWindow(_debug);
    }

    void DebugContents(System.Func<bool, int> nextLine)
    {
        debugWindow.Label($"gameState: {_gameState.ToString()}");
        debugWindow.Label($"life: {_currentLife}/{_maxLife}");
        switch (_gameState)
        {
            case GameState.PRE_RUNNING:
                if (debugWindow.Button("To Running"))
                    SetState(GameState.RUNNING);
                break;

            case GameState.RUNNING:
                if (debugWindow.Button("To PosRunning"))
                    SetState(GameState.POS_RUNNING);
                break;

            case GameState.POS_RUNNING:
                if (debugWindow.Button("EndLevel")){ }
                break;
        }

        if (_gameState == GameState.RUNNING && debugWindow.Button("Pause"))
            TogglePause();
    }

    void DebugNearests(System.Func<bool, int> nextLine)
    {
        foreach (KeyValuePair<SuitType, MusicNote> pair in _nearestNotes)
            debugWindow.Label($"{pair.Key}: hitTime:{pair.Value.hitTime} type:{pair.Value.noteType.ToString()}");
    }

#endif
}
