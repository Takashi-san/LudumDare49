using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] GameObject _mainScreen = null;
    [SerializeField] GameObject _creditScreen = null;
    [SerializeField] GameObject _controlScreen = null;
    [SerializeField] GameObject _musicSelectScreen = null;
    [SerializeField] GameObject _logo = null;

    [Header("Timeline")]
    [SerializeField] GameObject _timelinePrefab = null;
    [SerializeField] RectTransform _timelineParent = null;

    [Header("Input")]
    [SerializeField] GameObject _inputPrefab = null;
    [SerializeField] RectTransform _inputParent = null;

    Dictionary<SuitType, UIGameplayTimeline> _timelineDict = new Dictionary<SuitType, UIGameplayTimeline>();
    Dictionary<SuitType, UIGameplayInput> _inputDict = new Dictionary<SuitType, UIGameplayInput>();

    enum screen {
        main,
        credit,
        control,
        musicSelect
    }

    void Start() {
        SetScreen(screen.main);
        InputManager input = FindObjectOfType<InputManager>();
        if (input != null) {
            input.AddGameplayListener(OnInput);
        }

        SetupTimelines();
        SetupInputs();
    }

    void OnInput(SuitType naipe, InputAction.CallbackContext context)
    {
        NewInput(naipe, context.started || context.performed);
    }

    void NewInput(SuitType p_suitType, bool p_isholding) {
        if (_inputDict.ContainsKey(p_suitType)) {
            _inputDict[p_suitType].DoInput(p_isholding);
        }
    }

    void SetupTimelines() {
        foreach (SuitType suit in System.Enum.GetValues(typeof(SuitType)))
        {
            UIGameplayTimeline timeline = Instantiate(_timelinePrefab, Vector3.zero, Quaternion.identity, _timelineParent).GetComponent<UIGameplayTimeline>();
            _timelineDict.Add(suit, timeline);
        }
    }

    void SetupInputs() {
        foreach (SuitType suit in System.Enum.GetValues(typeof(SuitType)))
        {
            GameObject inputPrefab = Instantiate(_inputPrefab, Vector3.zero, Quaternion.identity, _inputParent);
            UIGameplayInput input = inputPrefab.GetComponent<UIGameplayInput>();
            input.Setup(suit);
            _inputDict.Add(suit, input);
        }
    }
    
    void SetScreen(screen p_screen) {
        _mainScreen.SetActive(false);
        _creditScreen.SetActive(false);
        _controlScreen.SetActive(false);
        _musicSelectScreen.SetActive(false);
        _logo.SetActive(false);

        switch(p_screen) {
            case screen.main:
                _mainScreen.SetActive(true);
                _logo.SetActive(true);
                break;
            
            case screen.credit:
                _creditScreen.SetActive(true);
                break;
            
            case screen.control:
                _controlScreen.SetActive(true);
                break;
            
            case screen.musicSelect:
                _musicSelectScreen.SetActive(true);
                break;
        }
    }

    void SetupMusicSelect() {
        // wip
    }
    
    #region Buttom
    public void GoStartGame() {
        // go to storytelling
        GameManager.Instance.LoadScene("Gameplay");
    }

    public void GoMusicSelect() {
        // go to music select
        SetScreen(screen.musicSelect);
    }

    public void GoControls() {
        // go to controls
        SetScreen(screen.control);
    }

    public void GoCredits() {
        // go to credits
        SetScreen(screen.credit);
    }

    public void GoMain() {
        // go to main
        SetScreen(screen.main);
    }
    #endregion
}
