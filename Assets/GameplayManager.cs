using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

    GameState _gameState = GameState.PRE_RUNNING;

    [SerializeField] BoardManager _boardManager;
    [SerializeField] UIGameplayBoard _uIGameplayBoard;
    [SerializeField] InputManager _inputManager;
    [SerializeField] MusicSheetManager _musicSheetManager;

    [SerializeField] AllMusicData _allMusicData;
    [SerializeField] int _musicIndex;
    [SerializeField] int _missInput;
    [SerializeField] int _goodInputDistance;
    [SerializeField] int _perfectInputDistance;
    [SerializeField] int _perfectPoints;
    [SerializeField] int _goodPoints;
    [SerializeField] int _simpleHitPoints;
    [SerializeField] int _missedPoints;
    [SerializeField] int _penaltyPoints;
    [SerializeField] int _maxLife;
    int _currentLife;

    void AddPoints(int points)
    {
        _currentLife = Mathf.Clamp(_currentLife + points, -1, _maxLife);
        _onLifeChange?.Invoke(_currentLife);
    }

    public System.Action<int> _onLifeChange;
    public System.Action<int> _onTimelineChange;
    public System.Action<SuitType, bool> _onInputChange;
    public System.Action<SuitType, MusicNote> _onDestroyNote;

    void Awake()
    {
#if DEBUG
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
        debugWindow.AddSession("Nearests:", DebugNearests);
#endif
    }

    private void OnEnable()
    {
        _inputManager.AddListener(OnAction);
    }

    private void OnDisable()
    {
        _inputManager.RemoveListener(OnAction);
    }

    void Start()
    {
        AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
        MusicSheet musicSheet = MusicSheetManager.Instance.MusicSheetList.Find(s => s.FileName == musicData.Name);
        AudioManager.Instance.GetMusicInfo(musicData.MusicRef).getLength(out int length);
        _uIGameplayBoard.Setup(musicSheet, length, this);

        _currentLife = _maxLife;
        _onLifeChange?.Invoke(_currentLife);
    }

    Dictionary<SuitType, MusicNote> _nearestNotes = new Dictionary<SuitType, MusicNote>();

    private void Update()
    {
        
        switch (_gameState)
        {
            case GameState.PRE_RUNNING:
                break;
            case GameState.RUNNING:
                AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.getTimelinePosition(out int timelinePosition);
                UpdateNearest(timelinePosition);
                
                foreach (KeyValuePair<SuitType, MusicNote> pair in _nearestNotes)
                    if (pair.Value.hitTime < timelinePosition + _perfectInputDistance)
                        AudioManager.Instance.MuteSuitType(pair.Key, true);


                _onTimelineChange?.Invoke(timelinePosition);
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
            //Was not hit/updated with input
            if (note.hitTime > toStepNote.hitTime)
            {
                Debug.Log($"Missed note {suitType} at {toStepNote.hitTime}, updating to {note.hitTime}");
                AddPoints(_missedPoints);

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
        GameState previous = _gameState;
        _gameState = newState;
        OnStateChange(previous, newState);
    }
    void OnStateChange(GameState previous, GameState newState)
    {
        switch (newState)
        {
            case GameState.PRE_RUNNING:
                break;
            case GameState.RUNNING:
                if (previous == GameState.PRE_RUNNING)
                {
                    AudioManager.Instance.PlayTrack(_allMusicData.GetMusic(_musicIndex).MusicRef, AudioManager.EAudioLayer.MUSIC);
                }
                break;
            case GameState.POS_RUNNING:
                break;
            case GameState.PAUSE:
                break;
        }
    }

    void OnAction(SuitType naipe, InputAction.CallbackContext context)
    {
        _onInputChange?.Invoke(naipe, context.started || context.performed);
        int timelinePosition = AudioManager.Instance.GetMusicTimelinePosition();
        if (context.started)
        {
            if (_gameState == GameState.RUNNING)
                EvaluatePress(naipe, timelinePosition);

            _boardManager.PressNaipe(naipe, true);
        }
        else if (context.canceled)
        {
            if (_gameState == GameState.RUNNING)
                EvaluateRelease(naipe, timelinePosition);

            _boardManager.PressNaipe(naipe, false);
        }
    }

    void EvaluatePress(SuitType naipe, int timelinePosition)
    {
        MusicNote note = _nearestNotes[naipe];
        string debugString = $" {naipe.ToString()} timeline at {timelinePosition} nearest note {note.noteType.ToString()} at {note.hitTime}";
        if(Mathf.Abs(timelinePosition - note.hitTime) < _missInput)
        {
            if (note.noteType == MusicNoteType.Avoid)
            {
                Debug.Log("BOOOM" + debugString);
                AddPoints(_penaltyPoints);
                AudioManager.Instance.MuteSuitType(naipe, true);
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

                AudioManager.Instance.MuteSuitType(naipe, false);
            }

            AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
            _nearestNotes[naipe] = _musicSheetManager.GetNearestNote(musicData.Name, naipe, _nearestNotes[naipe].hitTime, 0);

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
            
            case GameState.PAUSE:
                SetState(GameState.RUNNING);
                break;
        }

        if(_gameState == GameState.RUNNING && debugWindow.Button("Pause"))
            _gameState = GameState.PAUSE;
    }

    void DebugNearests(System.Func<bool, int> nextLine)
    {
        foreach (KeyValuePair<SuitType, MusicNote> pair in _nearestNotes)
            debugWindow.Label($"{pair.Key}: hitTime:{pair.Value.hitTime} type:{pair.Value.noteType.ToString()}");
    }

#endif
}
