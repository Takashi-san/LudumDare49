using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameplayMaestro : MonoBehaviour {
    const float SWITCH_TIME = 0.25f;
    const float RETURN_TO_IDLE_TIME = 1f;
    
    [SerializeField] Image _image = null;
    [SerializeField] List<Sprite> _goodSprites = new List<Sprite>();
    [SerializeField] List<Sprite> _mediumSprites = new List<Sprite>();
    [SerializeField] List<Sprite> _badSprites = new List<Sprite>();

    int[] _selectedPattern;
    state _currentState = state.good;
    bool _step = true;
    Coroutine _animation;
    Coroutine _idleCoroutine;
    bool _gameFinished = false;

    enum state {
        good,
        medium,
        bad
    }

    #region animation pattern
    int[] _patternIdle = {8, 8};
    int[] _patternVictory = {0, 4};
    int[] _patternDefeat = {0, 0};

    int[] _patternWaveHard = {0, 1};
    int[] _patternWaveSoft = {2, 1};

    int[] _patternLeftHard = {7, 3};
    int[] _patternLeftSoft = {2, 3};

    int[] _patternRightHard = {5, 6};
    int[] _patternRightSoft = {2, 6};
    #endregion

    void Start() {
        _selectedPattern = _patternIdle;
        
        GameplayManager gameplayManager = FindObjectOfType<GameplayManager>();
        if (gameplayManager != null) {
            gameplayManager.OnLifeChange += UpdateLife;
            gameplayManager._onInputChange += NewInput;
            // subscribe to win/loss -> GameFinished
        }
        _animation = StartCoroutine(DoAnimation());
    }

    void UpdateLife(int p_life) {
        int destructionStage = (11 - p_life) / 4;
        destructionStage = Mathf.Clamp(destructionStage, 0, 2);
        _currentState = (state) destructionStage;
    }

    void NewInput(SuitType p_suitType, bool p_isholding) {
        if (_gameFinished || !p_isholding) {
            return;
        }

        switch (p_suitType) {
            case SuitType.Chord:
                _selectedPattern = _currentState != state.bad ? _patternLeftSoft : _patternLeftHard;
                break;
            
            case SuitType.Metal:
                _selectedPattern = _currentState != state.bad ? _patternWaveSoft : _patternWaveHard;
                break;
            
            case SuitType.Wood:
                _selectedPattern = _currentState != state.bad ? _patternWaveSoft : _patternWaveHard;
                break;
            
            case SuitType.Percussion:
                _selectedPattern = _currentState != state.bad ? _patternRightSoft : _patternRightHard;
                break;
        }

        if (_idleCoroutine != null) {
            StopCoroutine(_idleCoroutine);
        }
        _idleCoroutine = StartCoroutine(ReturnIdle());
    }

    void GameFinished(bool p_won) {
        if (p_won) {
            _selectedPattern = _patternVictory;
        }
        else {
            _selectedPattern = _patternDefeat;
        }

        if (_idleCoroutine != null) {
            StopCoroutine(_idleCoroutine);
        }
        _gameFinished = true;
    }

    List<Sprite> GetList() {
        switch (_currentState) {
            case state.bad:
                return _badSprites;
            
            case state.medium:
                return _mediumSprites;
            
            case state.good:
            default:
                return _goodSprites;
        }
    }

    IEnumerator DoAnimation() {
        while(true) {
            _image.sprite = _step ? GetList()[_selectedPattern[0]] : GetList()[_selectedPattern[1]];
            _step = !_step;
            yield return new WaitForSeconds(SWITCH_TIME);
        }
    }

    IEnumerator ReturnIdle() {
        yield return new WaitForSeconds(RETURN_TO_IDLE_TIME);
        _selectedPattern = _patternIdle;
    }
}
