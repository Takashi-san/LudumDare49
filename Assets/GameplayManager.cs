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
    [SerializeField] float _badInputDistance;
    [SerializeField] float _goodInputDistance;
    [SerializeField] float _perfectInputDistance;
    AudioManager.AudioPack _playingTrack;

    public System.Action<int> _onTimelineChange;
    public System.Action<SuitType, bool> _onInputChange;
    public System.Action<SuitType, MusicNote> _onDestroyNote;

    void Awake()
    {
#if DEBUG
        debugWindow = new DebugWindow(this.ToString(), DebugContents);
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
        MusicSheet musicSheet = MusicSheetManager.Instance.MusicSheetList.Find( s => s.FileName == musicData.Name);
        AudioManager.Instance.GetMusicInfo(musicData.MusicRef).getLength(out int length);
        Debug.Log("Do setup");
        _uIGameplayBoard.Setup(musicSheet, length, this);
    }

    private void Update()
    {
        
        switch (_gameState)
        {
            case GameState.PRE_RUNNING:
                break;
            case GameState.RUNNING:
                AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC).EventInstance.getTimelinePosition(out int timelinePosition);
                Debug.Log($"before event timelinePosition {timelinePosition}");
                _onTimelineChange?.Invoke(timelinePosition);
                Debug.Log("after event");
                break;
            case GameState.POS_RUNNING:
                break;
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
                    _playingTrack = AudioManager.Instance.GetTrack(AudioManager.EAudioLayer.MUSIC);
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
        Debug.Log("OnAction");
        _onInputChange?.Invoke(naipe, context.started || context.performed);

        if (context.started)
        {
            Debug.Log("press");

            if (_gameState == GameState.RUNNING)
            {
                _playingTrack.EventInstance.getTimelinePosition(out int timeLinePosition);
                Debug.Log($"music is at: {timeLinePosition} == {timeLinePosition / 1000.0}");
                EvaluatePress(naipe, timeLinePosition);
            }

            _boardManager.PressNaipe(naipe, true);
        }
        else if (context.canceled)
        {
            Debug.Log("release");

            if (_gameState == GameState.RUNNING)
            {
                _playingTrack.EventInstance.getTimelinePosition(out int timeLinePosition);
                Debug.Log($"music is at: {timeLinePosition} == {timeLinePosition / 1000.0}");
                EvaluateRelease(naipe, timeLinePosition);
            }

            _boardManager.PressNaipe(naipe, false);
        }
    }

    void EvaluatePress(SuitType naipe, int timelinePosition)
    {
        AllMusicData.MusicData musicData = _allMusicData.GetMusic(_musicIndex);
        Debug.Log($"Evaluate press on {musicData.Name}");

        MusicNote note = _musicSheetManager.GetNearestNote(musicData.Name, naipe, timelinePosition);
        Debug.Log($"nearest note {note.noteType.ToString()} at {note.hitTime}");

        if(Mathf.Abs(timelinePosition - note.hitTime) < _badInputDistance)
        {
            if (note.noteType == MusicNoteType.Avoid)
            {
                Debug.Log("BOOOM");
            }
            else
            {
                if (Mathf.Abs(timelinePosition - note.hitTime) < _goodInputDistance)
                {
                    if (Mathf.Abs(timelinePosition - note.hitTime) < _perfectInputDistance)
                    {
                        Debug.Log("Perfect");
                    }
                    else
                    {
                        Debug.Log("Good");
                    }
                }
                else
                {
                    Debug.Log("Bad");
                }
            }
            _onDestroyNote?.Invoke(naipe, note);
        }
    }

    void EvaluateRelease(SuitType naipe, int timelinePosition)
    {

    }



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

#endif
}
