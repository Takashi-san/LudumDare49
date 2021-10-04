using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] GameObject _mainScreen = null;
    [SerializeField] GameObject _creditScreen = null;
    [SerializeField] GameObject _controlScreen = null;
    [SerializeField] GameObject _musicSelectScreen = null;
    
    enum screen {
        main,
        credit,
        control,
        musicSelect
    }

    void Start() {
        SetScreen(screen.main);
    }
    
    void SetScreen(screen p_screen) {
        _mainScreen.SetActive(false);
        _creditScreen.SetActive(false);
        _controlScreen.SetActive(false);
        _musicSelectScreen.SetActive(false);

        switch(p_screen) {
            case screen.main:
                _mainScreen.SetActive(true);
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
    
    #region Buttom
    public void GoStartGame() {
        // go to storytelling
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
